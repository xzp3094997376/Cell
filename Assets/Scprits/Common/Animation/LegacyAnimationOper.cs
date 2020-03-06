using UnityEngine;

/// <summary>
/// 动画需要改成旧版动画
/// </summary>
public class LegacyAnimationOper : MonoBehaviour
{
    public Animation anim;
    public string animName;
    void Awake()
    {
        anim = GetComponent<Animation>();
        IsStart = false;
        IsComplete = false;
        anim.playAutomatically = false;

        //framePointEvent = (a) =>
        //{
        //    Debug.Log(a);
        //};
        //PlayForward("jiujingdengguanbi_quan", 0);
        //anim.enabled = true;
    }

    private void Start()
    {


    }

    public WrapMode SetWrapMode
    {
        get
        {
            return anim.wrapMode;
        }
        set
        {
            anim.wrapMode = value;
        }
    }
    /// <summary>
    /// 当画是否开始
    /// </summary>
    public bool IsStart
    {
        get;
        set;
    }

    /// <summary>
    /// 动画是否完成
    /// </summary>
    public bool IsComplete
    {
        get;
        set;
    }

    public System.Action Complete;
    public System.Action<float> timePointEvent; //时间点事件,参数为当前时间
    public System.Action<int> framePointEvent; //帧事件,参数为当前帧数
    float timeLength;
    float currLength;
    int lastFrame = -1, curFrame = -1;
    AnimationState state;
    public float transitionTime = 0f;//过渡时间-   
    /// <summary>
    /// 从头开始播放动画剪辑
    /// </summary>
    /// <param name="clipName"></param> 
    public void PlayForward(string clipName, float normalizeTime = 0)
    {
        if (anim)
        {
            animName = clipName;
            //if (clipName == "MM_E_3RE_DY_KA")
            //{
            //    Debug.Log(anim.IsPlaying(clipName) + "   " + anim[clipName].normalizedTime);
            //}
            anim.Stop();
            anim.Rewind();
            if (!anim.IsPlaying(clipName))
            {
                state = anim[clipName];
                state.normalizedTime = normalizeTime;
                //anim[clipName].time = 0;
                lastFrame = curFrame = -1;
            }
            //anim.Play(clipName, 0, 0);
            timeLength = anim[clipName].length;
            IsStart = true;
            currLength = 0;
            anim[animName].speed = 1;
            anim.Play(clipName, PlayMode.StopAll);
        }
        else
        {
            Debug.Log("没有找到动画  " + clipName);
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void OnPause()
    {
        IsStart = false;
        if (anim != null)
        {
            anim[animName].speed = 0;
        }
    }

    /// <summary>
    /// 继续
    /// </summary>
    public void OnContinue()
    {
        IsStart = true;
        if (anim != null)
        {
            anim[animName].speed = 1;
        }

    }

    void Update()
    {
        //Debug.Log(Time.time);
        if (IsStart)
        {
            if (state?.name == animName)
            {
                timeLength = anim[animName].length;
                if (currLength <= timeLength)
                {
                    //Debug.Log(currLength + "   curlength" + timeLength);
                    if (timePointEvent != null)
                    {
                        timePointEvent(currLength);
                    }

                    if (framePointEvent != null)
                    {
                        float cur = anim[animName].clip.frameRate * anim[animName].length * anim[animName].normalizedTime;
                        //Debug.Log(anim[animName].normalizedTime);
                        //Debug.Log(Mathf.RoundToInt(curFrame));
                        curFrame = Mathf.RoundToInt(cur);
                        if (lastFrame != curFrame)
                        {
                            //Debug.Log(Mathf.RoundToInt(curFrame) + "----------------");
                            framePointEvent(curFrame);
                        }
                        lastFrame = curFrame;
                    }
                    currLength += Time.deltaTime;
                }
                else
                {
                    //Debug.LogError("complete  " + animName + "     " + currLength + "     " + Complete == null);
                    IsStart = false;
                    IsComplete = true;
                    currLength = 0;
                    lastFrame = curFrame = -1;
                    Complete?.Invoke();//回调中手动置为NULL
                    //Complete = null;
                    timePointEvent = null;
                }
            }
        }
    }
    private void OnDisable()
    {
        anim?.Rewind(animName);
    }
    void OnDestroy()
    {
        IsStart = false;
        IsComplete = false;
        Complete = null;
        timePointEvent = null;
        framePointEvent = null;
    }
}
