using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 材质替换
/// </summary>
public class MaterialControl : MonoBehaviour
{

    public static MaterialControl instance { get; private set; } = null;
    private void Awake()
    {
        instance = this;
    }
    #region   单个物体替换材质的情况
    /// <summary>
    /// 单个物体替换Shader相同的单个材质球
    /// </summary>
    /// <param name="obj">要替换材质球的物体</param>
    /// <param name="replacedMaterial">要替换的材质</param>
    public void ChangeOneObj_Singlesingle(GameObject obj, Material replacedMaterial)
    {
        obj.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(replacedMaterial);
    }
    /// <summary>
    /// 单个物体替换shader不同的单个材质球
    /// </summary>
    /// <param name="obj">要替换材质求的物体</param>
    /// <param name="shadername">要替换的材质的Shader的名字</param>
    /// <param name="replacedMaterial">要替换的材质</param>
    public void ChangeOneObj_Singlesingle(GameObject obj, string shadername, Material replacedMaterial)
    {
        obj.GetComponent<Renderer>().material.shader = Shader.Find(shadername);
        obj.GetComponent<Renderer>().material.CopyPropertiesFromMaterial(replacedMaterial);
    }
    /// <summary>
    /// 单个物体替换Shader相同的多个材质球
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="replacedMaterialGroup">要替换的材质的组合</param>
    public void ChangeOneObj_Singlemany(GameObject obj, Material[] replacedMaterialGroup)
    {
        obj.GetComponent<Renderer>().materials = replacedMaterialGroup;
    }
    /// <summary>
    /// 单个物体替换Shader不同的多个材质球
    /// </summary>
    public void ChangeOneObj_Singlemany(GameObject obj, string shadername, Material[] replacedMaterialGroup)
    {
        for (int i = 0; i < obj.GetComponent<Renderer>().materials.Length; i++)
        {
            obj.GetComponent<Renderer>().materials[i].shader = Shader.Find(shadername);
        }
        obj.GetComponent<Renderer>().materials = replacedMaterialGroup;
    }
    #endregion

    #region 多个物体替换材质的情况
    /// <summary>
    /// 多个物体替换相同Shader的单个材质球
    /// </summary>
    /// <param name="objGroup">要替换材质球的物体的组合</param>
    /// <param name="replacedMaterial">要替换的材质</param>
    public void ChangeManyObj_manySingle(List<GameObject> objGroup, Material replacedMaterial)
    {
        for (int i = 0; i < objGroup.Count; i++)
        {
            objGroup[i].GetComponent<Renderer>().material.CopyPropertiesFromMaterial(replacedMaterial);
        }
    }
    /// <summary>
    /// 多个物体替换不同Shader的单个材质球
    /// </summary>
    /// <param name="objGroup">要替换材质球的物体的组合</param>
    /// <param name="replacedMaterial">要替换的材质</param>
    public void ChangeManyObj_manySingle(List<GameObject> objGroup, Material replacedMaterial, string shadername)
    {
        for (int i = 0; i < objGroup.Count; i++)
        {
            objGroup[i].GetComponent<Renderer>().material.shader = Shader.Find(shadername);
            objGroup[i].GetComponent<Renderer>().material.CopyPropertiesFromMaterial(replacedMaterial);
        }
    }
    /// <summary>
    /// 多个物体替换相同的Shader的多个材质球
    /// </summary>
    /// <param name="objGroup">要替换材质球的物体的组合</param>
    /// <param name="replacedMaterialGroup">要替换的材质组合</param>
    public void ChangeManyObj_manyMany(List<GameObject> objGroup, Material[] replacedMaterialGroup)
    {
        for (int i = 0; i < objGroup.Count; i++)
        {
            objGroup[i].GetComponent<Renderer>().materials = replacedMaterialGroup;
        }
    }

    /// <summary>
    /// 多个物体替换相同的Shader的多个材质球
    /// </summary>
    /// <param name="objGroup">要替换材质球的物体的组合</param>
    /// <param name="replacedMaterialGroup">要替换的材质组合</param>
    public void ChangeManyObj_manyMany(List<GameObject> objGroup, Material[] replacedMaterialGroup, string shadername)
    {

        for (int i = 0; i < objGroup.Count; i++)
        {
            for (int j = 0; j < objGroup[i].GetComponent<Renderer>().materials.Length; j++)
            {
                objGroup[i].GetComponent<Renderer>().materials[j].shader = Shader.Find(shadername);
            }
            objGroup[i].GetComponent<Renderer>().materials = replacedMaterialGroup;
        }
    }
    #endregion


}
