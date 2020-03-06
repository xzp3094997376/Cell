using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace liu
{
    /// <summary>
    /// 需要有交互的canvas上的UI交互需要挂载
    /// 注意Graphic Raycast 需要在 F3dSpace Raycast 之上
    /// </summary>
    [RequireComponent(typeof(GCSeriesRaycaster), typeof(Canvas))]
    public class InteractCanvasSetting : MonoBehaviour
    {
        Camera camera2D;
        Camera cameraLeft;
        Canvas canvas;

        GCSeriesRaycaster f3dSpaceRaycaster;
        GraphicRaycaster graphicRaycaster;

        private void Start()
        {
            camera2D = Monitor23DMode.instance.camera2D;
            cameraLeft = Monitor23DMode.instance.CameraLeft;
            canvas = GetComponent<Canvas>();

            f3dSpaceRaycaster = GetComponent<GCSeriesRaycaster>();
            if (f3dSpaceRaycaster == null) f3dSpaceRaycaster = gameObject.AddComponent<GCSeriesRaycaster>();

            //需要注意的是 F3dSpaceRaycaster 继承于 GraphicRaycaster
            //所有 GraphicRaycaster 需要层级需要在  F3dSpaceRaycaster 之上
            graphicRaycaster = GetComponent<GraphicRaycaster>();
            if (graphicRaycaster == null) graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        }

        void Update()
        {
            if (Monitor23DMode.instance.is3D)
            {
                canvas.worldCamera = cameraLeft;
                //graphicRaycaster.enabled = false;
                f3dSpaceRaycaster.enabled = true;
            }
            else
            {
                canvas.worldCamera = camera2D;
                graphicRaycaster.enabled = true;
                f3dSpaceRaycaster.enabled = false;
            }
        }

    }
}
