using UnityEngine;

namespace ProceduralMeshGeneration {
    public class MapPreview : MonoBehaviour
    {
        public Renderer textureRenderer;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;
        public enum DrawMode{
            NoiseMap,
            Mesh,
            FalloffMap
        }

        public DrawMode drawMode;

        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureData textureData;
    
        public Material terrainMaterial;

        [Range(0,MeshSettings.numSupportedLODs-1)]
        public int editorLOD;
        public bool autoUpdate;
    
        public string meshName;

        public void DrawMapInEditor(){
            textureData.ApplyToMaterial(terrainMaterial);
            textureData.UpdateMeshHeights(terrainMaterial,heightMapSettings.minHeight,heightMapSettings.maxHeight);
        
            HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine,meshSettings.numVertsPerLine,heightMapSettings,Vector2.zero);
        
            if(drawMode == DrawMode.NoiseMap){
                DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
            }
            else if(drawMode == DrawMode.Mesh){
                DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values,meshSettings, editorLOD));
            }
            else if(drawMode == DrawMode.FalloffMap){
                DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine,heightMapSettings.falloffParameters),0,1)));
            }
        }
    
        public void DrawTexture(Texture2D texture) {
            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(texture.width,1,texture.height)/10f;
        
            textureRenderer.gameObject.SetActive(true);
            meshFilter.gameObject.SetActive(false);
        }
    
        public void DrawMesh(MeshData meshData){
            meshFilter.sharedMesh = meshData.CreateMesh();
            meshCollider.sharedMesh = meshFilter.sharedMesh;

            textureRenderer.gameObject.SetActive(false);
            meshFilter.gameObject.SetActive(true);
        }
    
        void OnValuesUpdated(){
            if(!Application.isPlaying){
                DrawMapInEditor();
            }
        }
    
        void OnTextureValuesUpdated(){
            textureData.ApplyToMaterial(terrainMaterial);
        }

        private void OnValidate() {

            if(meshSettings != null){
                meshSettings.OnValuesUpdated -= OnValuesUpdated;
                meshSettings.OnValuesUpdated += OnValuesUpdated;
            }
            if(heightMapSettings != null){
                heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
                heightMapSettings.OnValuesUpdated += OnValuesUpdated;
            }
            if(textureData != null){
                textureData.OnValuesUpdated -= OnTextureValuesUpdated;
                textureData.OnValuesUpdated += OnTextureValuesUpdated;
            }
        }
    }
}