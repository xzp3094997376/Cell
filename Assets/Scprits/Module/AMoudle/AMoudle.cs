using UnityEngine;
using UnityEngine.UI;

public class AMoudle : MonoBehaviour
{
    Transform leftBtnPar;
    public int LeftToggleNum = 3;
    GameObject yyhxModel;
    private void Awake()
    {
        Debug.Log("AMoudle");
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

        //设置toggleGroup
        ToggleGroup tGroup = popu.GetComponent<ToggleGroup>();

        string[] textStrs = new string[] { "第一阶段", "第二阶段" ,"第三阶段"};
        UnityEngine.Events.UnityAction<bool>[] deleArray = new UnityEngine.Events.UnityAction<bool>[] { OnA1Click, OnA2Click,OnA3Click };
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

            tg.group = tGroup;//设置toggleGroup
        }    

        //加载模型      
        yyhxModel = ResManager.GetPrefab("Prefabs/youYangHuXi");
        yyhxModel.transform.SetParent(transform, false);

        // 设置标签的偏移量
        PanelControl[] pCtrls= yyhxModel.transform.GetComponentsInChildren<PanelControl>();
        float[] offsetY = new float[] { 0.07f,0.07f};
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
    void OnA1Click(bool isOn)
    {
        if (isOn)
        {
            CallbackFinish();
            //人口增长
            GameObject go = ResManager.GetPrefab("Prefabs/A1Moudle");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);

            //OnA1MoudleClickCallback();        
            ShowMoudle(0);
        }
    }
    /// <summary>
    /// 按钮点击回调
    /// </summary>
    void OnA1MoudleClickCallback()
    {
        Transform a1moudle=transform.Find("A1Moudle");
        if (a1moudle!= null)
        {
            Destroy(a1moudle.gameObject);
        }
    }
    /// <summary>
    /// 人口分布
    /// </summary>
    void OnA2Click(bool isOn)
    {
        if (isOn)
        {
            CallbackFinish();
            //人口分布
            GameObject pg = ResManager.GetPrefab("Prefabs/A2Moudle");
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
    void OnA2MoudleClickCallback()
    {
        Transform a2moudle = transform.Find("A2Moudle");
        if (a2moudle != null)
        {
            Destroy(a2moudle.gameObject);
        }
    }
    void OnA3Click(bool isOn)
    {
        if (isOn)
        {
            CallbackFinish();
            //人口分布
            GameObject pg = ResManager.GetPrefab("Prefabs/A3Moudle");
            pg.transform.SetParent(transform);
            pg.transform.localScale = Vector3.one;
            pg.transform.localPosition = Vector3.zero;
            pg.transform.localRotation = Quaternion.Euler(Vector3.zero);

            ShowMoudle(2);
        }
    }
    /// <summary>
    /// 人口分布点击回调
    /// </summary>
    void OnA3MoudleClickCallback()
    {
        Transform a3moudle = transform.Find("A3Moudle");
        if (a3moudle != null)
        {
            Destroy(a3moudle.gameObject);
        }
    }

    /// <summary>
    /// 显示有氧呼吸某个阶段的模型
    /// </summary>
    void ShowMoudle(int id)
    {
        foreach (Transform item in yyhxModel.transform)
        {
            if (item.name.Contains("tag"))//略过标签
            {
                continue;
            }
            item.gameObject.SetActive(false);   
        }
        yyhxModel.transform.GetChild(id).gameObject.SetActive(true);
    }
    
    void CallbackFinish()
    {
        OnA1MoudleClickCallback();
        OnA2MoudleClickCallback();
        OnA3MoudleClickCallback();
    }

    void SetModelShow(HXSTEP step)
    {
        transform.parent.Find("");
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
