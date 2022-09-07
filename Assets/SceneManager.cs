using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    [SerializeField] public Material material;

    [SerializeField] public float minHeight;
    [SerializeField] public float maxHeight;
    [SerializeField] public int minPoints;
    [SerializeField] public int maxPoints;
    [SerializeField] public float minRingFraction;
    [SerializeField] public float maxRingFraction;
    [SerializeField] public float minBaseRadius;
    [SerializeField] public float maxBaseRadius;
    [SerializeField] public bool hasSameRingRadius;
    [SerializeField] public float maxRingRadiusNoise;
    [SerializeField] public bool hasFlatTop;
    [SerializeField] public bool hasNoisyBasePoints;
    [SerializeField] public float maxBasePointNoise;
    [SerializeField] public bool hasNoisyRingPoints;
    [SerializeField] public bool hasUpDownRingPoints;
    [SerializeField] public float maxRingPointNoise;
    [SerializeField] public float maxUprightAngle;
    [SerializeField] public float maxRingAngle;

    public CrystalGenerator generator;
    private GameObject crystal;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        generator = new CrystalGenerator(minHeight, maxHeight, minRingFraction, maxRingFraction, minPoints, maxPoints, minBaseRadius, maxBaseRadius, hasSameRingRadius, maxRingRadiusNoise, hasFlatTop, hasNoisyBasePoints, hasUpDownRingPoints, maxBasePointNoise, hasNoisyRingPoints, maxRingPointNoise, maxUprightAngle, maxRingAngle);
        NewCrystalMesh();
    }

    void NewCrystalMesh() {
        Destroy(crystal);
        crystal = new GameObject("mesh", typeof(MeshFilter), typeof(MeshRenderer));
        Mesh crystalMesh = generator.GenerateCrystalMesh();
        crystal.GetComponent<MeshFilter>().mesh = crystalMesh;
        crystal.GetComponent<MeshRenderer>().material = material;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Mesh crystalMesh = generator.GenerateCrystalMesh();
            NewCrystalMesh();
        }
    }
}
