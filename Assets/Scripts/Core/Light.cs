using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light
{
    private IDevice targetDevice;

    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }
    public LineRenderer Renderer { get; set; }
    public Color LightColor { get; set; }

    public Light(Vector3 origin, Vector3 direction, LineRenderer renderer, Color color)
    {
        Origin = origin;
        Direction = direction;
        Renderer = renderer;
        LightColor = color;
    }

    public void Update(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction;
    }

    public void Enable() 
    {
        RaycastHit hit;

        if(Physics.Raycast(Origin, Direction, out hit))
        {
            Render(hit.point);
            Debug.DrawRay(Origin, Direction * hit.distance, Color.red);

            IDevice newTarget = GameManager.Instance.SearchDevice(hit.collider);
            //target device change
            if(targetDevice != newTarget)
            {
                targetDevice?.HandleInputStop(this);
                targetDevice = newTarget;
            }
            targetDevice?.HandleInput(this, hit.point);
        }
        else
        {
            //no target device
            Render();
            targetDevice?.HandleInputStop(this);
            targetDevice = null;
        }
    }

    public void Disable()
    {
        Render(0);
        targetDevice?.HandleInputStop(this);
        targetDevice = null;
    }

    private void Render(float length = 100.0f) 
    {
        if(length == 0)
        {
            Renderer.enabled = false;
        }
        else
        {
            Renderer.enabled = true;
            Renderer.SetPosition(0, Origin);
            Renderer.SetPosition(1, Origin + Direction * length);
        }
    }

    private void Render(Vector3 end)
    {
        Renderer.enabled = true;
        Renderer.SetPosition(0, Origin);
        Renderer.SetPosition(1, end);
    }
}
