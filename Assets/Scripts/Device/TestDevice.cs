using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDevice : MonoBehaviour, IDevice
{
    private bool isInput = false;
    private Ray inputRay;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.registerDevice(GetComponent<Collider>(), this);
    }

    // Update is called once per frame
    void Update()
    {
        if(isInput)
        {
            Debug.DrawRay(transform.position, transform.forward * 10, Color.green);
        }
    }

    public void Process(Ray ray) 
    {
        inputRay = ray;
        isInput = true;
    }
}
