using GCSeries;
using liu;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ToolBox : MonoBehaviour
{
    //GameObject Text;
    Transform panel;
    public Sprite boxNormal, boxClick;
    SpriteCtrl spCtrl;
    List<string> objectsList = new List<string>();
    Dictionary<string, GameObject> objsDic = new Dictionary<string, GameObject>();
    bool selectFinish = false;
    AnimationOper ani;
    bool isPlay = false;
    /// <summary>
    /// 被点击的obj
    /// </summary>
    public GameObject ClickedObj;

    SimpleDrag simpleDrag;
    // Use this for initialization   
    void Start()
    {
        //Text = Tools.GetScenesObj("UI").transform.Find("TextGroup/gjx").gameObject;
        GraphicRaycaster gr = Tools.GetScenesObj("UI").transform.Find("InprojectionIgnoreCanvas").GetComponent<GraphicRaycaster>();

        GCSeriesRaycaster gcr = Tools.GetScenesObj("UI").transform.Find("InprojectionIgnoreCanvas").GetComponent<GCSeriesRaycaster>();

        GraphicRaycaster gr1 = GetComponent<GraphicRaycaster>();
        GCSeriesRaycaster gcr1 = GetComponent<GCSeriesRaycaster>();

        gr1 = gr;
        gcr = gcr1;


        Debug.Log("ToolBox");

        simpleDrag = Tools.GetScenesObj("SimpleDrag").GetComponent<SimpleDrag>();

        objectsList.Clear();

        //初始化工具箱内部物品
        spCtrl = Main.instance.spCtrl;
        Transform content = GetComponentInChildren<ContentSizeFitter>().transform;
        GameObject itemSource = ResManager.GetPrefab("Prefabs/Item");
        itemSource.transform.SetParent(content);
        BoxItem item = itemSource.GetComponent<BoxItem>();
        item.SetSprite(spCtrl[1]);
        item.name = spCtrl[1].name;
        item.transform.localScale = Vector3.one;

        for (int i = 2; i <= spCtrl.Length; i++)
        {
            GameObject obj = Instantiate(itemSource);
            obj.transform.SetParent(content);
            item = obj.GetComponent<BoxItem>();
            item.SetSprite(spCtrl[i]);
            item.name = spCtrl[i].name;
            obj.transform.localScale = Vector3.one;
        }

        //工具箱事件的点击注册
        GameObject boxbtn = transform.Find("Boxbtn").gameObject;
        UGUIEventListener.Get(boxbtn).onPointUp = (GameObject go) =>
        {

            //Text.transform.localScale = Vector3.one;
            Image img = go.GetComponent<Image>();
            if (img.sprite == boxNormal)
            {
                img.sprite = boxClick;
            }
            else
            {
                img.sprite = boxNormal;
                //发送箱子关闭事件
                if (selectFinish) //开始播放
                {
                    PlayAnimation();
                }
            }          

            ShowPanel();//控制工具箱的显示和隐藏
        };
  

        //视图隐藏
        panel = transform.Find("bg");
        panel.gameObject.SetActive(false);

        GlobalEntity.GetInstance().AddListener<GameObject>(EventEnum.Click, ItemClickCallback);
 
        string pName = transform.parent.name;
        string tip = "请打开工具箱选择元素";     
        Tip(tip);

    }

    /// <summary>
    /// 单个模块点击事件
    /// </summary>
    /// <param name="img"></param>
    void ItemClickCallback(GameObject img)
    {
        if (!CheckObj(img.name))
        {
            img.GetComponent<BoxItem>().ShowError();
            return;
        }

        if (objsDic.ContainsKey(img.name))
        {
            ClickedObj = objsDic[img.name];
        }
        else
        {
            ClickedObj = ResManager.GetPrefab("Prefabs/" + img.name);

            ClickedObj.transform.position = img.transform.position;

            objsDic.Add(img.name, ClickedObj);

            if (Monitor23DMode.instance.is3D)
            {
                simpleDrag._curDragObj = ClickedObj;
                FCore.addDragObj(ClickedObj, 1, true);
            }
        }



        ItemModel itm = ClickedObj.GetComponent<ItemModel>();
        if (itm == null)
        {
            itm = ClickedObj.AddComponent<ItemModel>();
        }
        objectsList.Add(img.name);


        Check();
    }

    /// <summary>
    /// 检测光反应或者暗反应元素是否全部选择正确
    /// </summary>
    public void Check()
    {
        bool right = true;
        string pName = transform.parent.name;
        string[] ghzy = new string[] { "light", "H2O", "lnt", "mei" };

        string tip = "光反应尚缺少元素 , 无法进行反应。";
        if (pName == "BMoudle")
        {
            //暗反应
            ghzy = new string[] { "CO2", "C5", "mei" };
            tip = "暗反应尚缺少元素 , 无法进行反应。";
        }

        if (ghzy.Length > objectsList.Count)//缺少物质
        {
            Tip(tip);
            right = false;
        }
        else
        {
            for (int i = 0; i < ghzy.Length; i++)
            {
                int index = objectsList.FindIndex((string str) =>
                {
                    return str == ghzy[i];
                });

                //
                if (index < 0)//选择错误
                {
                    Tip(tip);
                    right = false;
                    break;
                }
            }
        }

        if (right)
        {
            string rightTip = "请关闭工具箱开始光反应";
            if (pName == "BMoudle")
            {
                rightTip = "请关闭工具箱开始暗反应";
            }
            Tip(rightTip);
            selectFinish = true;
        }
    }

    /// <summary>
    /// 判断是光反应还是暗反应
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    bool CheckObj(string _name)
    {
        bool right = true;
        string pName = transform.parent.name;
        List<string> ghzy = new List<string> { "light", "H2O", "lnt", "mei" };

        if (pName == "BMoudle")
        {
            ghzy = new List<string> { "CO2", "C5", "mei" };
        }

        if (!ghzy.Contains(_name))
        {
            right = false;
        }
        return right;
    }
    /// <summary>
    /// 显示提示信息s
    /// </summary>
    /// <param name="msg"></param>
    void Tip(string msg)
    {
        Text tip;
        if (transform.parent.name == "AMoudle")
        {
            tip = Main.instance.TextGroup.transform.Find("GFY").GetChild(2).GetComponentInChildren<Text>();
        }
        else
        {
            tip = Main.instance.TextGroup.transform.Find("AFY").GetChild(2).GetComponentInChildren<Text>();
        }
        tip.text = msg;
    }


    /// <summary>
    /// 播放动画控制    
    /// </summary>
    void PlayAnimation()
    {     
        //工具箱隐藏
        gameObject.SetActive(false);      

        //动画模型显示
        Transform aniPar = transform.parent.GetChild(1);
        for (int i = 0; i < aniPar.childCount; i++)
        {
            aniPar.GetChild(i).gameObject.SetActive(true);
        }

        //提示隐藏//708动画帧数
        int point = 708;
        Transform aniTr = aniPar.transform.parent.Find("01");
        if (transform.parent.name == "BMoudle")
        {
            point = 578;
            aniTr = aniPar.transform.parent.Find("02");
            aniTr.Find("sw_swjj_H").gameObject.SetActive(false);
        }

        ani = aniTr.GetComponent<AnimationOper>();
        ani.timePointEvent = (a) =>
        {
            //Debug.Log(a + "---");
            if (a >= point - 1 && a <= point + 1 && !isPlay)
            {
                isPlay = true;
                ani.OnPause();
                CancelInvoke("PlayAgain");
                Invoke("PlayAgain", 1);
            }
        };
        ani.OnContinue();
        ani.PlayForward("Take 001");

    }

    void PlayAgain()
    {
        isPlay = false;
        ani.PlayForward("Take 001", 0);
        ani.OnContinue();
    }
    /// <summary>
    /// 显示ScrollView滚动面板
    /// </summary>
    public void ShowPanel()
    {
        bool show = panel.gameObject.activeSelf;
        panel.gameObject.SetActive(!show);
    }


    private void OnDestroy()
    {
        if (ani != null)
        {
            ani.timePointEvent = null;
        }

        GlobalEntity.GetInstance().RemoveListener<GameObject>(EventEnum.Click, ItemClickCallback);
        transform.Find("Boxbtn").GetComponent<UGUIEventListener>().RemoveAllListeners();
        //Text.transform.localScale = Vector3.zero;
    }
}
public enum EventEnum
{
    Click,//点击事件
}
