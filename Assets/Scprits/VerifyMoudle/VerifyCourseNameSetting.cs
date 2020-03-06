using CertificateVerify;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 每个互动课件在初始的时候需要输入对应课件的证书号
/// 匹配证书一个课件只有对应的证书号才能打开
/// </summary>
public class VerifyCourseNameSetting : MonoBehaviour {

    string courseName = "4/2/7/102294/102304/102305/102327/102376/11986";
	void Awake ()
    {
        CourseName.courseName = courseName;
        Debug.Log("该互动课件的证书名称（不带版本号）" + courseName);
        Debug.Log("互动课件证书匹配规则，课件证书号不带版本号，证书里是带版本号的。");
    }	
}
