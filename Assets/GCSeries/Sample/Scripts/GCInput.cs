using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using F3Device;
using UnityEngine;
using UnityEngine.EventSystems;


public class GCInput : MonoBehaviour
{
    #region 状态数据
    public enum MouseButton
    {
        None = -1,
        Left = 0,
        Right = 1,
        Middle = 2,
    }
    public class MousePoint
    {
        private int m_x = 0;
        private int m_y = 0;

        public MousePoint() { }
        public MousePoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public MousePoint(double x, double y)
        {
            this.X = (int)x;
            this.Y = (int)y;
        }
        public MousePoint(float x, float y)
        {
            this.X = (int)x;
            this.Y = (int)y;
        }

        public int X { get { return m_x; } set { m_x = value; } }
        public int Y { get { return m_y; } set { m_y = value; } }
    }
    public class MouseInputData
    {
        MouseState leftData = new MouseState((int)MouseButton.Left);
        MouseState rightData = new MouseState((int)MouseButton.Right);
        MouseState middleData = new MouseState((int)MouseButton.Middle);

        public bool GetMouseButtonDown(int button)
        {
            if (leftData.Button == button) return leftData.Down;
            else if (rightData.Button == button) return rightData.Down;
            else if (middleData.Button == button) return middleData.Down;
            return false;
        }
        public bool GetMouseButtonUp(int button)
        {
            if (leftData.Button == button) return leftData.Up;
            else if (rightData.Button == button) return rightData.Up;
            else if (middleData.Button == button) return middleData.Up;
            return false;
        }
        public MouseState Get(int button)
        {
            if (leftData.Button == button) return leftData;
            else if (rightData.Button == button) return rightData;
            else if (middleData.Button == button) return middleData;
            return null;
        }
        public bool IsPressedAndNoReleased(int button)
        {
            return Get(button).IsPressedAndNoReleased();
        }
        public void Reset()
        {
            leftData.Reset();
            rightData.Reset();
            middleData.Reset();
        }
        public void CopyFrom(MouseInputData data)
        {
            leftData.CoypFrom(data.Get(leftData.Button));
            rightData.CoypFrom(data.Get(rightData.Button));
            middleData.CoypFrom(data.Get(middleData.Button));
        }
        public void Combine(MouseInputData data)
        {
            leftData.Combine(data.Get(leftData.Button));
            rightData.Combine(data.Get(rightData.Button));
            middleData.Combine(data.Get(middleData.Button));
        }
    }
    public class MouseState
    {
        int button = 0;
        bool down = false;
        bool up = false;
        public int Button { get { return button; } set { button = value; } }
        public bool Down { get { return down; } set { down = value; } }
        public bool Up { get { return up; } set { up = value; } }
        public MouseState(int button)
        {
            this.button = button;
        }
        public bool IsPressedAndNoReleased()
        {
            return Down && !Up;
        }
        public void Reset()
        {
            down = false;
            up = false;
        }
        public void CoypFrom(MouseState input)
        {
            this.Down = input.Down;
            this.Up = input.Up;
        }
        public void Combine(MouseState input)
        {
            if (input.Down)
                this.Down = true;
            if (input.Up)
                this.Up = true;
        }
    }
    #endregion

    /// <summary>
    /// 编辑器下投屏是否支持输入
    /// </summary>
    public bool editorEnableInput = true;

