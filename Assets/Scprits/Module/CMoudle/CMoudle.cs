using UnityEngine;
using UnityEngine.UI;

public class CMoudle : MonoBehaviour
{
    Transform leftBtnPar;
    public int LeftToggleNum = 2;
    //public System.Action growAction, distribAction;
    private void Awake()
    {
        Debug.Log("CMoudle");
    }
    // Use this for initialization
    void Start()
    {
        //设置右边物体
        Transform left = Tools.GetScenesObj("UI").transform.Find("InprojectionIgnoreCanvas/Left");
        //leftBtnPar = left.Find("leftBtnPar");
        if (leftBtnPar == null)
        {
            leftBtnPar = new GameObject("leftBtnPar").transform;
            leftBtnPar.SetParent(left, false);
            leftBtnPar.localPosition = new Vector3(29f, 0, 0);
            leftBtnPar.localScale = Vector3.one;
        }
        //加载左侧按钮
        GameObject popu = ResManager.GetPrefab("Prefabs/leftBtn");
        popu.name = "popu";
        popu.transform.SetParent(leftBtnPar);
        popu.transform.localPosition = Vector3.zero;
        popu.transform.localScale = Vector3.one;
        popu.transform.localRotation = Quaternion.Euler(Vector3.zero);

        ToggleGroup tGroup = popu.GetComponent<ToggleGroup>();

        string[] textStrs = new string[] { "C1", "C2" };
        UnityEngine.Events.UnityAction<bool>[] deleArray = new UnityEngine.Events.UnityAction<bool>[] { OnC1Click, OnC2Click };
        for (int i = 0; i < LeftToggleNum; i++)
        {
            GameObject toggle = ResManager.GetPrefab("Prefabs/Toggle");
            toggle.transform.SetParent(popu.transform);
            toggle.transform.localPosition = Vector3.zero;
            toggle.transform.localScale = Vector3.one;
            toggle.transform.localRotation = Quaternion.Euler(Vector3.zero);
            toggle.GetComponentInChildren<Text>().text = textStrs[i];
            Toggle tg = toggle.GetComponentInChildren<Toggle>();
            tg.onValueChanged.RemoveAllListeners();
            tg.onValueChanged.AddListener(deleArray[i]);

            tg.group = tGroup;
        }
    }

    /// <summary>
    /// 人口增长
    /// </summary>
    void OnC1Click(bool isOn)
    {
        if (isOn)
        {
            //人口增长
            GameObject go = ResManager.GetPrefab("Prefabs/C1Moudle");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);

            OnPopuGrowthClickCallback();
        }
    }
    /// <summary>
    /// 按钮点击回调
    /// </summary>
    void OnPopuGrowthClickCallback()
    {

    }
    /// <summary>
    /// 人口分布
    /// </summary>
    void OnC2Click(bool isOn)
    {
        if (isOn)
        {
            //人口分布
            GameObject pg = ResManager.GetPrefab("Prefabs/C2Moudle");
            pg.transform.SetParent(transform);
            pg.transform.localScale = Vector3.one;
            pg.transform.localPosition = Vector3.zero;
            pg.transform.localRotation = Quaternion.Euler(Vector3.zero);
            OnPopuDistributionClickCallback();
        }
    }
    /// <summary>
    /// 人口分布点击回调
    /// </summary>
    void OnPopuDistributionClickCallback()
    {

    }
    /// <summary>
    ///销毁
    /// </summary>
    public void Dispose()
    {
        if (leftBtnPar != null)
        {
            Destroy(leftBtnPar.gameObject);
        }
        if (this.transform.childCount != 0)
        {
            Destroy(this.transform.GetChild(0).gameObject);
        }

    }
    private void OnDestroy()
    {

    }
}
