using UnityEngine;
using ProceduralMeshGeneration;

namespace IntroAssignment {
    public class LevelMeshGenerator {
        public static void GenerateLevel(MeshSettings meshSettings, HeightMapSettings heightMapSettings, MeshFilter meshFilter, MeshCollider meshCollider = null) {
            HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine,meshSettings.numVertsPerLine,heightMapSettings,Vector2.zero);
        
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap.values,meshSettings,0);
            Mesh mesh = meshData.CreateMesh();
            meshFilter.sharedMesh = mesh;
            if(meshCollider) meshCollider.sharedMesh = mesh;
        
            // textureRenderer.gameObject.SetActive(false);
            meshFilter.gameObject.SetActive(true);
        }
    }
}