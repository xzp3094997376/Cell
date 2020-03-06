/*******************************************************
 * 
 * 文件名(File Name)：             Singleton
 *
 * 作者(Author)：                  Yangzj
 *
 * 创建时间(CreateTime):           2016/02/25 11:33:58
 *
 *******************************************************/

using UnityEngine;
using System.Collections;

namespace MyFrameWork
{
	public class Singleton<T> where T:class,new()
	{
		/// <summary>
		/// The instance.
		/// </summary>
		protected static T _instance = null;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static T Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new T();
				}

				return _instance;
			}
		}

		protected Singleton()
		{
			if(_instance != null)
				throw new System.Exception(string.Format("单例已被实例化过:{0}",typeof(T)));

			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
		}

		/// <summary>
		/// Raises the application quit event.
		/// </summary>
		public void OnApplicationQuit ()
		{
			ReleaseValue();
			OnAppQuit();
			_instance = null;
		}

		protected virtual void OnAppQuit()
		{
		}

		public void ReleaseValue()
		{
			OnReleaseValue();
		}

		protected virtual void OnReleaseValue()
		{
		}

	}
}
