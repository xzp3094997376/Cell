using Runing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpliteReset : MonoBehaviour {
    private Vector3[] Positions;
    private Quaternion[] Rotations;
    private Vector3[] Scales;
    private Transform[] childs;
	void Start () {
		
	}

    private void Awake()
    {
        childs = SpliteTool.Instance.GetSpliteParts(transform);
        if (childs != null && childs.Length > 0)
        {
            int len = childs.Length;
            Positions = new Vector3[len];
            Rotations = new Quaternion[len];
            Scales = new Vector3[len];
            int i = 0;
            foreach (Transform tran in childs)
            {
                Positions[i] = tran.localPosition;
                Rotations[i] = tran.localRotation;
                Scales[i] = tran.localScale;
                i++;
            }
        }
    }

    public void Recover()
    {
        if(childs != null)
        {
            int i = 0;
            foreach (Transform tran in childs)
            {
                tran.localPosition = Positions[i];
                tran.localRotation = Rotations[i];
                tran.localScale = Scales[i];
                i++;
            }
        }
    }
}
