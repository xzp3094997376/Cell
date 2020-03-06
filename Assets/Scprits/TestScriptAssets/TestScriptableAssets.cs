using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TestScriptableAssets", order = 1)]
public class TestScriptableAssets : ScriptableObject {
    public string prefabName;
    public int numberOfPrefabsToCreate;
    public Vector3[] spawnPoints;
    public string[] desStrs;
}
