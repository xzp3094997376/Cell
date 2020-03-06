using UnityEngine;

public class AnimatorEventComponent : MonoBehaviour
{

    /// <summary>
    /// 添加帧事件(从所有动画片段层级中找)
    /// </summary>
    /// <param name="clipName">动画片段名称</param>
    /// <param name="frame">第几帧</param>
    /// <param name="action">回调</param>
    public void AddEvent(string clipName, int frame, System.Action action)
    {
        ParaObj para = ScriptableObject.CreateInstance<ParaObj>();
        para.action = action;

        Animator ani = GetComponent<Animator>();
        AnimationClip ac = System.Array.Find<AnimationClip>(ani.runtimeAnimatorController.animationClips, (AnimationClip _clip) =>
           {
               AnimationClip _ac = null;
               if (_clip.name == clipName)
               {
                   _ac = _clip;
               }
               return _ac;
           });
        //AnimationClip clip = ani.GetClip(clipName);

        AnimationEvent ae = new AnimationEvent();
        ae.time = frame / ac.frameRate;
        ae.functionName = "Callback";
        ae.objectReferenceParameter = para;
        ac.AddEvent(ae);
    }

    /// <summary>
    /// 添加帧事件(从某一层动画片段找)
    /// </summary>
    /// <param name="clipName">动画片段名称</param>
    /// <param name="frame">第几帧</param>
    /// <param name="action">回调</param>
    public void AddEvent(int layer, string clipName, int frame, System.Action action)
    {
        ParaObj para = ScriptableObject.CreateInstance<ParaObj>();
        para.action = action;

        Animator ani = GetComponent<Animator>();
        AnimatorClipInfo[] clipInfors = ani.GetCurrentAnimatorClipInfo(layer);
        AnimationClip ac = null;
        for (int i = 0; i < clipInfors.Length; i++)
        {
            if (clipInfors[i].clip.name == clipName)
            {
                ac = clipInfors[i].clip;
                break;
            }
        }

        AnimationEvent ae = new AnimationEvent();
        ae.time = frame / ac.frameRate;
        ae.functionName = "Callback";
        ae.objectReferenceParameter = para;
        ac.AddEvent(ae);
    }

    void Callback(Object obj)
    {
        // Debug.Log("animation event call  " + obj.GetType() + "   " + obj.name);
        (obj as ParaObj).action();
        //(obj as ParaObj).action = null;
    }
}
