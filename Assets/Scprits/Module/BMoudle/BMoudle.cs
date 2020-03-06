using UnityEngine;
using UnityEngine.UI;

public class BMoudle : MonoBehaviour
{
    Transform leftBtnPar;
    public int LeftToggleNum = 2;
    GameObject wyhxModel;
    private void Awake()
    {

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

        string[] textStrs = new string[] { "第一阶段", "第二阶段" };
        UnityEngine.Events.UnityAction<bool>[] deleArray = new UnityEngine.Events.UnityAction<bool>[] { OnB1Click, OnB2Click };
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

        //加载模型      
        wyhxModel = ResManager.GetPrefab("Prefabs/wuYangHuXi");
        wyhxModel.transform.SetParent(transform, false);

        // 设置标签的偏移量
        PanelControl[] pCtrls = wyhxModel.transform.GetComponentsInChildren<PanelControl>();
        float[] offsetY = new float[] { 0.07f, 0.07f };
        for (int i = 0; i < offsetY.Length; i++)
        {
            pCtrls[i].offSetY = offsetY[i];
        }

        Toggle tg0 = popu.GetComponentInChildren<Toggle>();
        tg0.isOn = true;
    }

    /// <summary>
    /// 人口增长
    /// </summary>
    void OnB1Click(bool isOn)
    {
        if (isOn)
        {
            CallbackFinish();
            //人口增长
            GameObject go = ResManager.GetPrefab("Prefabs/B1Moudle");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);

            ShowMoudle(0);
        }
    }
    /// <summary>
    /// 按钮点击回调
    /// </summary>
    void OnPopuGrowthClickCallback()
    {
        Transform b1moudle = transform.Find("B1Moudle");
        if (b1moudle != null)
        {
            Destroy(b1moudle.gameObject);
        }
    }
    /// <summary>
    /// 人口分布
    /// </summary>
    void OnB2Click(bool isOn)
    {
        if (isOn)
        {
            CallbackFinish();
            //人口分布
            GameObject pg = ResManager.GetPrefab("Prefabs/B2Moudle");
            pg.transform.SetParent(transform);
            pg.transform.localScale = Vector3.one;
            pg.transform.localPosition = Vector3.zero;
            pg.transform.localRotation = Quaternion.Euler(Vector3.zero);

            ShowMoudle(1);
        }
    }
    /// <summary>
    /// 人口分布点击回调
    /// </summary>
    void OnPopuDistributionClickCallback()
    {
        Transform b2moudle = transform.Find("B2Moudle");
        if (b2moudle != null)
        {
            Destroy(b2moudle.gameObject);
        }
    }
    void CallbackFinish()
    {
        OnPopuGrowthClickCallback();
        OnPopuDistributionClickCallback();      
    }

    /// <summary>
    /// 显示模型
    /// </summary>
    /// <param name="id"></param>
    void ShowMoudle(int id)
    {
        foreach (Transform item in wyhxModel.transform)
        {
            if (item.name.Contains("tag"))//略过标签
            {
                continue;
            }
            item.gameObject.SetActive(false);
        }
        wyhxModel.transform.GetChild(id).gameObject.SetActive(true);
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


