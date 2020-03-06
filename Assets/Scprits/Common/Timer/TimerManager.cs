using UnityEngine;
using System.Collections;

namespace MyFrameWork
{
	public class TimerManager : Singleton<TimerManager>
	{
		public static TimerBehaviour GetTimer(GameObject target)
		{
			return target.GetOrAddComponent<TimerBehaviour>();
		}

		public void Update()
		{
			FrameTimerHeap.Tick();
		}

		protected override void OnReleaseValue()
		{
			FrameTimerHeap.ReleaseVal();
		}

		protected override void OnAppQuit()
		{
			OnReleaseValue();
		}
	}
}