using DG.Tweening;
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
        public static Transform SetTransfrom(this Transform t, TransfromStruct ts)
        {
            t.transform.localPosition = ts.pos;
            t.transform.localRotation = ts.rotation;
            t.transform.localScale = ts.scale;
            return t.transform;
        }
        public static void DoTransfrom(this Transform tt, float time, TransfromStruct tarTransfromInfor, System.Action action)
        {
            tt.DOLocalMove(tarTransfromInfor.pos, time).onComplete = () =>
            {
                action?.Invoke();
                action = null;
            }; ;
            tt.DORotateQuaternion(tarTransfromInfor.rotation, time);
            tt.DOScale(tarTransfromInfor.scale, time);
        }
        public static GameObject InitObj(this GameObject go, Transform par, Vector3 pos, Vector3 eular, Vector3 scale)
        {
            go.transform.SetParent(par);
            go.transform.localPosition = pos;
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);
            go.transform.localScale = Vector3.one;
            return go;
        }

    }
}
public struct TransfromStruct
{
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rotation;
    public TransfromStruct(Vector3 _pos, Vector3 _scale, Quaternion _rotation)
    {
        pos = _pos;
        scale = _scale;
        rotation = _rotation;
    }

    public static implicit operator Transform(TransfromStruct ts)
    {
        Transform tf = new TransformExtension();
        tf.localPosition = ts.pos;
        tf.localRotation = ts.rotation;
        tf.localScale = ts.scale;
        return tf;
    }
    public static implicit operator TransfromStruct(Transform ts)
    {
        return new TransfromStruct(ts.localPosition, ts.localScale, ts.localRotation);
    }
}
public class TransformExtension : Transform
{
    public TransformExtension()
    {
    }
}
