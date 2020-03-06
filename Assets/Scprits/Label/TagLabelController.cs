using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Runing;

/// <summary>
/// 标签控制组件
/// </summary>
public class TagLabelController : MonoBehaviour{
    [Header("标签部件数组")]
    public PartUnit[] tagParts;
    [Header("标签预制体")]
    public GameObject LinePrefab;
    [Header("标签引线粗细")]
    public float lineThickness;
    public  float TagPanelmutiple = 5;
    private Transform[] tagLines;

    private void Awake()
    {
        Tools.CheckAddComponent<SpliteReset>(transform.gameObject);
        tagLines = new Transform[tagParts.Length];
        int i = 0;
        foreach (PartUnit tr in tagParts)
        {
            tagLines[i] = GameObject.Instantiate(LinePrefab).transform;
            tagLines[i].SetParent(tr.tran, false);
            tagLines[i].localScale = Vector3.one*TagPanelmutiple;
            i++;
        }

        //处理子节点重名带空格情况
        Transform[] temps =  Tools.TranGetChild(transform);

        foreach (Transform tr in temps)
        {
            int index = tr.name.IndexOf(' ');
            if (index > 0)
            {
                tr.name = tr.name.Substring(0, index);
            }
        }
    }

    [System.Serializable]
    public class PartUnit
    {
        /// <summary>
        /// 标签开始/结束父节点
        /// </summary>
        public Transform tran;
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Label;
        /// <summary>
        /// 标签开始节点
        /// </summary>
        public Transform start;
        /// <summary>
        /// 标签结束节点
        /// </summary>
        public Transform end;
    }

    void Start () {
        OnStart();
    }

    void OnStart()
    {
        int index = 0;
        foreach (Transform tr in tagLines)
        {
            tr.GetComponent<LineRenderer>().startWidth = lineThickness;
            tr.GetComponent<LineRenderer>().endWidth = lineThickness;
            PanelControl pc = tr.GetComponent<PanelControl>();
            pc.SetTagLabel(tagParts[index].Label);
            pc.followStartPos = tagParts[index].start;
            pc.followEndPos = tagParts[index].end;
            index++;
        }
    }
	
    /// <summary>
    /// 显示隐藏标签
    /// </summary>
    /// <param name="val"></param>
	public void ShowTag(bool val)
    {
        foreach(Transform tr in tagLines)
        {
            tr.gameObject.SetActive(val);
        }
    }

    private void OnDestroy()
    {
    }
}
