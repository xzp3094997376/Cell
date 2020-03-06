using DG.Tweening;
using GCSeries;
using liu;
using System;
using System.Collections.Generic;
using UnityEngine;
public static class Tools
{
    static Dictionary<string, GameObject> sceneObjDic = new Dictionary<string, GameObject>();
    /// <summary>
    /// 得到场景中的根目录下的所有物体
    /// </summary>
    /// <param name="name"></param>
    public static GameObject GetScenesObj(string name)
    {
        GameObject go = null;
        if (sceneObjDic.Count > 0)
        {
            if (sceneObjDic.ContainsKey(name))
            {
                go = sceneObjDic[name];
                return go;
            }
        }

        GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < rootObjs.Length; i++)
        {
            string _name = rootObjs[i].name;
            if (_name == name)
            {
                go = rootObjs[i];
            }
            if (!sceneObjDic.ContainsKey(_name))
            {
                sceneObjDic.Add(_name, rootObjs[i]);
            }
        }
        return go;
    }

    /// <summary>
    /// 游戏对象添加组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T CheckAddComponent<T>(this GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }

    /// <summary>
    /// 获得子Transform节点列表
    /// </summary>
    /// <param name="tran"></param>
    /// <returns></returns>
    public static Transform[] TranGetChild(Transform tran)
    {
        Queue<Transform> result = new Queue<Transform>();
        Queue<Transform> queue = new Queue<Transform>();//BFS队列
        queue.Enqueue(tran);

        while (queue.Count > 0)
        {
            Transform front = queue.Dequeue();
            result.Enqueue(front);

            for (int i = 0; i < front.childCount; i++)
            {
                queue.Enqueue(front.GetChild(i));
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// 得到3d标签预制
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="canvasScale"></param>
    /// <returns></returns>
    public static PanelControl GetLine(Vector3 start, Vector3 end, float canvasScale = 0.001f)
    {
        PanelControl lineCtrl = ResManager.GetPrefab("SceneRes/Line").GetComponent<PanelControl>();
        lineCtrl.followStartPos.transform.localPosition = start;
        lineCtrl.followEndPos.transform.localPosition = end;
        lineCtrl.GetComponentInChildren<Canvas>().transform.localScale = Vector3.one * canvasScale;
        return lineCtrl;
    }
    /// <summary>
    /// 延迟调用
    /// </summary>
    /// <typeparam name="T">泛型参数类型/typeparam>
    /// <param name="delayTime">延迟时间</param>
    /// <param name="t">Action 参数</param>
    /// <param name="action">方法</param>
    public static void DotweenInvoke<T>(float delayTime, T t, System.Action<T> action)
    {
        DOTween.To(
        () => { return 0; },//第一个执行方法
        (a) => { },//update
        0,
        delayTime).onComplete = () =>
        {
            action?.Invoke(t);
            //action()
        };
    }

    /// <summary>
    /// 相机dotween位移
    /// </summary>
    /// <typeparam name="T">回调函数参数类型</typeparam>
    /// <param name="tarPos">tween目标位置</param>
    /// <param name="delayTime">tween时间</param>
    /// <param name="para">tween完成回调函数参数</param>
    /// <param name="Complete">tween完成回调函数</param>
    public static void DoTweenCamera<T>(Vector3 tarPos, float delayTime, T para, System.Action<T> Complete)
    {
        if (Monitor23DMode.instance.is3D)//3d相机推进
        {
            Camera3D camera3D = GetScenesObj("Camera3D").GetComponent<Camera3D>();
            camera3D.enabled = false;

            camera3D.transform.DOLocalMove(tarPos, delayTime).onComplete = () =>
            {
                Complete?.Invoke(para);
                camera3D.enabled = true;
            };
        }
        else//2d相近推进
        {
            MRSystem mrSys = GetScenesObj("MRSystem").GetComponent<MRSystem>();
            mrSys.transform.DOLocalMove(tarPos, delayTime).onComplete = () =>
            {
                Complete?.Invoke(para);
            };
        }
    }


    /// <summary>
    /// 相机推进：模型dotween位移
    /// </summary>
    /// <param name="obj">tween目标位置</param>
    /// <param name="tarPos">tween目标位置</param>
    /// <param name="delayTime">tween时间</param>
    /// <param name="Complete">tween完成回调函数</param>
    /// <param name="isWorld">是否世界坐标</param>
    public static void DoTweenModel(GameObject obj, Vector3 tarPos, float delayTime, System.Action Complete, bool isWorld)
    {
        if (isWorld)
        {
            obj.transform.DOMove(tarPos, delayTime).onComplete = () =>
            {
                Complete?.Invoke();
            };
        }
        else
        {
            obj.transform.DOLocalMove(tarPos, delayTime).onComplete = () =>
            {
                Complete?.Invoke();
            };
        }
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public static string GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        string str = Convert.ToInt64(ts.TotalMilliseconds).ToString();
        return str;
    }

    public static string NewGUID()
    {
        return BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0).ToString();
    }
}
