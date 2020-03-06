using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MUIBehaviour : UIBehaviour
{
    /// <summary>
    /// UI大小回调
    /// </summary>
    public System.Action<Vector2> dimensionChangedAction;
    TextSpacing textSpace;//字体间距控制
    Text text;
    RectTransform rt;
    float offset = 10;
    protected override void OnEnable()
    {
        text = GetComponent<Text>();
        rt = GetComponent<RectTransform>();
        textSpace = gameObject.CheckAddComponent<TextSpacing>();
    }

    public void SetTextContent(string content)
    {
        text.text = content;
    }
    public void SetTextSpace(float space)
    {
        textSpace.Spacing = space;
    }
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        dimensionChangedAction?.Invoke(new Vector2(rt.sizeDelta.x + 100, rt.sizeDelta.y + 70));
    }
    protected override void OnDestroy()
    {
        dimensionChangedAction = null;
    }

}
