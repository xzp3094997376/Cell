using UnityEngine;

namespace MyFrameWork
{
    public static class MethodExtension
    {

        #region GetOrAddComponent
        /// <summary>
        /// 获取游戏对象上的组件，如果不存在新建组件
        /// </summary>
        /// <returns>The or add component.</returns>
        /// <param name="go">Go.</param>
        /// <param name="path">在子游戏对象上添加组件，需要填写相对路径</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetOrAddComponent<T>(this GameObject go, string path = "") where T : Component
        {
            Transform t;
            if (string.IsNullOrEmpty(path))
                t = go.transform;
            else
                t = go.transform.Find(path);

            if (null == t)
            {
                Debug.LogError("GetOrAddComponent not Find GameObject at Path: " + path);
                return null;
            }

            T ret = t.GetComponent<T>();
            if (null == ret)
                ret = t.gameObject.AddComponent<T>();
            return ret;
        }
        /// <summary>
        /// 获取游戏对象上的组件，如果不存在新建组件
        /// </summary>
        /// <returns>The or add component.</returns>
        /// <param name="t">T.</param>
        /// <param name="path">在子游戏对象上添加组件，需要填写相对路径</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetOrAddComponent<T>(this Transform t, string path = "") where T : Component
        {
            return t.gameObject.GetOrAddComponent<T>(path);
        }

        /// <summary>
        /// 获取游戏对象上的组件，如果不存在新建组件
        /// </summary>
        /// <returns>The or add component.</returns>
        /// <param name="mono">Mono.</param>
        /// <param name="path">在子游戏对象上添加组件，需要填写相对路径</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetOrAddComponent<T>(this MonoBehaviour mono, string path = "") where T : Component
        {
            return mono.gameObject.GetOrAddComponent<T>(path);
        }

        #endregion

        #region GetComponentByPath
        /// <summary>
        /// 获取游戏对象上的组件，没有返回null
        /// </summary>
        /// <returns>The component by path.</returns>
        /// <param name="transform">Transform.</param>
        /// <param name="path">Path.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetComponentByPath<T>(this Transform transform, string path) where T : Component
        {
            Transform t = transform.Find(path);
            if (null == t)
            {
                Debug.LogError("GetComponentByPath not Find GameObject at Path: " + path);
                return null;
            }
            T ret = t.GetComponent<T>();
            if (null == ret)
                Debug.LogError("GetComponentByPath not Find [ " + typeof(T).ToString() + " ] Component at Path: " + path);
            return ret;
        }
        /// <summary>
        /// 获取游戏对象上的组件，没有返回null
        /// </summary>
        /// <returns>The component by path.</returns>
        /// <param name="mono">Mono.</param>
        /// <param name="path">Path.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetComponentByPath<T>(this MonoBehaviour mono, string path) where T : Component
        {
            return mono.transform.GetComponentByPath<T>(path);
        }
        /// <summary>
        /// 获取游戏对象上的组件，没有返回null
        /// </summary>
        /// <returns>The component by path.</returns>
        /// <param name="go">Go.</param>
        /// <param name="path">Path.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetComponentByPath<T>(this GameObject go, string path) where T : Component
        {
            return go.transform.GetComponentByPath<T>(path);
        }


        #endregion
    }
}
