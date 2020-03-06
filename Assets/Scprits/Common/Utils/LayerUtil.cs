public class LayerUtil
{

    /// <summary>
    /// 除去某一层外的所有层级
    /// </summary>
    /// <returns></returns>
    public static int EverythingButLayer(int layer)
    {
        //camera.cullingMask = ~(1 << x);  // 渲染除去层x的所有层
        return ~(1 << layer);
    }

    /// <summary>
    /// 只看这一层        
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static int OpenLayer(int layer)
    {

        return 1 << layer;
    }
    /// <summary>
    /// 增加一层
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static int AddLayer(int originLayer, int newLayer)
    {
        return (originLayer) | (1 << newLayer);
    }
    /// <summary>
    ///  显示某一层    camera.cullingMask = 1 << x + 1 << y + 1 << z; // 摄像机只显示第x层,y层,z层.
    /// </summary>
    /// <param name="layers"></param>
    /// <returns></returns>
    public static int OpenSomeLayer(params int[] layers)
    {
        int sum = 0;
        for (int i = 0; i < layers.Length; i++)
        {
            sum += (1 << layers[i]);
        }
        return sum;
    }
}
