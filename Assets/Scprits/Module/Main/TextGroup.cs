using UnityEngine;

public class TextGroup : MonoBehaviour
{
    Transform par;
    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 选择所需元素点击提示
    /// </summary>
    public void TipForChoose()
    {
        ResetObj(1);
        par.GetChild(2).gameObject.SetActive(true);
    }
    /// <summary>
    /// 错误点击提示
    /// </summary>
    public void TipForError(int num = 1)
    {
        ResetObj(num);
        par.GetChild(1).gameObject.SetActive(true);
    }
    /// <summary>
    /// 光反应和暗反应点击区域提示
    /// </summary>
    public void TipForClickArea()
    {
        ResetObj();
        par.GetChild(0).gameObject.SetActive(true);
    }
    /// <summary>
    /// 重置所有
    /// </summary>
    public void ResetAll(FYEnum fy)
    {
        par = transform.Find(fy.ToString("g"));
    }
    public void ResetObj(int num = 0)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < par.childCount - num; i++)
        {
            par.GetChild(i).gameObject.SetActive(false);
        }
        par.gameObject.SetActive(true);
        //gameObject.SetActive(false);
    }

    public void ResetAll()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        //Debug.LogError(transform.childCount);
    }


    private void OnDisable()
    {
        //Debug.LogError("disba");
    }
}
public enum FYEnum
{
    GFY,//有氧呼吸
    AFY,//无氧呼吸
    YXYS//没有反应
}
