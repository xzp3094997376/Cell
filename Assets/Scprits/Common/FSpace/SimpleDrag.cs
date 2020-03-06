using liu;
using operatemodeltool;
using UnityEngine;
using xuexue.common.drag2dtool;

namespace GCSeries
{
    /// <summary>
    /// 简单拖拽的示例
    /// </summary>
    public class SimpleDrag : MonoBehaviour
    {
        private Common.LogLevel m_LogLevel = Common.LogLevel.FATAL;
        /// <summary>
        /// 创建的笔的射线物体
        /// </summary>
        GameObject _penObj;

        [HideInInspector]
        public PenRay tempPenRay;

        /// <summary>
        /// 是否在点击的时候震动一下
        /// </summary>
        public bool enableShake = true;

        [HideInInspector]
        /// <summary>
        /// 后键双击判定时间
        /// </summary>
        public float doubleClickTimes = 0.22f;

        Camera camera2D;
        void Start()
        {
            //设置屏幕为3D显示模式
            // FCore.SetScreen3D();

            FCore.EventKey0Down += OnKey0Down;
            FCore.EventKey0Up += OnKey0Up;

            FCore.EventKey1Down += OnKey0Down;
            FCore.EventKey1Up += OnKey0Up;

            FCore.EventKey2Down += OnKey2Down;
            FCore.EventKey2Up += OnKey2Up;

            _penObj = new GameObject("penRay");
            tempPenRay = _penObj.AddComponent<PenRay>();


            //通过3DUI物体找到挂在在上面的UIButton3D脚本。
            // uibutton3d = FindObjectOfType<UIButton3D>();
            camera2D = Monitor23DMode.instance.camera2D;
        }

        void OnApplicationQuit()
        {
            //在程序退出的时候设置屏幕为2D显示
            FCore.SetScreen2D();
        }

        private void OnKey2Up()
        {
            OnKey0Up();
        }

        private float lastClickTime;

        private void OnKey2Down()
        {
            if (Time.realtimeSinceStartup - lastClickTime < doubleClickTimes)
            {
                Debug.Log("SimpleDrag.OnKey2Down(): ");
                if (GlobalConfig.Instance._curOperateObj != null)
                {
                    GlobalConfig.Instance._curOperateObj.transform.position = tempPenRay.transform.Find("trackHandle/Pen").position;
                }

                OnKey0Down();
            }

            lastClickTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 记录当前拖拽的物体
        /// </summary>
        [HideInInspector]
        public GameObject _curDragObj;

        private GameObject Raycast(out RaycastHit raycastHit)
        {
            if (Physics.Raycast(FCore.penRay, out raycastHit, tempPenRay.rayLength))
            {
                return raycastHit.collider.gameObject;
            }
            return null;
        }


        /// <summary>
        /// 3D 拖拽或者移动
        /// </summary>
        private void OnKey0Down()
        {

            //F3D.AppLog.AddMsg($"OnKey0Down 开始抓取物体");
            RaycastHit raycastHit;
            GameObject dragObj = Raycast(out raycastHit);
            if (dragObj == null)
            {
                //F3D.AppLog.AddMsg($"OnKey0Down 开始抓取物体 = null");
            }
            if (dragObj != null)
            {
                if (Monitor23DMode.instance.is3D)
                {
                    _curDragObj = dragObj;
                    //F3D.AppLog.AddMsg($"OnKey0Down 抓取物体 {_curDragObj.name}");

                    if (GlobalConfig.Instance.operationModel == OperationModel.Move)//移动物体
                    {

                        //添加抓取的物体
                        FCore.addDragObj(_curDragObj, raycastHit.distance, true);
                    }
                    else if (GlobalConfig.Instance.operationModel == OperationModel.Rotate)//旋转物体
                    {
                        OperationModelTool.Instance.AddRotaObject(_curDragObj, true);
                    }

                    if (enableShake)
                    {
                        FCore.PenShake();//震动一下
                    }

                    GlobalConfig.Instance._curOperateObj = _curDragObj;

                }
            }
        }

        public void OnKey0Up()
        {
            //移出抓取的物体
            FCore.deleteDragObj(_curDragObj);

            //移除旋转的物体
            OperationModelTool.Instance.DeleRotaObject();

            _curDragObj = null;
        }

        private void Update()
        {
            if (m_LogLevel <= Common.LogLevel.DEBUG)
                Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "SimpleDrag.Update", "");
            if (GCInput.GetMouseButtonDown(0))
            {
                if (m_LogLevel <= Common.LogLevel.DEBUG)
                    Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "SimpleDrag.Update", "leftDown=true");
                Drag2DObj();
            }

            if (GCInput.GetMouseButtonUp(0))
            {
                OnMouseBtnUp();
            }
        }

        void OnMouseBtnUp()
        {
            Drag2DTool.Instance.clearDragObj();
            OperationModelTool.Instance.DeleRotaObject();
            _curDragObj = null;
        }

        void Drag2DObj()
        {
            RaycastHit raycastHit;
            //int defaultLayer = LayerMask.NameToLayer("Default");//这个层是模型

            Ray ray = GCInput.mouseCamera.ScreenPointToRay(GCInput.mousePosition);

            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;
            if (m_LogLevel <= Common.LogLevel.DEBUG)
                Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "SimpleDrag.Drag2DObj", "UI no hit");
            GameObject dragObj = null;
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity/*, 1 << defaultLayer*/))
            {
                //if (uiDis < raycastHit.distance)//通过鼠标到UI跟鼠标到物体的距离判断是否进行对模型操作
                //{
                //    return;
                //}
                dragObj = raycastHit.collider.gameObject;
                _curDragObj = dragObj;

                if (_curDragObj.tag=="DontDrag")
                {
                    return;
                }

                if (GlobalConfig.Instance.operationModel == OperationModel.Move)
                {
                    Drag2DTool.Instance.addDragObj(_curDragObj, GCInput.mouseCamera);
                }
                else if (GlobalConfig.Instance.operationModel == OperationModel.Rotate)
                {
                    OperationModelTool.Instance.AddRotaObject(_curDragObj, false);
                }

                GlobalConfig.Instance._curOperateObj = _curDragObj;
            }
        }

    }
}
