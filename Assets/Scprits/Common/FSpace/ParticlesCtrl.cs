using UnityEngine;
using System.Collections;

namespace future3d.unityLibs
{
    /// <summary>
    /// 粒子系统控制
    /// </summary>
    public class ParticlesCtrl : MonoBehaviour
    {

        private ParticleSystem particls;
        void Start()
        {
            particls = GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// 控制开启关闭
        /// </summary>
        public bool Enable
        {
            get { return particls.enableEmission; }
            set { particls.enableEmission = value; }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="time">几秒后销毁</param>
        public void Kill(float time = 0)
        {
            Destroy(gameObject, time);
        }

    }
}
