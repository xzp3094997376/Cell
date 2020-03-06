using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventComponent : MonoBehaviour
{
    Animator ani;
    string clipName;
    private void Awake()
    {
        ani = GetComponent<Animator>();
    }
    /// <summary>
    /// 播放某一个动画
    /// </summary>
    public void PlayForward(string _clipName, int offset = 0, float speed = 1)
    {
        gameObject.SetActive(true);
        ani.speed = speed;
        clipName = _clipName;
        float timeLength = 0;
        float timePoint = 0;
        //速率
        System.Array.FindIndex(ani.runtimeAnimatorController.animationClips, (ac) =>
        {
            //Debug.LogError(ac.name+"   "+clipName+"   "+offset);
            if (ac.name == clipName)
            {
                timeLength = ac.length;
                timePoint= offset/ (timeLength*ac.frameRate);
                return true;
            }
            return false;
        });
       // Debug.LogError(timePoint+" "+clipName);
        ani.Play(clipName,0,timePoint);             
    }
    /// <summary>
    /// 添加帧事件(从所有动画片段层级中找)
    /// </summary>
    /// <param name="clipName">动画片段名称</param>
    /// <param name="frame">第几帧</param>
    /// <param name="action">回调</param>
    public void AddEvent(string clipName, int frame, System.Action action)
    {
        bool hasAdd = HasAddEvent(clipName, frame);
        if (hasAdd)
        {
            return;
        }


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


     AnimationClip GetClip(string clip)
    {
        AnimationClip[] clips = ani.runtimeAnimatorController.animationClips;
        AnimationClip ac = null;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == clip)
            {
                ac = clips[i];
                break;
            }
        }
        return ac;
    }
    /// <summary>
    /// 添加帧事件(从某一层动画片段找)
    /// </summary>
    /// <param name="clipName">动画片段名称</param>
    /// <param name="frame">第几帧</param>
    /// <param name="action">回调</param>
    public void AddEvent(int layer, string clipName, int frame, System.Action action)
    {
        bool hasAdd= HasAddEvent(clipName, frame);
        if (hasAdd)
        {
            return;
        }

        ParaObj para = ScriptableObject.CreateInstance<ParaObj>();//可以优化
        para.action = action;

        AnimationClip ac= GetClip(clipName);
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

    /// <summary>
    /// 判断是否某一帧已经有帧事件,只添加一个
    /// </summary>
    /// <returns></returns>
    bool HasAddEvent(string clipName,int frame)
    {
        bool hasAdd = false;
        AnimationClip clip = GetClip(clipName);
        float time = frame / clip.frameRate;
        for (int i = 0; i < clip.events.Length; i++)
        {
            if (clip.events[i].time == time)
            {
                hasAdd = true;
                break;
            }
        }
        return hasAdd;
    }
    /// <summary>
    /// 移除某一帧某个事件
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="frame"></param>
    public void ClearSpecialEvents(string clipName, int frame)
    {       
        //Animation ani = GetComponent<Animation>();
        AnimationClip clip = GetClip(clipName);
        //Debug.Log("事件个数： " + clip.events.Length);
        float time = frame /clip.frameRate;
        for (int i = 0; i < clip.events.Length; i++)
        {
            if (clip.events[i].time == time)
            {
                ArrayList arr = new ArrayList(clip.events);
                arr.RemoveAt(i);
                clip.events=(AnimationEvent[])arr.ToArray(typeof(AnimationEvent));
                //Debug.Log("事件个数(移除后)： " + clip.events.Length);
                break;
            }
        }
    }

    /// <summary>
    /// 移除某一个动画的所有帧事件
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="frame"></param>
    public void ClearAllEvents(string clipName)
    {        
        AnimationClip[] acp= ani.runtimeAnimatorController.animationClips;
        for (int i = 0; i < acp.Length; i++)
        {
            for (int j = 0; j < acp[i].events.Length; j++)
            {
                acp[i].events[j] = null;
            }
            acp[i].events = null;
        }    
    }


    /// <summary>
    /// 停止播放
    /// </summary>
    /// <param name="clipName"></param>
    public void OnPause()
    {
        //Animation ani = GetComponent<Animation>();            
       ani.speed = 0;
    }

    /// <summary>
    /// /继续播放
    /// </summary>
    /// <param name="clipName"></param>
    public void OnContinue()
    {
        ani.speed = 1;
    }
}
