using UnityEngine;
using System.Collections;
using System;

namespace MyFrameWork
{
	public class TimerBehaviour : MonoBehaviour
	{
		private uint m_leftSeconds = 0;
		private Action<uint> m_onTimer  = null;
		private Action m_callback = null;
		/// <summary>
		/// 开启定时器
		/// </summary>
		/// <param name="leftSeconds">倒计时秒数</param>
		/// <param name="onTimer">每秒回调,返回参数剩余秒数</param>
		/// <param name="callback">倒计时结束回调</param>
		public void StartTimer(uint leftSeconds, Action<uint> onTimer,Action callback)
		{
			m_leftSeconds = leftSeconds;
			m_onTimer = onTimer;
			m_callback = callback;


			if (m_leftSeconds>0)
			{
				StartTimer();
			}
			else
			{
				StopTimer();
				if (m_callback != null)
				{
					m_callback();
				}
			}
		}

		public uint GetSeconds()
		{
			return m_leftSeconds;
		}

		private bool timerIsActive = false;
		private float timeActive = 0f;

		/// <summary>
		/// 重新记时
		/// </summary>
		private void StartTimer()
		{
			timerIsActive = true;
			timeActive = 0f;

			//第一次回调，看需要
			if (m_onTimer!=null)
			{
				m_onTimer(m_leftSeconds);
			}

		}

		/// <summary>
		/// 停止记时
		/// </summary>
		public void StopTimer()
		{
			timerIsActive = false;
			timeActive = 0f;
		}

		// Update is called once per frame
		void Update()
		{
			if (timerIsActive)
			{
				timeActive += Time.deltaTime * 1000;
//				Debug.LogWarning(timeActive);

				if (timeActive > 1000)
				{
					timeActive -= 1000f;
					m_leftSeconds--;
					if (m_leftSeconds>0)
					{
						//每秒回调
						if (m_onTimer!=null)
						{
							m_onTimer(m_leftSeconds);
						}
					}
					else
					{
						StopTimer();
						if (m_callback != null)
						{
							m_callback();
						}
					}
				}
			}
		}

		public uint GetCurrentTime()
		{
			return m_leftSeconds;
		}

		/// <summary>
		/// 获取剩余时间,格式:00:00
		/// </summary>
		/// <returns>The current time string.</returns>
		public string GetCurrentTimeString()
		{
			return m_leftSeconds.ToString("00.00");
		}
		public bool IsTimerRunning()
		{
			return timerIsActive;
		}

		void OnDisable()
		{
			if (IsTimerRunning())
			{
				StopTimer();
			}
		}

		void OnDestroy()
		{
			m_onTimer  = null;
			m_callback = null;
		}
	}
}
