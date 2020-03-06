using System.Collections;
using UnityEngine;

public class AnimationEventComponent : MonoBehaviour
{
    Animation ani;
    string clipName;
    private void Start()
    {
        ani = GetComponent<Animation>();
    }
    /// <summary>
    /// 播放某一个动画
    /// </summary>
    public void PlayForward(string _clipName,int offset=0,float speed=1)
    {
        clipName = _clipName;
        //速率
        AnimationState state=ani[clipName];
        state.speed = 1;

        //偏移量
        AnimationClip clip = ani.GetClip(clipName);
        state.normalizedTime = offset / clip.frameRate;

        ani.Play(clipName);   
    }
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

        //Animation ani = GetComponent<Animation>();
        AnimationClip clip = ani.GetClip(clipName);

        AnimationEvent ae = new AnimationEvent();
        ae.time = frame /clip.frameRate;
        ae.functionName = "Callback";
        ae.objectReferenceParameter = para;
        clip.AddEvent(ae);
    }


    void Callback(Object obj)
    {
        // Debug.Log("animation event call  " + obj.GetType() + "   " + obj.name);
        (obj as ParaObj).action();
        //(obj as ParaObj).action = null;
    }

    /// <summary>
    /// 移除某一帧某个事件
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="frame"></param>
    public void ClearSpecialEvents(string clipName,int frame)
    {
        //Animation ani = GetComponent<Animation>();
        AnimationClip clip = ani.GetClip(clipName);
        float time = frame / ani.clip.frameRate;
        for (int i = 0; i <clip.events.Length; i++)
        {
            if (clip.events[i].time==time)
            {
                ArrayList arr = new ArrayList(clip.events);
                arr.RemoveAt(i);
                clip.events = (AnimationEvent[])arr.ToArray(typeof(AnimationEvent));
            }
        }
    }

    /// <summary>
    /// 移除某一个动画的所有帧事件
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="frame"></param>
    public void ClearAllEvents(string clipName, int frame)
    {
        //Animation ani = GetComponent<Animation>();
        AnimationClip clip = ani.GetClip(clipName);
        clip.events = null;
    }


    /// <summary>
    /// 停止播放
    /// </summary>
    /// <param name="clipName"></param>
    public void OnPause(string clipName)
    {
        //Animation ani = GetComponent<Animation>();            
        ani[clipName].speed = 0;
    }

    /// <summary>
    /// /继续播放
    /// </summary>
    /// <param name="clipName"></param>
    public void OnContinue(string clipName)
    {
        Animation ani = GetComponent<Animation>();
        ani[clipName].speed = 1;
    }
}

