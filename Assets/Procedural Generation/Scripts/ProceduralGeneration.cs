using UnityEngine;

namespace ProceduralMeshGeneration {
    [RequireComponent(typeof(MeshFilter))]
    public class ProceduralGeneration : MonoBehaviour
    {
        Mesh mesh;
        Vector3[] vertices;
        int[] triangles;
        Vector2[] uvs;
        Color[] colors;
        float minY,maxY;
    
        public int xSize = 20;
        public int zSize = 20;
        public float xOffset = 0;
        public float zOffset = 0;
        public float xMultiplier = 1;
        public float yMultiplier = 1;
        public float zMultiplier = 1;
        public Gradient gradient;

        private void Start() {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

        }
        private void Update() {
            CreateShape();
            UpdateMesh();
        }

        void CreateShape(){
            minY = 0;
            maxY = 0;
            vertices = new Vector3[(xSize + 1) * (zSize + 1)];

            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++, i++)
                {
                    float y = Mathf.PerlinNoise((x*xMultiplier)+xOffset,(z*zMultiplier)+zOffset)*yMultiplier;
                    vertices[i] = new Vector3(x,y,z);
                    if(y<minY) minY = y;
                    if(y>maxY) maxY = y;
                }
            }

            triangles = new int[xSize*zSize*2*3];

            for (int i = 0, z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    int v1 = z*(xSize+1) + x;
                    int v2 = z*(xSize+1) + x + 1;
                    int v3 = (z+1)*(xSize+1) + x;
                    int v4 = (z+1)*(xSize+1) + x + 1;
                    triangles[i++] = v1;
                    triangles[i++] = v3;
                    triangles[i++] = v2;
                    triangles[i++] = v2;
                    triangles[i++] = v3;
                    triangles[i++] = v4;
                }
            }

            uvs = new Vector2[vertices.Length];

        
            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++, i++)
                {
                    uvs[i] = new Vector2((float)x/xSize,(float)z/zSize);
                }
            }

            colors = new Color[vertices.Length];
        
            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++, i++)
                {
                    float height = Mathf.InverseLerp(minY,maxY,vertices[i].y);
                    colors[i] = gradient.Evaluate(height);
                }
            }
        }

        void UpdateMesh(){
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.colors = colors;

            mesh.RecalculateNormals();
        }

        private void OnDrawGizmos() {
            if(vertices == null) return;
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], .1f);
            }  
        }
    }
}