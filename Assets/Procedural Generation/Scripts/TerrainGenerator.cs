using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration {
    public class TerrainGenerator : MonoBehaviour
    {
        const float viewerMoveThreshold = 25f;
        const float viewerMoveThresholdSquared = viewerMoveThreshold*viewerMoveThreshold;


        public int colliderLODIndex;
        public LODInfo[] detailLevels;
    
        public Transform viewer;
        public Material mapMaterial;
    
        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureData textureSettings;

        private Vector2 viewerPosition;
        private Vector2 viewerLastPosition;
    
        int chunkSize;
        int chunksVisibleInViewDst;

        Dictionary<Vector2,TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

        private void Start() {
            textureSettings.ApplyToMaterial(mapMaterial);
            textureSettings.UpdateMeshHeights(mapMaterial,heightMapSettings.minHeight,heightMapSettings.maxHeight);


            float maxViewDst = detailLevels[detailLevels.Length-1].visibleDstThreshold;
            float meshWorldSize = meshSettings.meshWorldSize;
            chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst/chunkSize);
        
            UpdateVisibleChunks();
        }

        private void Update() {
            viewerPosition = new Vector2(viewer.position.x,viewer.position.z);

            if(viewerPosition != viewerLastPosition){
                foreach(TerrainChunk chunk in visibleTerrainChunks){
                    chunk.UpdateCollisionMesh();
                }
            }
        
            // if(Vector2.Distance(viewerLastPosition, ViewerPosition) > viewerMoveThreshold){
            if((viewerLastPosition-viewerPosition).sqrMagnitude > viewerMoveThresholdSquared){ //more optimized
                viewerLastPosition = viewerPosition;
                UpdateVisibleChunks();
            }
        }

        void UpdateVisibleChunks()
        {
            HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
            for (var i = visibleTerrainChunks.Count - 1; i >= 0 ; i--)
            {
                alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
                visibleTerrainChunks[i].UpdateTerrainChunk();
            }

            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x/chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y/chunkSize);

            for(int yOffset = -chunksVisibleInViewDst;yOffset <= chunksVisibleInViewDst; yOffset++){
                for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)){
                    
                        if(terrainChunkDictionary.ContainsKey(viewedChunkCoord)){
                            terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                            // if(terrainChunkDictionary[viewedChunkCoord].isVisible()){
                            //     TerrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                            // }
                        }
                        else {
                            TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
                            terrainChunkDictionary.Add(viewedChunkCoord,newChunk);
                            newChunk.OnVisibilityChanged += OnTerrainChunkVisibilityChanged;
                            newChunk.Load();
                        }
                    
                    }
                }
            }
        }

        void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool visible) {
            if (visible) {
                visibleTerrainChunks.Add(chunk);
            }
            else {
                visibleTerrainChunks.Remove(chunk);
            }
        }
    }

    [System.Serializable]
    public struct LODInfo{
        [Range(0,MeshSettings.numSupportedLODs - 1)]
        public int LOD;
        public float visibleDstThreshold;

        public float sqrVisibleDistThreshold{
            get{
                return visibleDstThreshold * visibleDstThreshold;
            }
        }
    }
}