using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseTimerTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //每隔1秒执行
        MyFrameWork.FrameTimerHeap.AddTimer(0, 1000, () =>
        {
            Common.AppLog.AddMsg(Common.LogLevel.DEBUG,"Exceute Check Screen State.");
        });


        //倒计时
        MyFrameWork.TimerManager.GetTimer(gameObject).StartTimer(5, (t) => {
            Common.AppLog.AddMsg(Common.LogLevel.DEBUG,"Left Seconds: " + t);
        }, () => { Common.AppLog.AddMsg(Common.LogLevel.DEBUG,"End Timer"); });
    }

    // Update is called once per frame
    void Update () {
        MyFrameWork.TimerManager.Instance.Update();
    }
}
