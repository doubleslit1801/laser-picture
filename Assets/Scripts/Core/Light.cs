using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light
{
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

    public void Raycast() 
    {
        RaycastHit hit;

        if(Physics.Raycast(Origin, Direction, out hit))
        {
            Render(hit.point);
            Debug.DrawRay(Origin, Direction * hit.distance, Color.red);
            
            IDevice device = GameManager.Instance.searchDevice(hit.collider);
            device?.HandleInput(this, hit.point);
        }
        else
        {
            Render();
        }
    }

    public void Render(float length = 100.0f) 
    {
        Renderer.SetPosition(0, Origin);
        Renderer.SetPosition(1, Origin + Direction * length);
    }

    public void Render(Vector3 end)
    {
        Renderer.SetPosition(0, Origin);
        Renderer.SetPosition(1, end);
    }
}
