using UnityEngine;

public class TerrainChunk {

    const float colliderGenerationDistThreshold = 5f;
    public event System.Action<TerrainChunk, bool> OnVisibilityChanged;
    public Vector2 coord;
    
    GameObject meshObject;
    Vector2 sampleCenter;
    Bounds bounds;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODInfo[] detailLevels;
    LODMesh[] LODMeshes;
    int colliderLODIndex;

    HeightMap heightMap;
    bool heightMapReceived;
    int previousLODIndex = -1;
    bool hasSetCollider;
    private float maxViewDst;

    private HeightMapSettings heightMapSettings;
    private MeshSettings meshSettings;
    private Transform viewer;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material)
    {
        this.coord = coord;
        
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;

        sampleCenter = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position,Vector2.one * meshSettings.meshWorldSize);

        // meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        meshObject = new GameObject("Terrain Chunk");

        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);;
        meshObject.transform.parent = parent;
        SetVisible(false);

        LODMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < LODMeshes.Length; i++){
            LODMeshes[i] = new LODMesh(detailLevels[i].LOD);
            LODMeshes[i].UpdateCallback += UpdateTerrainChunk;
            if(i == colliderLODIndex)
                LODMeshes[i].UpdateCallback += UpdateCollisionMesh;
        }

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
    }

    public void Load(){
        ThreadedDataRequester.RequestData(()=> HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCenter),OnHeightMapReceived);
    }

    void OnHeightMapReceived(object heightMapObject){
        heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;
        
        UpdateTerrainChunk();
    }

    Vector2 viewerPosition {
        get {
            return new Vector2(viewer.position.x,viewer.position.z );
        }
    }
    public void UpdateTerrainChunk(){
        if(!heightMapReceived) return;

        float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

        bool wasVisible = isVisible();
        bool visible = viewerDistanceFromNearestEdge <= maxViewDst;

        if(visible){
            int LODIndex = 0;
            for (int i = 0; i < detailLevels.Length-1; i++)
            {
                if(viewerDistanceFromNearestEdge > detailLevels[i].visibleDstThreshold){
                    LODIndex = i + 1;
                }
                else break;
            }
            if(LODIndex != previousLODIndex){
                LODMesh lODMesh = LODMeshes[LODIndex];
                if(lODMesh.hasMesh){
                    previousLODIndex = LODIndex;
                    meshFilter.mesh = lODMesh.mesh;
                }
                else if(!lODMesh.hasRequested){
                    lODMesh.RequestMesh(heightMap,meshSettings);
                }
            }

        }

        if (wasVisible != visible)
        {
            SetVisible(visible);
            OnVisibilityChanged?.Invoke(this,visible);
        }
        
    }

    public void UpdateCollisionMesh(){
        if(hasSetCollider) return;
        float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

        if(sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDistThreshold){
            if(!LODMeshes[colliderLODIndex].hasRequested){
                LODMeshes[colliderLODIndex].RequestMesh(heightMap,meshSettings);
            }
        }

        if(sqrDstFromViewerToEdge < colliderGenerationDistThreshold * colliderGenerationDistThreshold){
            if(LODMeshes[colliderLODIndex].hasMesh){
                meshCollider.sharedMesh = LODMeshes[colliderLODIndex].mesh;
                hasSetCollider = true;
            }
        }
    }

    public void SetVisible(bool visible){
        meshObject.SetActive(visible);
    }
    public bool isVisible(){
        return meshObject.activeSelf;
    }
}

public class LODMesh{

    public Mesh mesh;
    public bool hasRequested;
    public bool hasMesh;
    int LOD;
    public event System.Action UpdateCallback;

    public LODMesh(int LOD){
        this.LOD = LOD;
    }

    void OnMeshDataReceived(object meshDataObject){
        mesh = ((MeshData)meshDataObject).CreateMesh();
        hasMesh = true;
        UpdateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings){
        hasRequested = true;
        
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, LOD),OnMeshDataReceived);
    }
}