    private static GCInput instance = new GCInput();
    public static GCInput Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GCInput>();
            return instance;
        }
    }
    public static Vector3 mouseUIPosition { get { return Instance.m_mouseUIPosition; } }
    public static Vector3 mousePosition { get { return Instance.m_mousePosition; } }
    public static Camera mouseCamera { get { return Instance.m_mouseCamera; } }
    public static Camera mouseUICamera { get { return Instance.m_mouseUICamera; } }

    /// <summary>
    /// UI专用
    /// </summary>
    /// <param name="buttonId"></param>
    /// <returns></returns>
    public static PointerEventData.FramePressState GetStateForMouseButton(int buttonId)
    {
        var pressed = Instance.InputData.GetMouseButtonDown(buttonId);
        var released = Instance.InputData.GetMouseButtonUp(buttonId);

        PointerEventData.FramePressState outState = PointerEventData.FramePressState.NotChanged;

        if (pressed && released)
            outState = PointerEventData.FramePressState.PressedAndReleased;
        else if (pressed)
            outState = PointerEventData.FramePressState.Pressed;
        else if (released)
            outState = PointerEventData.FramePressState.Released;

        if (Instance.m_LogLevel <= Common.LogLevel.DEBUG)
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.GetStateForMouseButton", $"outState={outState.ToString()} curFrameOriginType={Instance.m_curFrameOriginType.ToString()}");
        return outState;
    }

    public static bool GetMouseButtonDown(int button)
    {
#if UNITY_EDITOR
        if (!GCInput.Instance.editorEnableInput)
            return Input.GetMouseButtonDown(button);
#endif
        return Instance.InputData.GetMouseButtonDown(button);
    }
    public static bool GetMouseButtonUp(int button)
    {
#if UNITY_EDITOR
        if (!GCInput.Instance.editorEnableInput)
            return Input.GetMouseButtonUp(button);
#endif
        return Instance.InputData.GetMouseButtonUp(button);
    }

    /// <summary>
    /// 主窗口是否在3D设备显示下（一体机3D显示）
    /// </summary>
    public bool U3dIs3DShow { get { return liu.Monitor23DMode.instance.is3D; } }

    /// <summary>
    /// 投屏窗口是否3D设备显示下（大屏3D显示）
    /// </summary>
    public bool PROIs3DShow { get { return huang.common.screen.ScreenManger.Instance.CurScreenMode == huang.common.screen.ScreenManger.DualScreenMode.VR; } }
    private bool IsUsing { get { return instance == this; } }

    private Common.LogLevel m_LogLevel = Common.LogLevel.FATAL;                 
    private IntPtr m_projectorHandle = IntPtr.Zero;                               //投屏窗口句柄
    private API.SRect m_projectorWindowRect;                                      //投屏窗口位置 屏幕坐标
    private API.SRect m_u3dWindowRect;                                            //主U3D窗口范围             
    private API.SRect m_u3dMonitorRect;                                           //主U3D窗口所在的显示器的范围
    private API.SRect m_projectorMonitorRect;                                     //投屏窗口所在的显示器的范围

    [Header("2D相机")]
    public Camera m_u3dCamera;
    [Header("眼镜左相机")]
    public Camera m_u3dLeftCamera;
    [Header("眼镜右相机")]
    public Camera m_u3dRightCamera;
    [Header("AR相机")]
    public Camera m_projectorCamera;
    [Header("投屏左相机")]
    public Camera m_projectorLeftCamera;
    [Header("投屏右相机")]
    public Camera m_projectorRightCamera;

    private Vector3 m_mouseUIPosition = Vector3.zero;
    private Vector3 m_mousePosition = Vector3.zero;
    private Camera m_mouseCamera = null;
    private Camera m_mouseUICamera = null;

    private enum OriginType
    {
        PRO_2D_TOUCH,
        PRO_3D_TOUCH,

        PRO_2D_MOUSE,
        PRO_3D_MOUSE,

        U3D_2D_TOUCH,
        U3D_3D_TOUCH,

        U3D_2D_MOUSE,
        U3D_3D_MOUSE,
    }

    private API.SPoint m_curFrameMouseScreenPosition = new API.SPoint();          //当前帧鼠标屏幕坐标
    private MouseInputData m_curFrameInputData = new MouseInputData();            //当前帧按压数据 
    private MouseInputData m_recordDownInputData = new MouseInputData();          //记录之前谁按下过还未释放
    private API.SPoint m_curFrameTouchPosition = new API.SPoint();                //当前帧触屏坐标(并不在鼠标实际位置 转换过)
    private OriginType m_curFrameOriginType = OriginType.U3D_2D_MOUSE;            //当前帧数据源类型

    private bool m_u3dTouchFeedback = true;     //主屏触屏反馈
    private bool m_proTouchFeedback = true;     //投屏触屏反馈

    private MouseInputData InputData { get { return m_curFrameInputData; } }

    private bool IsTouchPressed(int flags)
    {
        return (flags != 0 && (flags & Window32Msg.TOUCHEVENTF_DOWN) != 0);
    }
    private bool IsTouchReleased(int flags)
    {
        return (flags != 0 && (flags & Window32Msg.TOUCHEVENTF_UP) != 0);
    }
    private API.SRect GetWindownRect(IntPtr hWnd)
    {
        try
        {
            if (hWnd == IntPtr.Zero)
                return new API.SRect();
            API.SPoint clientPoint = new API.SPoint();
            if (!API.ClientToScreen(hWnd, ref clientPoint))
            {
                Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.GetWindownRect", $"error{Marshal.GetLastWin32Error()} hWnd:{hWnd}");
                return new API.SRect();
            }
            API.SRect lpRect = new API.SRect();
            if (!API.GetClientRect(hWnd, out lpRect))
            {
                Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.GetWindownRect", $"error{Marshal.GetLastWin32Error()} hWnd:{hWnd}");
                return new API.SRect();
            }
            return new API.SRect(clientPoint.x, clientPoint.y, lpRect.right + clientPoint.x, lpRect.bottom + clientPoint.y);
        }
        catch (Exception ex)
        {
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.GetWindownRect", ex);
            return new API.SRect();
        }
    }


    void Awake()
    {
        if (instance != null) return;

        if(m_u3dCamera == null) Common.AppLog.AddFormat(Common.LogLevel.ERROR, "GCInput.Awake", "m_u3dCamera=null");
        if(m_u3dLeftCamera == null) Common.AppLog.AddFormat(Common.LogLevel.ERROR, "GCInput.Awake", "m_u3dLeftCamera=null");
        if(m_u3dRightCamera == null) Common.AppLog.AddFormat(Common.LogLevel.ERROR, "GCInput.Awake", "m_u3dRightCamera=null");
        if(m_projectorCamera == null) Common.AppLog.AddFormat(Common.LogLevel.ERROR, "GCInput.Awake", "m_projectorCamera=null");
        if(m_projectorLeftCamera == null) Common.AppLog.AddFormat(Common.LogLevel.ERROR, "GCInput.Awake", "m_projectorLeftCamera=null");
        if(m_projectorRightCamera == null) Common.AppLog.AddFormat(Common.LogLevel.ERROR, "GCInput.Awake", "m_projectorRightCamera=null");

        Common.AppLog.Level = Common.LogLevel.DEBUG;

        m_mouseCamera = m_u3dCamera;
        m_mouseUICamera = m_u3dCamera;

        Window32Msg.Instance.m_MouseDown = MouseDown;
        Window32Msg.Instance.m_MouseUp = MouseUp;
        Window32Msg.Instance.m_MouseMove = MouseMove;
        Window32Msg.Instance.m_MouseDouble = MouseDouble;
        Window32Msg.Instance.m_MouseTouch = U3dTouch;
        Window32Msg.Instance.m_PROMouseTouch = PROTouch;

        huang.common.screen.ScreenManger.Instance.CreateFARWindowEvent = (IntPtr handle) =>
        {
            m_projectorHandle = handle;
            Window32Msg.Instance.Notice(handle);
            F3Device.DeviceManager.Instance.Refresh();
        };
        huang.common.screen.ScreenManger.Instance.CloseFARWindowEvent = () =>
        {
            m_projectorHandle = IntPtr.Zero;
        };

    }
    void OnDestroy()
    {
        if (IsUsing)
            instance = null;
    }

    private void OnApplicationQuit()
    {
        Window32Msg.Instance.ApplicationQuit();
    }

    /// <summary>
    /// 让UI来更新 => 为了保证 其他脚本调用时不在UI获取到那帧数据前
    /// </summary>
    public void Process()
    {

        if (!IsUsing) return;
        
        if (m_LogLevel <= Common.LogLevel.DEBUG)
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.Update", $"start");
        
        /*-- 窗口数据 --*/
        m_projectorWindowRect = GetWindownRect(m_projectorHandle);
        m_projectorMonitorRect = F3Device.DeviceManager.Instance.FindMonitorRect(m_projectorWindowRect);
        m_u3dWindowRect = GetWindownRect(Window32Msg.Instance.m_u3dHandle);
        m_u3dMonitorRect = F3Device.DeviceManager.Instance.FindMonitorRect(m_u3dWindowRect);
        /*-- 鼠标位置 --*/
        API.GetCursorPos(out m_curFrameMouseScreenPosition);
        if (m_LogLevel <= Common.LogLevel.DEBUG)
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.Update", $"curFrameMouse={m_curFrameMouseScreenPosition.x} {m_curFrameMouseScreenPosition.y} ");

        //处理消息队列
        //在这里计算当前帧数据
        UpdateMsg();

        //输出对应使用数据
        UpdatePosition();
        UpdateTouchFeedback();
        
        if (m_LogLevel <= Common.LogLevel.DEBUG)
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.Update", $"end");
    }
    private void UpdatePosition()
    {
        //UI 相机不能自己更换  
        //UI检测在原生GraphicRaycaster基础上追加GCSeriesRaycaster 使用射线相机不好替换 我们将坐标往这个相机坐标转换
        m_mouseUIPosition = Input.mousePosition;
        m_mouseUICamera = m_u3dCamera;
        if (U3dIs3DShow)
            m_mouseUICamera = m_u3dLeftCamera;

        m_mouseCamera = m_u3dCamera;
        m_mousePosition = Input.mousePosition;

        if (m_curFrameOriginType == OriginType.PRO_2D_MOUSE || m_curFrameOriginType == OriginType.PRO_2D_TOUCH
           || m_curFrameOriginType == OriginType.PRO_3D_MOUSE || m_curFrameOriginType == OriginType.PRO_3D_TOUCH)
            UpdateProjectorPosition();
        else
            UpdateU3dPosition();
    }
    private void UpdateU3dPosition()
    {
        if (m_curFrameOriginType == OriginType.U3D_2D_MOUSE || m_curFrameOriginType == OriginType.U3D_2D_TOUCH)
            return; //使用原生输入数据Input.mousePosition  默认相机m_u3dCamera

        API.SPoint point = m_curFrameOriginType == OriginType.U3D_3D_TOUCH ? m_curFrameTouchPosition : m_curFrameMouseScreenPosition;

        //窗口左右
        //转到左下->右上坐标系
        int clientX = point.x - m_u3dWindowRect.left;
        int clientY = m_u3dWindowRect.bottom - point.y;

        //重新映射视屏范围 
        int w2 = m_u3dWindowRect.Width / 2;
        bool left = clientX <= w2;
        clientX = clientX > w2 ? clientX - w2 : clientX;

        float rx = clientX * 1.0f / w2;
        float ry = clientY * 1.0f / m_u3dWindowRect.Height;

        //更新针对UI检测相机使用的坐标
        m_mouseUIPosition = new Vector3(m_mouseUICamera.scaledPixelWidth * rx, m_mouseUICamera.scaledPixelHeight * ry);

        //更新空间用相机和坐标
        m_mouseCamera = left ? m_u3dLeftCamera : m_u3dRightCamera;
        API.Point offset = CameraViewOffest(m_mouseCamera);
        m_mousePosition = new Vector3(m_mouseCamera.scaledPixelWidth * rx + offset.x, m_mouseCamera.scaledPixelHeight * ry + offset.y);
    }
    private void UpdateProjectorPosition()
    {
        API.SPoint point = m_curFrameMouseScreenPosition;
        if (m_curFrameOriginType == OriginType.PRO_2D_TOUCH || m_curFrameOriginType == OriginType.PRO_3D_TOUCH)
            point = m_curFrameTouchPosition;

        if (m_curFrameOriginType == OriginType.PRO_2D_TOUCH || m_curFrameOriginType == OriginType.PRO_2D_MOUSE)
        {
            //投屏窗口正常不分左右

            //转到左下->右上坐标系
            int clientX = point.x - m_projectorWindowRect.left;
            int clientY = m_projectorWindowRect.bottom - point.y;
            float rx = clientX * 1.0f / m_projectorWindowRect.Width;
            float ry = clientY * 1.0f / m_projectorWindowRect.Height;

            //更新针对UI检测相机使用的坐标
            m_mouseUIPosition = new Vector3(m_mouseUICamera.scaledPixelWidth * rx, m_mouseUICamera.scaledPixelHeight * ry);

            //更新空间用相机和坐标
            m_mouseCamera = m_projectorLeftCamera;
            API.Point offset = CameraViewOffest(m_mouseCamera);
            m_mousePosition = new Vector3(m_mouseCamera.scaledPixelWidth * rx + offset.x, m_mouseCamera.scaledPixelHeight * ry + offset.y);
        }
        else
        {
            //投屏窗口左右

            //转到左下->右上坐标系
            int clientX = point.x - m_projectorWindowRect.left;
            int clientY = m_projectorWindowRect.bottom - point.y;

            //重新映射视屏范围 
            int w2 = m_projectorWindowRect.Width / 2;
            bool left = clientX <= w2;
            clientX = clientX > w2 ? clientX - w2 : clientX;

            float rx = clientX * 1.0f / w2;
            float ry = clientY * 1.0f / m_projectorWindowRect.Height;

            //更新针对UI检测相机使用的坐标
            m_mouseUIPosition = new Vector3(m_mouseUICamera.scaledPixelWidth * rx, m_mouseUICamera.scaledPixelHeight * ry);

            //更新空间用相机和坐标
            m_mouseCamera = left ? m_projectorLeftCamera : m_projectorRightCamera;
            API.Point offset = CameraViewOffest(m_mouseCamera);
            m_mousePosition = new Vector3(m_mouseCamera.scaledPixelWidth * rx + offset.x, m_mouseCamera.scaledPixelHeight * ry + offset.y);
        }
    }


    /// <summary>
    /// 计算相机视图偏移量
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    private API.Point CameraViewOffest(Camera camera)
    {
        if (camera == null)
            return new API.Point();
        float x = m_mouseCamera.rect.x * m_mouseCamera.scaledPixelWidth / m_mouseCamera.rect.width;
        float y = m_mouseCamera.rect.y * m_mouseCamera.scaledPixelHeight / m_mouseCamera.rect.height;
        return new API.Point((int)x, (int)y);
    }
    private void UpdateTouchFeedback()
    {
        IntPtr hWnd = Window32Msg.Instance.m_u3dHandle;
        if (hWnd == IntPtr.Zero)
            return;

        if (U3dIs3DShow && m_u3dTouchFeedback)
        {
            F3Device.API.SetTouchFeedback(hWnd, false);
            m_u3dTouchFeedback = false;
        }
        if (!U3dIs3DShow && !m_u3dTouchFeedback)
        {
            F3Device.API.SetTouchFeedback(hWnd, true);
            m_u3dTouchFeedback = true;
        }

        if (PROIs3DShow && m_proTouchFeedback)
        {
            Window32Msg.Instance.SetTouchFeedback(m_projectorHandle, false);
            m_proTouchFeedback = false;
        }
        if (!PROIs3DShow && !m_proTouchFeedback)
        {
            Window32Msg.Instance.SetTouchFeedback(m_projectorHandle, true);
            m_proTouchFeedback = true;
        }
    }


    #region Msg 接收处理 
    class MsgData
    {
        public OriginType m_type;
        public MouseInputData m_data;
        public API.SPoint m_point;

        public MsgData(OriginType type, MouseInputData data, API.SPoint point)
        {
            m_type = type;
            m_data = data;
            m_point = point;
        }
    }
    class MsgDataList : List<MsgData>
    {
        public new void Add(MsgData item)
        {
            if (base.Count > 100) base.RemoveAt(0);
            base.Add(item);
            if (GCInput.Instance.m_LogLevel <= Common.LogLevel.DEBUG)
            {
                Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.AddMsgData", $"OriginType={item.m_type.ToString()}");
                Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.AddMsgData", $"LeftDown={item.m_data.GetMouseButtonDown(0)} LeftUp={item.m_data.GetMouseButtonUp(0)}");
            }
        }
    }

    MsgDataList m_msgList = new MsgDataList();

    /// <summary>
    /// 处理消息 输出到帧数据 
    /// 调用顺序 ： 窗口消息=>UI脚本调用输入值=>本脚本Update 
    /// 为了同步状态数据和坐标数据帧
    /// </summary>
    private void UpdateMsg()
    {
        m_curFrameInputData.Reset();
        //目前没有去接收处理鼠标移动消息
        if (m_msgList.Count == 0)
        {
            //没有消息：

            //3D主屏鼠标没有消息 => 没有按压状态改变 => 可能坐标改变 => 继续使用m_curFrameMouseScreenPosition
            //3D主屏触屏没有消息 => 没有任何变动

            //投屏鼠标没有消息 => 没有按压状态改变 => 可能坐标改变 => 继续使用m_curFrameMouseScreenPosition
            //投屏触屏没有消息 => 没有任何变动

            //2D主屏鼠标/触屏 => 更新原生Input状态 => 因为后面使用的也是原生Input.mousePosition
            if (m_curFrameOriginType == OriginType.U3D_2D_MOUSE || m_curFrameOriginType == OriginType.U3D_2D_TOUCH)
            {
                m_curFrameInputData.Get(0).Down = Input.GetMouseButtonDown(0);
                m_curFrameInputData.Get(1).Down = Input.GetMouseButtonDown(1);
                m_curFrameInputData.Get(2).Down = Input.GetMouseButtonDown(2);
                m_curFrameInputData.Get(0).Up = Input.GetMouseButtonUp(0);
                m_curFrameInputData.Get(1).Up = Input.GetMouseButtonUp(1);
                m_curFrameInputData.Get(2).Up = Input.GetMouseButtonUp(2);
            }
            return;
        }
        //可能最后使用的触屏位置
        API.SPoint finalPoint = m_msgList[m_msgList.Count - 1].m_point;
        //过滤
        OriginType finalType = m_msgList[m_msgList.Count - 1].m_type;
        for (int i = m_msgList.Count - 1; i >= 0; i--)
        {
            MsgData msg = m_msgList[i];
            if (msg.m_type != finalType)
                m_msgList.RemoveAt(i);
        }
        MouseInputData finalInput = null;
        //合并按压数据
        foreach (MsgData msg in m_msgList)
        {
            if (finalInput == null)
                finalInput = msg.m_data;
            else
                finalInput.Combine(msg.m_data);
        }
        m_msgList.Clear();
        //这样干 至少编辑器里能兼容情况下原生输入
        if (finalType == OriginType.U3D_2D_MOUSE || finalType == OriginType.U3D_2D_TOUCH)
        {
            finalInput.Get(0).Down = Input.GetMouseButtonDown(0);
            finalInput.Get(1).Down = Input.GetMouseButtonDown(1);
            finalInput.Get(2).Down = Input.GetMouseButtonDown(2);
            finalInput.Get(0).Up = Input.GetMouseButtonUp(0);
            finalInput.Get(1).Up = Input.GetMouseButtonUp(1);
            finalInput.Get(2).Up = Input.GetMouseButtonUp(2);
        }

        //数据源切换补充逻辑
        if (m_curFrameOriginType != finalType)
        {
            bool pressed0 = m_recordDownInputData.IsPressedAndNoReleased(0);
            bool pressed1 = m_recordDownInputData.IsPressedAndNoReleased(1);
            bool pressed2 = m_recordDownInputData.IsPressedAndNoReleased(2);
            //补充一次释放
            if (pressed0) finalInput.Get(0).Up = true;
            if (pressed1) finalInput.Get(1).Up = true;
            if (pressed2) finalInput.Get(2).Up = true;
        }

        //当前帧输出状态只要有释放状态 重新记录
        if (finalInput.Get(0).Down) m_recordDownInputData.Get(0).Down = true;
        if (finalInput.Get(1).Down) m_recordDownInputData.Get(1).Down = true;
        if (finalInput.Get(2).Down) m_recordDownInputData.Get(2).Down = true;
        if (finalInput.Get(0).Up) m_recordDownInputData.Get(0).Reset();
        if (finalInput.Get(1).Up) m_recordDownInputData.Get(1).Reset();
        if (finalInput.Get(2).Up) m_recordDownInputData.Get(2).Reset();

        //同步更新数据
        m_curFrameInputData.CopyFrom(finalInput);
        m_curFrameOriginType = finalType;
        m_curFrameTouchPosition = finalPoint;


        if (m_LogLevel <= Common.LogLevel.DEBUG)
        {
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.UpdateMsg", $"curFrameOriginType={m_curFrameOriginType}");
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.UpdateMsg", $"LeftDown={m_curFrameInputData.GetMouseButtonDown(0)} LeftUp={m_curFrameInputData.GetMouseButtonUp(0)}");
        }
    }

    private void MouseDown(Window32Msg.MouseOrigin origin, MouseButton arg1, MousePoint arg2)
    {
        MouseInputData data = new MouseInputData();
        data.Get((int)arg1).Down = true;

        OriginType type = OriginType.U3D_2D_MOUSE;
        switch (origin)
        {
            case Window32Msg.MouseOrigin.PRO_MOUSE: type = PROIs3DShow ? OriginType.PRO_3D_MOUSE : OriginType.PRO_2D_MOUSE; break;
            case Window32Msg.MouseOrigin.PRO_TOUCH: type = PROIs3DShow ? OriginType.PRO_3D_TOUCH : OriginType.PRO_2D_TOUCH; break;
            case Window32Msg.MouseOrigin.U3D_MOUSE: type = U3dIs3DShow ? OriginType.U3D_3D_MOUSE : OriginType.U3D_2D_MOUSE; break;
            case Window32Msg.MouseOrigin.U3D_TOUCH: type = U3dIs3DShow ? OriginType.U3D_3D_TOUCH : OriginType.U3D_2D_TOUCH; break;
        }
        if (m_LogLevel <= Common.LogLevel.DEBUG)
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.MouseDown", $"Button={arg1.ToString()}  OriginType={type.ToString()} point={arg2.X} {arg2.Y}");

        m_msgList.Add(new MsgData(type, data, new API.SPoint(arg2.X, arg2.Y)));
    }
    private void MouseUp(Window32Msg.MouseOrigin origin, MouseButton arg1, MousePoint arg2)
    {
        MouseInputData data = new MouseInputData();
        data.Get((int)arg1).Up = true;

        OriginType type = OriginType.U3D_2D_MOUSE;
        switch (origin)
        {
            case Window32Msg.MouseOrigin.PRO_MOUSE: type = PROIs3DShow ? OriginType.PRO_3D_MOUSE : OriginType.PRO_2D_MOUSE; break;
            case Window32Msg.MouseOrigin.PRO_TOUCH: type = PROIs3DShow ? OriginType.PRO_3D_TOUCH : OriginType.PRO_2D_TOUCH; break;
            case Window32Msg.MouseOrigin.U3D_MOUSE: type = U3dIs3DShow ? OriginType.U3D_3D_MOUSE : OriginType.U3D_2D_MOUSE; break;
            case Window32Msg.MouseOrigin.U3D_TOUCH: type = U3dIs3DShow ? OriginType.U3D_3D_TOUCH : OriginType.U3D_2D_TOUCH; break;
        }
        if (m_LogLevel <= Common.LogLevel.DEBUG)
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.MouseUp", $"Button={arg1.ToString()}  OriginType={type.ToString()} point={arg2.X} {arg2.Y}");

        m_msgList.Add(new MsgData(type, data, new API.SPoint(arg2.X, arg2.Y)));
    }
    private void MouseMove(Window32Msg.MouseOrigin origin, MouseButton arg1, MousePoint arg2)
    {
        return;

        /*

        if (m_outLog)
            Common.AppLog.AddMsg(Common.LogLevel.INFO, $"GCInput.MouseMove : Button={arg1.ToString()}  isPRO={isPRO} point={arg2.X} {arg2.Y}");

        if (m_curFrameOriginType == OriginType.PRO_2D_TOUCH && isPRO)
        {
            //这是2D投屏触屏引发的移动
            //OriginType type = OriginType.PRO_2D_TOUCH;
            //OriginInput(type, new API.SPoint(arg2.X + m_projectorWindowRect.Left, arg2.Y + m_projectorWindowRect.Top));
        }
        else if (m_curFrameOriginType == OriginType.PRO_3D_TOUCH && isPRO)
        {
            //这是3D投屏触屏引发的移动
            //OriginType type = OriginType.PRO_3D_TOUCH;
            //API.SPoint out1, out2;
            //API.SPoint input = new API.SPoint(arg2.X + m_projectorWindowRect.Left, arg2.Y + m_projectorWindowRect.Top);
            //TouchTransform.TouchPosition(m_projectorScreenRect, input, out out1, out out2);
            //OriginInput(type, out1);
        }
        else if (m_curFrameOriginType == OriginType.U3D_2D_TOUCH && !isPRO)
        {
            //这是2D主窗口触屏引发的移动
            //不处理  使用了原生输入数据
        }
        else if (m_curFrameOriginType == OriginType.U3D_3D_TOUCH && !isPRO)
        {
            //这是3D主窗口触屏引发的移动
            OriginType type = OriginType.U3D_3D_TOUCH;
            API.SPoint out1, out2;
            API.SPoint input = new API.SPoint(arg2.X + m_u3dWindowRect.Left, arg2.Y + m_u3dWindowRect.Top);
            TouchTransform.TouchPosition(m_u3dScreenRect, input, out out1, out out2);
            OriginInput(type, out1);
        }
        else
        {
            //投屏窗口上鼠标移动和主窗口鼠标移动我们不获取数据
            //使用m_curFrameMouseScreenPosition
        }
        */
    }
    private void MouseDouble(Window32Msg.MouseOrigin origin, MouseButton arg1, MousePoint arg2)
    {

    }
    private void U3dTouch(int dwFlags, int screenx, int screeny)
    {
        if (m_LogLevel <= Common.LogLevel.DEBUG)
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.U3dTouch", "");

        MouseInputData data = new MouseInputData();
        data.Get(0).Down = IsTouchPressed(dwFlags);
        data.Get(0).Up = IsTouchReleased(dwFlags);

        if (U3dIs3DShow)
        {
            API.SPoint out1, out2;
            TouchTransform.TouchPosition(m_u3dMonitorRect, new API.SPoint(screenx, screeny), out out1, out out2);
            m_msgList.Add(new MsgData(OriginType.U3D_3D_TOUCH, data, new API.SPoint(out1.x, out1.y)));
        }
        else
            m_msgList.Add(new MsgData(OriginType.U3D_2D_TOUCH, data, new API.SPoint(screenx, screeny)));
    }
    private void PROTouch(int dwFlags, int screenx, int screeny)
    {
        if (m_LogLevel <= Common.LogLevel.DEBUG)
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "GCInput.PROTouch", "");

        MouseInputData data = new MouseInputData();
        data.Get(0).Down = IsTouchPressed(dwFlags);
        data.Get(0).Up = IsTouchReleased(dwFlags);

        if (PROIs3DShow)
        {
            API.SPoint out1, out2;
            TouchTransform.TouchPosition(m_projectorMonitorRect, new API.SPoint(screenx, screeny), out out1, out out2);
            m_msgList.Add(new MsgData(OriginType.PRO_3D_TOUCH, data, new API.SPoint(out1.x, out1.y)));
        }
        else
        {
            m_msgList.Add(new MsgData(OriginType.PRO_2D_TOUCH, data, new API.SPoint(screenx, screeny)));
        }
    }
    #endregion

}

