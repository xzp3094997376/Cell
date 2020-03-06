using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CombineMesh  {
    [MenuItem("Plugins/合并选择mesh")]
    static void CombineMeshFunc()
    {
        GameObject[] objs = Selection.gameObjects;

        for (int j = 0; j < objs.Length; j++)
        {
            MeshFilter[] meshfilters = objs[j].GetComponentsInChildren<MeshFilter>();

            CombineInstance[] combine = new CombineInstance[meshfilters.Length];

            Matrix4x4 matrix = objs[j].transform.worldToLocalMatrix;

            for (int i = 0; i < meshfilters.Length; i++)
            {
                MeshFilter mf = meshfilters[i];

                MeshRenderer mr = mf.GetComponent<MeshRenderer>();

                if (mr == null) continue;

                combine[i].mesh = mf.sharedMesh;

                combine[i].transform = matrix * mf.transform.localToWorldMatrix;
            }
            Mesh mesh = new Mesh();

            mesh.name = objs[j].name;

            mesh.CombineMeshes(combine, true);

            string path = @"Assets/ComMesh/" + mesh.name + ".asset";

            AssetDatabase.CreateAsset(mesh, path);
        }
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("CombineMesh", "Combine successfully!", "OK", "");
    }
}
