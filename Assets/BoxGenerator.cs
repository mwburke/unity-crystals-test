using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxGenerator : MonoBehaviour
{

    public static Mesh GenerateBoxMesh(float sideLength) {
        Mesh mesh = new();

        List<Vector3> vertices = GenerateVertices(sideLength);
        mesh.SetVertices(vertices);

        List<int> triangles = GenerateTriangles();
        mesh.SetTriangles(triangles, 0);

        // List<Vector2> uvs = GetStandardUvs(vertices, true, true);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    public static List<Vector3> GenerateVertices(float sideLength) {
        // Vertices are clockwise on the bottom layer, then top layer
        // All sides start at zero origin
        List<Vector3> vertices = new();

        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, sideLength, 0));
        vertices.Add(new Vector3(sideLength, sideLength, 0));
        vertices.Add(new Vector3(sideLength, 0, 0));

        vertices.Add(new Vector3(0, 0, sideLength));
        vertices.Add(new Vector3(0, sideLength, sideLength));
        vertices.Add(new Vector3(sideLength, sideLength, sideLength));
        vertices.Add(new Vector3(sideLength, 0, sideLength));

        return vertices;
    }

    public static List<int> GenerateTriangles() {
        List<int> triangles = new();    

        // Something is wrong here with the clockwise ordering

        // Bottom
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(3);

        // Left
        triangles.Add(0);
        triangles.Add(4);
        triangles.Add(1);
        triangles.Add(1);
        triangles.Add(4);
        triangles.Add(5);

        // Back
        triangles.Add(1);
        triangles.Add(6);
        triangles.Add(5);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(6);

        // Right 
        triangles.Add(2);
        triangles.Add(7);
        triangles.Add(6);
        triangles.Add(2);
        triangles.Add(3);
        triangles.Add(7);

        // Front
        triangles.Add(0);
        triangles.Add(4);
        triangles.Add(7);
        triangles.Add(0);
        triangles.Add(7);
        triangles.Add(3);

        // Top
        triangles.Add(4);
        triangles.Add(5);
        triangles.Add(6);
        triangles.Add(4);
        triangles.Add(6);
        triangles.Add(7);

        return triangles;
    }
}
