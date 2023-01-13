using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, -transform.right);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(transform.position, -transform.right * hit.distance, Color.red);

            IDevice device = GameManager.Instance.searchDevice(hit.collider);
            device?.Process(ray);
        }
    }
}
