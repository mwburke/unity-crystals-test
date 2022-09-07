using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalGenerator {

    // Base parameters
    private float minHeight;
    private float maxHeight;
    private int minPoints;
    private int maxPoints;
    private float minRingFraction;
    private float maxRingFraction;
    private float minBaseRadius;
    private float maxBaseRadius;
    private bool hasSameRingRadius;
    private float maxRingRadiusNoise;
    private bool hasFlatTop;
    private bool hasNoisyBasePoints;
    private float maxBasePointNoise;
    private bool hasNoisyRingPoints;
    private bool hasUpDownRingPoints;
    private float maxRingPointNoise;
    private float maxUprightAngle;
    private float maxRingAngle;

    // Derived parameters
    private float baseRadius;
    private float ringRadius;
    private Vector3 angleVector;
    private Vector3 ringAngleVector;
    private float totalHeight;
    private float ringFraction;
    private int numPoints;
    private float rotationOffset;
    private Vector3 heightVector;
    private Vector3 ringFractionVector;

    public CrystalGenerator(float minHeight, float maxHeight, float minRingFraction, float maxRingFraction, int minPoints, int maxPoints, float minBaseRadius, float maxBaseRadius, bool hasSameRingRadius, float maxRingRadiusNoise, bool hasFlatTop, bool hasNoisyBasePoints, bool hasUpDownRingPoints, float maxBasePointNoise, bool hasNoisyRingPoints, float maxRingPointNoise, float maxUprightAngle, float maxRingAngle) {
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.minRingFraction = minRingFraction;
        this.maxRingFraction = maxRingFraction;
        this.minPoints = minPoints;
        this.maxPoints = maxPoints;
        this.minBaseRadius = minBaseRadius;
        this.maxBaseRadius = maxBaseRadius;
        this.hasSameRingRadius = hasSameRingRadius;
        this.maxRingRadiusNoise = maxRingRadiusNoise;
        this.hasFlatTop = hasFlatTop;
        this.hasNoisyBasePoints = hasNoisyBasePoints;
        this.maxBasePointNoise = maxBasePointNoise;
        this.hasNoisyRingPoints = hasNoisyRingPoints;
        this.hasUpDownRingPoints = hasUpDownRingPoints;
        this.maxRingPointNoise = maxRingPointNoise;
        this.maxUprightAngle = maxUprightAngle;
        this.maxRingAngle = maxRingAngle;
    }


    public void InitializeDerivedParams() {
        numPoints = Random.Range(minPoints, maxPoints);
        totalHeight = Random.Range(minHeight, maxHeight);
        heightVector = new Vector3(totalHeight, totalHeight, totalHeight);
        angleVector = new Vector3(Random.Range(-maxUprightAngle / 2, maxUprightAngle / 2), 1, Random.Range(-maxUprightAngle / 2, maxUprightAngle / 2)).normalized;
        ringAngleVector = new Vector3(Random.Range(-maxRingAngle / 2, maxRingAngle / 2), 1, Random.Range(-maxRingAngle / 2, maxRingAngle / 2)).normalized;
        baseRadius = Random.Range(minBaseRadius, maxBaseRadius);
        if (hasSameRingRadius) {
            ringRadius = baseRadius;
        } else {
            ringRadius = baseRadius * (1f + Random.Range(-maxRingRadiusNoise, maxRingRadiusNoise));
        }
        if (hasFlatTop) {
            // If we want a flat top, we just set it as the ring center at the top of the crystal
            ringFraction = 1f;
        } else {
            ringFraction = Random.Range(minRingFraction, maxRingFraction);
        }
        
        ringFractionVector = new Vector3(ringFraction, ringFraction, ringFraction);
        rotationOffset = Random.Range(0, 2f * Mathf.PI);
    }


    public Vector3[] GenerateAnchorPoints() {
        Vector3[] anchorPoints = new Vector3[3];

        // Base point
        anchorPoints[0] = new Vector3(0, 0, 0);

        // Ring center
        anchorPoints[1] = Vector3.Scale(Vector3.Scale(angleVector, heightVector), ringFractionVector);

        // Top of crystal
        anchorPoints[2] = Vector3.Scale(angleVector, heightVector);

        return anchorPoints;
    }


    public Mesh GenerateCrystalMesh() {
        InitializeDerivedParams();

        Mesh mesh = new();
        
        Vector3[] anchorPoints = GenerateAnchorPoints();

        List<Vector3> vertices = GenerateVertices(anchorPoints);
        mesh.SetVertices(vertices);

        List<int> triangles = GenerateTriangles();
        mesh.SetTriangles(triangles, 0);

        // List<Vector2> uvs = GetStandardUvs(vertices, true, true);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }



    public List<Vector3> GenerateVertices(Vector3[] anchorPoints) {
        /* Generate the base level vertices in a rough polygon shape
         * The "ring" level vertices are the second level up, corresponding to the below ones
         * The top value is where the ring level vertices all converge.
         */
        List<Vector3> vertices = new();

        // Bottom center
        vertices.Add(anchorPoints[0]);

        Vector3 finalRingAngleVector = ringAngleVector;
        Vector3 rotOffsetVector = new Vector3(Mathf.Cos(rotationOffset), 0, Mathf.Sin(rotationOffset)).normalized;
        Vector3 zeroOffsetDir = Vector3.Cross(finalRingAngleVector, rotOffsetVector);

        // Base
        for (int i = 0; i < numPoints; i++) {

            float angle = (float)i / (float)numPoints * 360f;

            Quaternion rot = Quaternion.AngleAxis(angle, finalRingAngleVector);

            // Find the offset by rotating the "zero" offset, then extending it by the radius
            Vector3 offset = ringRadius * 2 * (rot * zeroOffsetDir);

            Vector3 vertPosition = anchorPoints[0] + offset;


            if (hasNoisyBasePoints) {
                Vector3 noiseOffset = new Vector3(
                    Random.Range(-0.5f, 0.5f) * maxBasePointNoise,
                    0f,
                    Random.Range(-0.5f, 0.5f) * maxBasePointNoise
                );

                vertPosition += noiseOffset;
            }

            vertices.Add(vertPosition);
        }

        // Ring
        // A lot of this pulled from https://stackoverflow.com/questions/63308574/in-unity-calculate-points-a-given-distance-perpendicular-to-a-line-at-angles-on
        finalRingAngleVector = (ringAngleVector + angleVector).normalized;
        // zeroOffsetDir += anchorPoints[1];

        for (int i = 0; i < numPoints; i++) {

            float angle = (float)i / (float)numPoints * 360f;

            Quaternion rot = Quaternion.AngleAxis(angle, finalRingAngleVector);

            // Find the offset by rotating the "zero" offset, then extending it by the radius
            Vector3 offset = ringRadius * 2 * (rot * zeroOffsetDir);

            Vector3 vertPosition = anchorPoints[1] + offset;

            // TODO: add in the angleVector
            // TODO: add in the ringAngleVector, do we add this up on one side, then down on the opposite?
            // and then just lerp between the two?

            if (hasNoisyRingPoints) {
                Vector3 noiseOffset = new Vector3(
                    Random.Range(-0.5f, 0.5f) * maxRingPointNoise,
                    Random.Range(-0.5f, 0.5f) * maxRingPointNoise,
                    Random.Range(-0.5f, 0.5f) * maxRingPointNoise
                );
                vertPosition += noiseOffset;
            } else if (hasUpDownRingPoints) {
                if (i % 2 == 0) {
                    // TODO: new parameter for this? or just readjust the factor
                    // What about odd number of points, don't allow?
                    vertPosition += finalRingAngleVector * maxRingPointNoise;
                } else {
                    vertPosition -= finalRingAngleVector * maxRingPointNoise;
                }
            }

            vertices.Add(vertPosition);
        }
        // Top
        vertices.Add(anchorPoints[2]);

        /*
         * DEBUG ONLY
        for (int i = 0; i < vertices.Count; i++) {
            GameObject sphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere3.transform.position = vertices[i];
        }*/

        return vertices;
    }

    public List<int> GenerateTriangles() {
        List<int> triangles = new();

        // Bottom center to base
        int baseOffset = 1;
        for (int i = 0; i < numPoints; i++) {
            triangles.Add(baseOffset + ((i + 1) % numPoints));
            triangles.Add(0);
            triangles.Add(i + baseOffset);
        }

        // Base to ring
        // Base rings are 1 to 1 + numPoints - 1
        // Rings are (numPoints) to (numPoints * 2 - 1)
        int ringOffset = numPoints + 1;
        
        for (int i = 0; i < numPoints; i++) {
            // First one
            triangles.Add(ringOffset + ((i + 1) % numPoints));
            triangles.Add(ringOffset + i);
            triangles.Add(baseOffset + i);

            // Second one
            triangles.Add(baseOffset + i);
            triangles.Add(baseOffset + ((i + 1) % numPoints));
            triangles.Add(ringOffset + ((i + 1) % numPoints));
        }

        // Ring to top
        // Rings are (numPoints) to (numPoints * 2 - 1) to 1 + numPoints * 2 as the top
        for (int i = 0; i < numPoints; i++) {
            triangles.Add(ringOffset + ((i + 1) % numPoints)); 
            triangles.Add(ringOffset + numPoints);
            triangles.Add(ringOffset + i);
        }

        return triangles;
    }
}
