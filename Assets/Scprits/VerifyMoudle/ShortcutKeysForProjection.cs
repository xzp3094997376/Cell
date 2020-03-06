using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using huang.module.ui.settingui;

/// <summary>
/// 快捷键切换投屏
/// </summary>
public class ShortcutKeysForProjection : MonoBehaviour
{
    UISetting uISetting;

    UISetting GUISetting
    {
        get
        {
            if (uISetting == null) uISetting = FindObjectOfType<UISetting>();
            return uISetting;
        }

    }
    private void Update()
    {

        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {

            if (Input.GetKeyDown(KeyCode.Z))
            {
                //Alt+Z 不投屏
                StartCoroutine(GUISetting.ShortcutKeySetNone());
               

                return;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                //Alt+X VR2D
                StartCoroutine(GUISetting.ShortcutKeySetVR2D());
                return;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                //Alt+C VR3D
                StartCoroutine(GUISetting.ShortcutKeySetVR3D());
                return;
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                //Alt+V AR
                StartCoroutine(GUISetting.ShortcutKeySetAR());
                return;
            }
        }
    }

}
