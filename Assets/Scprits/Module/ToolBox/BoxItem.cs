using UnityEngine;
using UnityEngine.UI;

public class BoxItem : MonoBehaviour
{
    Text text;
    Image img, mask;
    public string ItemName;
    //string pName;
    private void Awake()
    {
        img = transform.Find("Image").GetComponent<Image>();
        //img.raycastTarget = false;
        text = GetComponentInChildren<Text>();
        //text.raycastTarget = false;
        mask = transform.Find("mask").GetComponent<Image>();
        mask.raycastTarget = false;

        //pName = Main.instance.transform.GetChild(1).name;
    }

    private void Start()
    {
        UGUIEventListener.Get(transform.Find("Image").gameObject).onPointDown = (GameObject go) =>
        {
            //Debug.LogError("fs");
            img.raycastTarget = false;
            mask.enabled = true;
            mask.color = new Color32(0, 0, 0, 38);
            GlobalEntity.GetInstance().Dispatch(EventEnum.Click, gameObject);
        };

        mask.enabled = false;
        transform.localScale = Vector3.one;
        string textStr = this.name;
        if (this.name == "light")
        {
            textStr = "光";
        }
        else if (this.name == "mei")
        {
            textStr = "酶";
        }
        else if (this.name == "lnt")
        {
            textStr = "色素";
        }
        else if (this.name == "CH2O")
        {
            textStr = "(CH2O)";
        }
        text.text = textStr;
    }

    /// <summary>
    /// 设置图片
    /// </summary>
    /// <param name="sp"></param>
    public void SetSprite(Sprite sp)
    {
        img.sprite = sp;
        ItemName = sp.name;
    }

    /// <summary>
    /// 显示错误信息
    /// </summary>
    public void ShowError()
    {
        mask.gameObject.SetActive(true);
        CancelInvoke("HideError");
        Invoke("HideError", 1);
    }
    void HideError()
    {
        mask.gameObject.SetActive(false);
        img.raycastTarget = true;
    }
    public void ResetAll()
    {
        mask.enabled = false;
    }
}
