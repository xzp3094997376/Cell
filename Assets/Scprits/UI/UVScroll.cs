using UnityEngine;
using System.Collections;

namespace future3d.unityLibs
{
    /// <summary>
    /// UV动画控制脚本
    /// </summary>
    public class UVScroll : MonoBehaviour
    {
        public float SpeedX; //水平方向速度
        public float SpeedY; //垂直方向速度  
        public float Direction;  //正向 or 逆向
        private Material material;  //UV动画作用的材质
        private float deltX;  //每秒变化X
        private float deltY;  //每秒变化Y

        void Start()
        {
            material = GetComponent<Renderer>().material;
        }

        void Update()
        {
            if (material)
            {
                deltX += SpeedX * Time.deltaTime * Direction;
                deltY += SpeedY * Time.deltaTime * Direction;
                material.SetTextureOffset("_MainTex", new Vector2(deltX, deltY));
            }
        }
    }
}
