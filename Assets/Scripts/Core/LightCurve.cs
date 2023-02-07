using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightCurve
{
    private IDevice targetDevice;
    private Light virtualLight;

    public Vector3 Origin { get; set; }
    private Vector3 _direction;
    public Vector3 Direction
    {
        get => _direction;
        set
        {
            _direction = value.normalized;
        }
    }
    public LineRenderer Renderer { get; set; }
    public Color LightColor { get; set; }

    public LightCurve(Vector3 origin, Vector3 direction, LineRenderer renderer, Color color)
    {
        Origin = origin;
        Direction = direction;
        Renderer = renderer;
        LightColor = color;
    }

    // Update is called once per frame
    public void Update(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction;
    }

    public (Vector3 origin, Vector3 direction)? Enable()
    {
        RaycastHit hit;

        Vector3 currentPos = Origin;
        Vector3 prevPos;
        Vector3 direction = Direction;
        List<Vector3> positions = new();
        List<Blackhole> blackholes;

        do
        {
            positions.Add(currentPos);
            blackholes = Physics.OverlapSphere(currentPos + direction * 0.1f, 0.0f, 1 << 6)
                .Where(c => c.GetComponent<Blackhole>() != null)
                .Select(c => c.GetComponent<Blackhole>())
                .ToList();
            foreach (var b in blackholes)
            {
                Vector3 toCenter = b.transform.position - currentPos;
                if (toCenter.magnitude <= b.InnerRadius)
                {
                    this.Render(positions);
                    return null;
                }
                direction += toCenter.normalized * Mathf.Pow(b.Radius / toCenter.magnitude, 2) * 0.01f;
            }
            direction = direction.normalized;
            prevPos = currentPos;
            currentPos += direction * 0.1f;
            if (Physics.Raycast(prevPos, direction, out hit, 0.1f, 1 << 6))
            {
                positions.Add(hit.point);
                this.Render(positions);
                IDevice newTarget = GameManager.Instance.SearchDevice(hit.collider);

                if (targetDevice != newTarget)
                {
                    if (virtualLight is null)
                    {
                        virtualLight = new(prevPos, direction, Renderer, LightColor);
                    }
                    targetDevice?.HandleInputStop(virtualLight);
                    targetDevice = newTarget;
                }
                targetDevice?.HandleInput(virtualLight, hit.point);
                return null;
            }
        } while (blackholes.Count > 0);
        positions.Add(currentPos);
        Render(positions);
        return (currentPos, direction);
    }

    public void Disable()
    {
        Renderer.enabled = false;
        targetDevice?.HandleInputStop(virtualLight);
        targetDevice = null;
    }

    public void Render(List<Vector3> positions)
    {
        Renderer.enabled = true;
        Renderer.positionCount = positions.Count;
        Renderer.SetPositions(positions.ToArray());
    }
}
