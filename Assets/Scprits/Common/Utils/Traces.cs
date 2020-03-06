using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class Traces{
	// Get Object Properties
	public static string[] GetProperties(object o){
		List<string> props = new List<string> ();
		PropertyInfo[] pis = o.GetType ().GetProperties ();
		foreach (PropertyInfo pi in pis) {
			props.Add (pi.Name);
		}

		string[] objPNames = props.ToArray ();
		return objPNames;
	}

	// Get Object Properties's Values
	public static void GetPropertiesValues(object o){
		List<string> props = new List<string> ();
		PropertyInfo[] objps = o.GetType ().GetProperties ();
		Type objType = o.GetType ();
		foreach (PropertyInfo objp in objps) {
			string pname = objp.Name;
			PropertyInfo pi = objType.GetProperty(pname);
			Debug.Log ("==>" + objp.Name + "---" + pi.GetValue(o,null));
			//props.Add (pi.GetValue(o,null).ToString());
		}
		//return props.ToArray ();
	}

	// Get Object Properties's Values
	public static void GetFieldValues(object o){
		//List<string> props = new List<string> ();
		FieldInfo[] objps = o.GetType ().GetFields();
		Type objType = o.GetType ();
		foreach (FieldInfo objp in objps) {
			string pname = objp.Name;
			Debug.Log ("==>" + pname + "---" + objp.GetValue(o));
			//props.Add (pi.GetValue(o,null).ToString());
		}
		//return props.ToArray ();
	}
}
