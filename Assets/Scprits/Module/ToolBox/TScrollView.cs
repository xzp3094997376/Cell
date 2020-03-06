using UnityEngine.EventSystems;
using UnityEngine.UI;
[System.Serializable]
public class TScrollView : ScrollRect
{

    public override void OnScroll(PointerEventData data)
    {
        //base.OnScroll(data);
        //Debug.Log("scroll");

    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
    }

    public override void OnDrag(PointerEventData eventData)
    {
        //base.OnDrag(eventData);
        //Debug.Log("OnDrag");
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        //base.OnEndDrag(eventData);
        //Debug.Log("OnEndDrag");
    }

}