public class Window32Msg
{
    public enum MouseOrigin
    {
        PRO_TOUCH,
        PRO_MOUSE,

        U3D_TOUCH,
        U3D_MOUSE,
    }

    private static Window32Msg instance = new Window32Msg();
    public static Window32Msg Instance
    {
        get
        {
            if (instance == null)
                instance = new Window32Msg();
            return instance;
        }
    }

    internal const int WM_MOUSEMOVE = 0x200;        //移动鼠标
    internal const int WM_LBUTTONDOWN = 0x201;      //按下鼠标左键
    internal const int WM_LBUTTONUP = 0x202;        //释放鼠标左键
    internal const int WM_LBUTTONDBLCLK = 0x203;    //双击鼠标左键
    internal const int WM_RBUTTONDOWN = 0x204;      //按下鼠标右键
    internal const int WM_RBUTTONUP = 0x205;        //释放鼠标右键
    internal const int WM_RBUTTONDBLCLK = 0x206;    //双击鼠标右键
    internal const int WM_MBUTTONDOWN = 0x207;      //按下鼠标中键 
    internal const int WM_MBUTTONUP = 0x208;        //释放鼠标中键
    internal const int WM_MBUTTONDBLCLK = 0x209;    //双击鼠标中键
    internal const int WM_MOUSEWHEEL = 0x20A;
    internal const int WM_TOUCH = 0x0240;           //触屏

