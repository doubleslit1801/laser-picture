using System;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
    private float _radius = 4.0f;
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            var col = GetComponent<CapsuleCollider>();
            transform.localScale = Vector3.one * 0.25f * _radius;
        }
    }
    public float InnerRadius
    {
        get => _radius / 8;
    }
}
