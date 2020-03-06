
// CompilingFinishedCallback.cs
using GCSeries;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class CompilingFinishedCallback
{

   static void ResetAttrCallback()
    {
        MRSystem mRSystem = GameObject.FindObjectOfType<MRSystem>();
        mRSystem.isAutoSlant = false;
        mRSystem.ViewerScale = 5;
     
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        ResetAttrCallback();
#if !UNITY_EDITOR
        Debug.Log("脚本reload");
#endif
    }

    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        ResetAttrCallback();
        Debug.Log("处理风波");
    }
}