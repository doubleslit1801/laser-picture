using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectData
{
    public string prefab;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class StageData
{
    public int maxLaser;
    public Vector3[] drawing;
    public ObjectData[] objects;
}