    /*--投屏窗口交互消息--*/
    internal const int PRO_WM_ADD = 0x4000;
    internal const int PRO_WM_TOUCH = PRO_WM_ADD + WM_TOUCH;
    internal const int PRO_WM_MOUSEMOVE = PRO_WM_ADD + WM_MOUSEMOVE;             //移动鼠标
    internal const int PRO_WM_LBUTTONDOWN = PRO_WM_ADD + WM_LBUTTONDOWN;         //按下鼠标左键
    internal const int PRO_WM_LBUTTONUP = PRO_WM_ADD + WM_LBUTTONUP;             //释放鼠标左键
    internal const int PRO_WM_LBUTTONDBLCLK = PRO_WM_ADD + WM_LBUTTONDBLCLK;     //双击鼠标左键
    internal const int PRO_WM_RBUTTONDOWN = PRO_WM_ADD + WM_RBUTTONDOWN;         //按下鼠标右键
    internal const int PRO_WM_RBUTTONUP = PRO_WM_ADD + WM_RBUTTONUP;             //释放鼠标右键
    internal const int PRO_WM_RBUTTONDBLCLK = PRO_WM_ADD + WM_RBUTTONDBLCLK;     //双击鼠标右键
    internal const int PRO_WM_MBUTTONDOWN = PRO_WM_ADD + WM_MBUTTONDOWN;         //按下鼠标中键 
    internal const int PRO_WM_MBUTTONUP = PRO_WM_ADD + WM_MBUTTONUP;             //释放鼠标中键
    internal const int PRO_WM_MBUTTONDBLCLK = PRO_WM_ADD + WM_MBUTTONDBLCLK;     //双击鼠标中键
    internal const int PRO_WM_MOUSEWHEEL = PRO_WM_ADD + WM_MOUSEWHEEL;

