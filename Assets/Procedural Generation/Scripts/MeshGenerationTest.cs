using UnityEngine;

namespace ProceduralMeshGeneration {
    [RequireComponent(typeof(MeshFilter))]
    public class MeshGenerationTest : MonoBehaviour
    {
        Mesh mesh;
        Vector3[] vertices;
        int[] triangles;

        private void Start() {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

            CreateShape();
            UpdateMesh();
        }

        void CreateShape(){

            vertices = new Vector3[]{
                new Vector3(1,0,0),
                new Vector3(0,1,0),
                new Vector3(0,0,1)
            };

            triangles = new int[]{
                0,1,2
            };
        }

        void UpdateMesh(){
            Debug.Log("test");

            mesh.Clear();

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
        }
    }
}