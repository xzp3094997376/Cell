using UnityEngine;

public class TipPanel : MonoBehaviour
{
    MUIBehaviour tipText;
    // Use this for initialization
    void Awake()
    {
        tipText = transform.GetChild(0).GetComponent<MUIBehaviour>();
        RectTransform rt = GetComponent<RectTransform>();
        tipText.dimensionChangedAction = delegate (Vector2 sizeDelta)
        {
            rt.sizeDelta = sizeDelta;
        };
        rt.sizeDelta = Vector2.zero;
    }
    public void SetText(string text)
    {
        gameObject.SetActive(true);
        tipText.SetTextContent(text);
    }

    public void ShowText(string content)
    {
        gameObject.SetActive(true);
        tipText.SetTextContent(content);
        Tools.DotweenInvoke<GameObject>(1, gameObject, (GameObject go) =>
        {
            gameObject.SetActive(false);
        });
    }

}
