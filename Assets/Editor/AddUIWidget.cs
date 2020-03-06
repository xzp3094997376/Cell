using UnityEditor;
using UnityEngine;

/// <summary>
/// 给RectTransform组件添加扩展菜单--指定UI添加碰撞体
/// </summary>
public class AddUIWidget
{
    [MenuItem("CONTEXT/RectTransform/添加UI碰撞体")]
    static void AddUIBoxCollider()
    {
        GameObject seleObj = Selection.activeGameObject;
        BoxCollider box = seleObj?.AddComponent<BoxCollider>();
        if (box != null)
        {
            Vector2 size = seleObj.GetComponent<RectTransform>().sizeDelta;
            box.size = new Vector3(size.x, size.y, 0.003f);
        }
    }

    static Vector3 worldPos;
    [MenuItem("CONTEXT/Transform/拷贝世界坐标")]
    static void CopyWorldPos()
    {
        GameObject seleObj = Selection.activeGameObject;
        worldPos = seleObj.transform.position;
    }
    [MenuItem("CONTEXT/Transform/粘贴世界坐标")]
    static void PasteWorldPos()
    {
        GameObject seleObj = Selection.activeGameObject;
        seleObj.transform.position = worldPos;
    }
}
