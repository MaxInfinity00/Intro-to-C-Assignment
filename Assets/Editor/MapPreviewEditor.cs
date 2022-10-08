using ProceduralMeshGeneration;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MapPreview))]
public class MapPreviewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapPreview mapPreview = (MapPreview)target;

        if(DrawDefaultInspector()){
            if(mapPreview.autoUpdate){
                mapPreview.DrawMapInEditor();
            }
        }

        if(GUILayout.Button("Generate")){
            mapPreview.DrawMapInEditor();
            
        }
        if(GUILayout.Button("Save Mesh")){
            string uniquePathName = AssetDatabase.GenerateUniqueAssetPath("Assets/Procedural Generation/Saved Meshes/" + mapPreview.meshName + ".asset");
            AssetDatabase.CreateAsset(mapPreview.meshFilter.sharedMesh,uniquePathName);
            AssetDatabase.SaveAssets();
        }
    }
}