    internal const int PRO_WM_OPEN = 1034;
    internal const int PRO_WM_CLOSE = PRO_WM_OPEN + 1;
    internal const int PRO_OPEN_TOUCH_FEEDBACK = PRO_WM_OPEN + 5;               //打开投屏窗口触屏反馈效果
    internal const int PRO_CLOSE_TOUCH_FEEDBACK = PRO_WM_OPEN + 6;              //关闭投屏窗口触屏反馈效果

    private bool m_isClosed = false;
    private IntPtr m_oldWndProc = IntPtr.Zero;
    private F3Device.API.CallBack m_newWndProc = null;

    public IntPtr m_u3dHandle = IntPtr.Zero;                                         //主程序窗口句柄
    /// <summary>
    /// 参数：dwFlags , x  ,  y
    /// </summary>
    public Action<int, int, int> m_MouseTouch;                                       //主窗口触屏
    public Action<int, int, int> m_PROMouseTouch;                                    //投屏窗口触屏

    public Action<MouseOrigin, GCInput.MouseButton, GCInput.MousePoint> m_MouseDown;
    public Action<MouseOrigin, GCInput.MouseButton, GCInput.MousePoint> m_MouseUp;
    public Action<MouseOrigin, GCInput.MouseButton, GCInput.MousePoint> m_MouseMove;
    public Action<MouseOrigin, GCInput.MouseButton, GCInput.MousePoint> m_MouseDouble;


