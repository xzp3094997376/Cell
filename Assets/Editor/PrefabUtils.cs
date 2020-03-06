using UnityEditor;
using UnityEngine;
public class PrefabUtils : Editor
{
    /// <summary>
    /// 创建图片预制体
    /// </summary>
    [MenuItem("Assets/AssetsCreate/ImageToSpriteprefab", false, 1)]
    static void CreateSpriteUGUIPrefab()
    {
        string[] guid = Selection.assetGUIDs;
        string[] path = new string[] { AssetDatabase.GUIDToAssetPath(guid[0]) };
        Debug.Log(path[0]);
        string[] assets = AssetDatabase.FindAssets("t:texture2D", path);

        GameObject go = new GameObject("SpritePrefab");
        SpriteCtrl spCtrl = go.AddComponent<SpriteCtrl>();

        for (int i = 0; i < assets.Length; i++)
        {
            string spritePath = AssetDatabase.GUIDToAssetPath(assets[i]);
            Debug.Log(spritePath);
            spCtrl.Add(AssetDatabase.LoadAssetAtPath<Sprite>(spritePath));
        }

        //path[0] = Application.dataPath + "/Resources/Prefabs";
        GameObject obj = PrefabUtility.CreatePrefab(path[0] + "/sprite.prefab", go,ReplacePrefabOptions.ConnectToPrefab);
        DestroyImmediate(go);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
