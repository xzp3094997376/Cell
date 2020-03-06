using UnityEngine;
using MyFrameWork;
public class A1Moudle : MonoBehaviour
{
    TipPanel tip;
    Transform yyhxModelTr;
    ToolBox toolBox;
    // Use this for initialization
    void Start()
    {
        tip = Tools.GetScenesObj("UI").transform.Find("InprojectionIgnoreCanvas/TipPanel").GetComponent<TipPanel>();
        tip.SetText("请选择反应区域");

        //有氧呼吸墨西哥显示
        yyhxModelTr= transform.parent.Find("youYangHuXi");            
        //注册点击事件
        GlobalEntity.GetInstance().RemoveAllListeners();
        GlobalEntity.GetInstance().AddListener<GameObject>(MsgEvent.MOUSE_DOWN,ChooseArea);//选择反应区域
    }

    /// <summary>
    /// 选择反应区域模型移动
    /// </summary>
    /// <param name="go"></param>
    void ChooseArea(GameObject go)
    {
        Vector3 tarPos = Vector3.zero;
        Vector3 eular = Vector3.zero;
        if (go.name.Contains("XianLiTi"))//排除其他box影响
        {
            tip.SetText("选择错误,请重新选择");
        }
        else if (go.name.Contains("XiBaoZiJiZhi"))
        {
            //当前点击事移除
            GlobalEntity.GetInstance().RemoveListener<GameObject>(MsgEvent.MOUSE_DOWN, ChooseArea);

            //模型移动
            tarPos = new Vector3(0,0.97f,0.802f);
            eular = new Vector3(-46.687f,0,0);
            yyhxModelTr.DoTransfrom(1, new TransfromStruct(tarPos,yyhxModelTr.localScale,Quaternion.Euler(eular)), () => {

                //工具箱显示
                toolBox = ResManager.GetPrefab("Prefabs/ToolBox").GetComponent<ToolBox>();
                toolBox.transform.SetParent(transform, false);

                //提示打开工具箱
                tip.SetText("请打开工具箱选择元素");
            });

         
        }

        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public enum MsgEvent
{
    MOUSE_DOWN,
    MOUSE_UP,
    MOUSE,DRAG
}
