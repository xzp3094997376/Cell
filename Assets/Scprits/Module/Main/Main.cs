using GCSeries;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    Transform canvasTr;
    //Use this for initialization
    public static Main instance;
    public SpriteCtrl spCtrl;

    public GameObject TextGroup;
    private void Awake()
    {
        instance = this;

        MRSystem mrSys = FindObjectOfType<MRSystem>();
        mrSys.isAutoSlant = false;
        mrSys.ViewerScale = 5;
    }
    void Start()
    {     
        Init();
        //GlobalEntity.GetInstance().RemoveAllListeners(ModelTask.FINISHED);
        //GlobalEntity.GetInstance().AddListener<bool>(ModelTask.FINISHED, OnPopulationToggleClick);
        Debug.Log("主入口程序");
    }
    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        //底部按钮显示
        canvasTr = Tools.GetScenesObj("UI").transform.Find("InprojectionIgnoreCanvas");
        Toggle[] togs = canvasTr.Find("BottomCenter/MiddleUI").GetComponentsInChildren<Toggle>();
        for (int i = 0; i < togs.Length; i++)
        {
            togs[i].onValueChanged.RemoveAllListeners();
            togs[i].gameObject.SetActive(false);
        }
        togs[0].gameObject.SetActive(true);
        togs[1].gameObject.SetActive(true);
        //togs[2].gameObject.SetActive(true);
        togs[0].onValueChanged.AddListener(OnAMoudleToggleClick);
        togs[1].onValueChanged.AddListener(OnBMoudleToggleClick);
        //togs[2].onValueChanged.AddListener(OnCMoudleToggleClick);
        //人口模块默认初始化
        togs[0].isOn = true;
        //OnAMoudleToggleClick(true);
    }
    /// <summary>
    /// 重置当前状态场景
    /// </summary>
    void ResetScene()
    {
        AMoudleFinish();
        BMoudleFinish();
        CMoudleFinish();
    }
    /// <summary>
    ///人口模块
    /// </summary>
    void OnAMoudleToggleClick(bool isOn)
    {
        //加载人口模预制体
        if (isOn)
        {
            //重置场景 
            ResetScene();
            //
            AMoudle ption = ResManager.GetPrefab("Prefabs/AMoudle").GetComponent<AMoudle>();
            ption.name = "AMoudle";
            ption.transform.SetParent(transform);
            ption.transform.localScale = Vector3.one;
            ption.transform.localPosition = Vector3.zero;
        }
    }
    /// <summary>
    /// 处理人口模块结算Destroy
    /// </summary>
    void AMoudleFinish()
    {
        GameObject po = transform.Find("AMoudle")?.gameObject;
        if (po != null)
        {
            po.GetComponent<AMoudle>().Dispose();
            Destroy(po);
        }
    }
    /// <summary>
    /// 人种模块
    /// </summary>
    void OnBMoudleToggleClick(bool isOn)
    {
        if (isOn)
        {
            ResetScene();
            BMoudle race = ResManager.GetPrefab("Prefabs/BMoudle").GetComponent<BMoudle>();
            race.name = "BMoudle";
            race.transform.SetParent(transform);
            race.transform.localScale = Vector3.one;
            race.transform.localPosition = Vector3.zero;
        }
    }
    /// <summary>
    /// 处理状态重置destroy
    /// </summary>
    void BMoudleFinish()
    {
        GameObject race = transform.Find("BMoudle")?.gameObject;
        if (race != null)
        {
            race.GetComponent<BMoudle>().Dispose();
            Destroy(race);
        }
    }
    /// <summary>
    /// 人种模块
    /// </summary>
    void OnCMoudleToggleClick(bool isOn)
    {
        if (isOn)
        {
            ResetScene();

            CMoudle race = ResManager.GetPrefab("Prefabs/CMoudle").GetComponent<CMoudle>();
            race.name = "CMoudle";
            race.transform.SetParent(transform);
            race.transform.localScale = Vector3.one;
            race.transform.localPosition = Vector3.zero;
        }
    }
    /// <summary>
    /// 处理状态重置destroy
    /// </summary>
    void CMoudleFinish()
    {
        GameObject race = transform.Find("CMoudle")?.gameObject;
        if (race != null)
        {
            race.GetComponent<CMoudle>().Dispose();
            Destroy(race);
        }
    }   
    
}
public enum ModelTask
{
    START,
    FINISHED
}

enum HXSTEP
{
    YYHX_1,
    YYHX_2,
    YYHX_3,
    WYHX_4,
    WYHX_5
}