    private Window32Msg()
    {
        m_newWndProc = new F3Device.API.CallBack(this.CustomProc);
        m_u3dHandle = F3Device.API.GetProcessWnd();

#if UNITY_EDITOR
        if (!GCInput.Instance.editorEnableInput) return;
        //找game窗口
        F3Device.API.EnumChildWindows(m_u3dHandle, new F3Device.API.CHILDWNDENUMPROC(EnumGameViewWindow), 0);
#endif
        m_oldWndProc = F3Device.API.SetWindowLongPtr(m_u3dHandle, F3Device.API.GWL_WNDPROC, this.m_newWndProc);
        if (m_oldWndProc == IntPtr.Zero)
            Common.AppLog.AddFormat(Common.LogLevel.ERROR, "WindowMsg.WindowMsg", $"设置窗口过程失败 主窗口hwnd={m_u3dHandle} win32error:{Marshal.GetLastWin32Error()}");
        else
            Common.AppLog.AddFormat(Common.LogLevel.INFO, "WindowMsg.WindowMsg", $"设置窗口过程成功 主窗口hwnd={m_u3dHandle}");
    }
    private bool EnumGameViewWindow(IntPtr hwnd, int lParam)
    {
        int cTxtLen = F3Device.API.GetWindowTextLength(hwnd.ToInt32()) + 1;
        System.Text.StringBuilder text = new System.Text.StringBuilder(cTxtLen);
        F3Device.API.GetWindowText(hwnd.ToInt32(), text, cTxtLen);
        string title = text.ToString();
        if (title.Contains("GameView"))
        {
            m_u3dHandle = hwnd;
            return false;
        }
        return true;
    }
    public void ApplicationQuit()
    {
        if (this.m_oldWndProc != IntPtr.Zero)
        {
            F3Device.API.SetWindowLongPtr(m_u3dHandle, F3Device.API.GWL_WNDPROC, this.m_oldWndProc);
            this.m_oldWndProc = IntPtr.Zero;
        }
    }
    public void Notice(IntPtr handle)
    {
        if (m_u3dHandle != IntPtr.Zero)

#if UNITY_EDITOR
            API.PostMessage(handle, PRO_WM_OPEN, m_u3dHandle, new IntPtr(1));//编辑器模式
#else
            API.PostMessage(handle, PRO_WM_OPEN, m_u3dHandle, IntPtr.Zero);
#endif
    }
    public void SetTouchFeedback(IntPtr handle, bool enable)
    {
        if (handle != IntPtr.Zero)
            API.PostMessage(handle, (uint)(enable ? PRO_OPEN_TOUCH_FEEDBACK : PRO_CLOSE_TOUCH_FEEDBACK), IntPtr.Zero, IntPtr.Zero);
    }
    private IntPtr CustomProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
    {
        if (m_isClosed) return IntPtr.Zero;
        //关闭慢的处理 
        if (msg == F3Device.API.WM_CLOSE || msg == F3Device.API.WM_DESTROY || msg == F3Device.API.WM_QUIT)
        {
            m_isClosed = true;

            if (m_u3dHandle != IntPtr.Zero && m_oldWndProc != IntPtr.Zero)
                F3Device.API.SetWindowLongPtr(m_u3dHandle, F3Device.API.GWL_WNDPROC, this.m_oldWndProc);
        }
        if (!m_isClosed)
        {
            MsgHandle(hWnd, msg, wParam, lParam);
        }
        return F3Device.API.CallWindowProc(m_oldWndProc, hWnd, msg, wParam, lParam);
    }

