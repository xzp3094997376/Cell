using UnityEngine;
using System.Collections;

namespace future3d.unityLibs
{
    /// <summary>
    /// 物理碰撞检测
    /// </summary>
    public class HitCtrl : MonoBehaviour
    {

        public event System.Action<GameObject, Collider> OnTriggerEnterHandler;
        public event System.Action<GameObject, Collider> OnTriggerExitHandler;
        public event System.Action<GameObject, Collision> OnCollisionEnterHandler;
        public event System.Action<GameObject, Collision> OnCollisionExitHandler;
        public event System.Action<GameObject, ControllerColliderHit> OnControllerColliderHitHandler;

        /// <summary>
        /// 触发器-进入
        /// </summary>
        /// <param name="other"></param>
        void OnTriggerEnter(Collider other)
        {
            if (OnTriggerEnterHandler != null)
            {
                OnTriggerEnterHandler(gameObject, other);
            }
        }

        /// <summary>
        /// 触发器-离开
        /// </summary>
        /// <param name="other"></param>
        void OnTriggerExit(Collider other)
        {
            if (OnTriggerExitHandler != null)
            {
                OnTriggerExitHandler(gameObject, other);
            }
        }

        /// <summary>
        /// 碰撞-进入
        /// </summary>
        /// <param name="other"></param>
        void OnCollisionEnter(Collision other)
        {
            if (OnCollisionEnterHandler != null)
            {
                OnCollisionEnterHandler(gameObject, other);
            }
        }

        /// <summary>
        /// 碰撞-离开
        /// </summary>
        /// <param name="other"></param>
        void OnCollisionExit(Collision other)
        {
            if (OnCollisionExitHandler != null)
            {
                OnCollisionExitHandler(gameObject, other);
            }
        }

        /// <summary>
        /// 角色控制器-碰撞
        /// </summary>
        /// <param name="hit"></param>
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (OnControllerColliderHitHandler != null)
            {
                OnControllerColliderHitHandler(gameObject, hit);
            }
        }

        void OnDestroy()
        {
            if (OnTriggerEnterHandler != null)
            {
                foreach (System.Delegate d in OnTriggerEnterHandler.GetInvocationList())
                {
                    OnTriggerEnterHandler -= (System.Action<GameObject, Collider>)d;
                }
            }

            if (OnTriggerExitHandler != null)
            {
                foreach (System.Delegate d in OnTriggerExitHandler.GetInvocationList())
                {
                    OnTriggerExitHandler -= (System.Action<GameObject, Collider>)d;
                }
            }

            if (OnCollisionEnterHandler != null)
            {
                foreach (System.Delegate d in OnCollisionEnterHandler.GetInvocationList())
                {
                    OnCollisionEnterHandler -= (System.Action<GameObject, Collision>)d;
                }
            }

            if (OnCollisionExitHandler != null)
            {
                foreach (System.Delegate d in OnCollisionExitHandler.GetInvocationList())
                {
                    OnCollisionExitHandler -= (System.Action<GameObject, Collision>)d;
                }
            }

            if (OnControllerColliderHitHandler != null)
            {
                foreach (System.Delegate d in OnControllerColliderHitHandler.GetInvocationList())
                {
                    OnControllerColliderHitHandler -= (System.Action<GameObject, ControllerColliderHit>)d;
                }
            }
        }

    }
}
