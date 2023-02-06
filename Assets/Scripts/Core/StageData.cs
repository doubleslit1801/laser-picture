using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData
{
    public GameObject Prefab { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    
    public GameObject ToInstance()
    {
        return GameObject.Instantiate(Prefab, Position, Rotation);
    }
}

public class StageData
{
    public Vector3[] Drawing { get; set; }
    public ObjectData[] Objects { get; set; }
}

[System.Serializable]
public class ObjectDataNew
{
    public string prefab;
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class StageDataNew
{
    public Vector3[] drawing;
    public ObjectDataNew[] objects;
}