    private bool TouchMsg(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_TOUCH)
        {
            int inputCount = LoWord(wParam.ToInt32());
            STouchInput[] inputs = new STouchInput[inputCount];
            if (GetTouchInputInfo(lParam, inputCount, inputs, touchInputSize))
            {
                if (m_MouseTouch != null)
                    m_MouseTouch(inputs[0].dwFlags, inputs[0].x / 100, inputs[0].y / 100);
            }
            return true;
        }
        if (msg == PRO_WM_TOUCH)
        {
            ushort sx = (ushort)lParam;
            ushort sy = (ushort)(lParam.ToInt32() >> 16);
            //ushort proHwnd = (ushort)wParam;
            ushort touchFlags = (ushort)(wParam.ToInt32() >> 16);
            if (m_PROMouseTouch != null)
                m_PROMouseTouch(touchFlags, sx, sy);
            return true;
        }
        return false;
    }

    private void MsgHandle(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == PRO_WM_OPEN)
        {
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "WindowMsg.MsgHandle", $"hWnd:{hWnd}  msg:{msg} 投屏窗口成功连接");
            return;
        }
        if (msg == PRO_WM_CLOSE)
        {
            Common.AppLog.AddFormat(Common.LogLevel.DEBUG, "WindowMsg.MsgHandle", $"hWnd:{hWnd}  msg:{msg} 投屏窗口关闭");
            return;
        }

        if (TouchMsg(hWnd, msg, wParam, lParam)) return;

        MouseOrigin origin = MouseOrigin.PRO_MOUSE;
        //将投屏转发过来的鼠标消息处理
        if (msg >= PRO_WM_MOUSEMOVE && msg <= PRO_WM_MBUTTONDBLCLK)
        {
            msg -= PRO_WM_ADD;
            //ushort proHwnd = (ushort)wParam;
            ushort touchFlags = (ushort)(wParam.ToInt32() >> 16);
            origin = touchFlags != 0 ? MouseOrigin.PRO_TOUCH : MouseOrigin.PRO_MOUSE;
        }
        else if (msg >= WM_MOUSEMOVE && msg <= WM_MBUTTONDBLCLK)
        {
            origin = IsTouchEvent() ? MouseOrigin.U3D_TOUCH : MouseOrigin.U3D_MOUSE;
        }

        if (msg < WM_MOUSEMOVE || msg > WM_MBUTTONDBLCLK) return;
        if (origin == MouseOrigin.PRO_TOUCH || origin == MouseOrigin.U3D_TOUCH) return; //触摸提升的鼠标事件忽略掉

        ushort x = (ushort)lParam;
        ushort y = (ushort)(lParam.ToInt32() >> 16);
        GCInput.MousePoint point = new GCInput.MousePoint(x, y);

        switch (msg)
        {
            case WM_MOUSEMOVE: if (m_MouseMove != null) m_MouseMove(origin, GCInput.MouseButton.None, point); break;
            case WM_LBUTTONDOWN: if (m_MouseDown != null) m_MouseDown(origin, GCInput.MouseButton.Left, point); break;
            case WM_LBUTTONUP: if (m_MouseUp != null) m_MouseUp(origin, GCInput.MouseButton.Left, point); break;
            case WM_LBUTTONDBLCLK: if (m_MouseDouble != null) m_MouseDouble(origin, GCInput.MouseButton.Left, point); break;
            case WM_RBUTTONDOWN: if (m_MouseDown != null) m_MouseDown(origin, GCInput.MouseButton.Right, point); break;
            case WM_RBUTTONUP: if (m_MouseUp != null) m_MouseUp(origin, GCInput.MouseButton.Right, point); break;
            case WM_RBUTTONDBLCLK: if (m_MouseDouble != null) m_MouseDouble(origin, GCInput.MouseButton.Right, point); break;
            case WM_MBUTTONDOWN: if (m_MouseDown != null) m_MouseDown(origin, GCInput.MouseButton.Middle, point); break;
            case WM_MBUTTONUP: if (m_MouseUp != null) m_MouseUp(origin, GCInput.MouseButton.Middle, point); break;
            case WM_MBUTTONDBLCLK: if (m_MouseDouble != null) m_MouseDouble(origin, GCInput.MouseButton.Middle, point); break;
            default: break;
        }
    }

    #region 触屏消息
    //投屏 会提升触屏到鼠标消息  主窗口不会

    public const int TOUCHEVENTF_MOVE = 0x0001;
    public const int TOUCHEVENTF_DOWN = 0x0002;
    public const int TOUCHEVENTF_UP = 0x0004;

    [StructLayout(LayoutKind.Sequential)]
    public struct STouchInput
    {
        public int x;
        public int y;
        public System.IntPtr hSource;
        public int dwID;
        public int dwFlags;
        public int dwMask;
        public int dwTime;
        public System.IntPtr dwExtraInfo;
        public int cxContact;
        public int cyContact;
    }
    private int touchInputSize = Marshal.SizeOf(new STouchInput());

    [DllImport("user32")]
    public static extern bool GetTouchInputInfo(System.IntPtr hTouchInput, int cInputs, [In, Out] STouchInput[] pInputs, int cbSize);
    private static int LoWord(int number)
    {
        return (number & 0xffff);
    }

    [DllImport("user32", SetLastError = false)]
    static extern int GetMessageExtraInfo();

    public const uint MI_WP_SIGNATURE = 0xFF515700;
    public const uint SIGNATURE_MASK = 0xFFFFFF00;
    bool IsTouchEvent()
    {
        int dw = GetMessageExtraInfo();
        return (dw & SIGNATURE_MASK) == MI_WP_SIGNATURE;
    }

    #endregion

}
