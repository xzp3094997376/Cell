using System.Collections;
using UnityEngine;

/// <summary>
/// 字体的颜色变化
/// </summary>
public class ColorControl : MonoBehaviour
{
    Color color1;
    Color color2;
   
    float recycleTime = 1.2f;
    public Material mat;
    string ColorFrom, ColorTo;
    Color[] colors = new Color[2];
    bool isChangeColor = true;

    Color origin;//记录颜色初始值
    private void Start()
    {
        ColorFrom = "#FFFFFF";
        if (gameObject.name.Contains("XianLiTi"))
        {
            recycleTime = 0.45f;         
            ColorTo = "#84E706";
        }
        else
        {
            ColorTo = "#A748DB";
        }

        ColorUtility.TryParseHtmlString(ColorFrom, out color1);
        ColorUtility.TryParseHtmlString(ColorTo, out color2);
        colors[0] = color1;
        colors[1] = color2;

        origin =color1;
         StartCoroutine("ChangeColor");
    }
    IEnumerator ChangeColorFade()
    {
        while (isChangeColor)
        {
            for (float i = 0; i < 1; i+=0.08f)
            {
                Color c = Color.Lerp(color1, color2, i);
                mat.color = c;
                yield return new WaitForSeconds(recycleTime);                
            }
            Color temp = color1;
            color1 = color2;
            color2 = temp;

        }      
    }

    IEnumerator ChangeColor()
    {
        WaitForSeconds wf = new WaitForSeconds(recycleTime);
        while (isChangeColor)
        {
            mat.color = color1;
            yield return wf;
            Color temp = color1;
            color1 = color2;
            color2 = temp;
        

        }
    }
    private void OnDisable()
    {
        mat.color = origin;
    }
}
