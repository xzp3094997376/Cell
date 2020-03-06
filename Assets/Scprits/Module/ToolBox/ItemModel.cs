using UnityEngine;

public class ItemModel : MonoBehaviour
{
    Vector3 pos;
    // Use this for initialization
    void Start()
    {
        pos = Vector3.zero;
        Transform par = Main.instance.transform.Find("AMoudle") == null ? Main.instance.transform.Find("BMoudle").Find("ObjectsPar") : Main.instance.transform.Find("AMoudle").Find("ObjectsPar");
        transform.SetParent(par);
    }

    public void MoveTo()
    {
        switch (this.name)
        {
            case "light":
                pos = new Vector3(0.5907882f, 1.33756f, 0.739f);
                break;
            case "CO2":
                pos = new Vector3(0.22f, 1.322058f, 0.739f);
                break;
            case "O2":
                pos = new Vector3(0.149f, 1.35152f, 0.739f);
                break;
            case "H2O":
                pos = new Vector3(-0.107f, 1.324f, 0.739f);
                break;
            case "CH2O":
                pos = new Vector3(1.189f, 1.192f, 0.739f);
                break;
            case "H":
                pos = new Vector3(1.222f, 0.57f, 0.739f);
                break;
            case "pi":
                pos = new Vector3(0.765f, 0.254f, 0.739f);
                break;
            case "ADP":
                pos = new Vector3(0.351f, 0.176f, 0.739f);
                break;
            case "ATP":
                pos = new Vector3(-0.019f, 0.178f, 0.739f);
                break;
            case "mei":
                pos = new Vector3(1.168f, 1.014f, 0.739f);
                break;
            case "C5":
                pos = new Vector3(0.674f, 1.195f, 0.739f);
                break;
            case "lnt":
                pos = new Vector3(1.022f, 1.156f, 0.744f);
                break;
            case "C3":
                pos = new Vector3(-1.037f, 0.816f, 0.739f);
                break;
            default:
                break;
        }
        transform.localPosition = pos;
    }
}
