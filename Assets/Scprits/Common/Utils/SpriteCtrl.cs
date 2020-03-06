using System.Collections.Generic;
using UnityEngine;

public class SpriteCtrl : MonoBehaviour
{
    public List<Sprite> spriteList = new List<Sprite>();
    /// <summary>
    /// 返回list长度
    /// </summary>
    public int Length
    {
        get
        {
            return spriteList.Count;
        }
    }

    /// <summary>
    /// 根据索引获取Sprite
    /// </summary>      
    /// <param name="index"></param>
    /// <returns></returns>
    public Sprite this[int index]
    {
        get
        {
            Sprite sp = GetSprite(((SpriteEnum)index).ToString("g"));
            return sp;
        }
    }


    public void Add(Sprite sp)
    {
        Animator ani;
        spriteList.Add(sp);
    }

    /// <summary>
    /// 通过名字获取list中的某一个sp
    /// </summary>
    /// <param name="name"></param>
    public Sprite GetSprite(string name)
    {
        Sprite msp = null;
        Sprite sp = spriteList.Find((Sprite _sp) =>
        {
            if (_sp.name == name)
            {
                msp = _sp;
            }
            return msp;
        });
        return msp;
    }

    public enum SpriteEnum
    {
        C3H4O3 = 1,
        C6H12O6 = 2,
        H = 3,    
        H2O=4,
        mei = 5,
        O2 = 6
    }
}
