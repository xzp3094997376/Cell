using UnityEngine;

public class AnimationEventComponent : MonoBehaviour
{

    /// <summary>
    /// 添加帧事件
    /// </summary>
    /// <param name="clipName">动画片段名称</param>
    /// <param name="frame">第几帧</param>
    /// <param name="action">回调</param>
    public void AddEvent(string clipName, int frame, System.Action action)
    {
        ParaObj para = ScriptableObject.CreateInstance<ParaObj>();
        para.action = action;

        Animation ani = GetComponent<Animation>();
        AnimationClip clip = ani.GetClip(clipName);

        AnimationEvent ae = new AnimationEvent();
        ae.time = frame / ani.clip.frameRate;
        ae.functionName = "Callback";
        ae.objectReferenceParameter = para;
        clip.AddEvent(ae);
    }

    void Callback(Object obj)
    {
        // Debug.Log("animation event call  " + obj.GetType() + "   " + obj.name);
        (obj as ParaObj).action();
        (obj as ParaObj).action = null;
    }
}
