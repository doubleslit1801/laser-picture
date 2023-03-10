using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Light
{
    private IDevice targetDevice;

    public Vector3 Origin { get; set; }
    private Vector3 _originDirection;
    private Vector3 OriginDirection
    {
        get => _originDirection;
        set
        {
            _originDirection = value.normalized;
        }
    }
    public LineRenderer Renderer { get; set; }
    public Color LightColor { get; set; }
    private Vector3 _direction;
    public Vector3 Direction
    {
        get => _direction;
        private set
        {
            _direction = value.normalized;
        }
    }

    private static Material lightMaterial = Resources.Load<Material>("Materials/laser");

    public Light(Vector3 origin, Vector3 direction, LineRenderer renderer, Color color)
    {
        Origin = origin;
        OriginDirection = direction;
        Renderer = renderer;
        LightColor = color;
        Renderer.startColor = color;
        Renderer.endColor = color;
        Renderer.widthMultiplier = 0.1f;
        Renderer.material = lightMaterial;
    }

    public void Update(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        OriginDirection = direction;
    }

    public void UpdateColor(Color c)
    {
        LightColor = c;
        Renderer.startColor = c;
        Renderer.endColor = c;
    }

    public void Enable()
    {
        List<Vector3> positions = new List<Vector3>() { Origin };
        var (addPosition, hit) = Ray(Origin, OriginDirection);
        positions.AddRange(addPosition);
        if (hit is null)
        {
            try
            {
                targetDevice?.HandleInputStop(this);
            }
            finally
            {
                targetDevice = null;
            }
        }
        else
        {
            IDevice newTarget = GameManager.Instance.SearchDevice(hit.Value.collider);
            if (targetDevice != newTarget)
            {
                try
                {
                    targetDevice?.HandleInputStop(this);
                }
                finally
                {
                    targetDevice = newTarget;
                }
            }
            targetDevice?.HandleInput(this, hit.Value.point);
        }
        Render(positions);
    }

    private (List<Vector3>, RaycastHit?) Ray(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        List<Blackhole> blackholes = Physics.OverlapSphere(origin + direction * 0.1f, 0.0f, 1 << 7)
            .Select(c => c.GetComponent<Blackhole>())
            .ToList();

        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, (1 << 6) | (1 << 7)) || blackholes.Count > 0)
        {
            if (blackholes.Count > 0 || hit.collider.gameObject.layer == 7) //Blackhole
            {
                List<Vector3> positions = new();
                Vector3 currentPos = origin;
                Vector3 prevPos;
                Vector3 currentDirection = direction;

                do
                {
                    blackholes = Physics.OverlapSphere(currentPos + currentDirection * 0.1f, 0.0f, 1 << 7)
                        .Select(c => c.GetComponent<Blackhole>())
                        .Where(b => b is not null)
                        .ToList();

                    foreach (var b in blackholes)
                    {
                        Vector3 toCenter = b.transform.position - currentPos;
                        if (toCenter.magnitude <= b.InnerRadius)
                        {
                            positions.Add(currentPos);
                            return (positions, null);
                        }
                        currentDirection += 0.005f * Mathf.Pow(b.Radius / toCenter.magnitude, 2) * toCenter.normalized;
                    }
                    currentDirection = currentDirection.normalized;
                    prevPos = currentPos;
                    currentPos += currentDirection * 0.1f;
                    if (Physics.Raycast(prevPos, currentDirection, out hit, 0.1f, 1 << 6))
                    {
                        positions.Add(hit.point);
                        Direction = currentDirection;
                        return (positions, hit);
                    }
                    positions.Add(currentPos);
                } while (blackholes.Count > 0);

                //Blackhole ????????? Device??? ????????? ??????
                var (rayPositions, rayHit) = Ray(currentPos, currentDirection);
                positions.AddRange(rayPositions);
                return (positions, rayHit);
            }
            else //Device
            {
                Direction = direction;
                return (new List<Vector3>() { hit.point }, hit);
            }
        }
        else
        {
            //Device??? Blackhole ????????? ??????
            try
            {
                targetDevice?.HandleInputStop(this);
            }
            finally
            {
                targetDevice = null;
            }
            return (new List<Vector3>() { origin + direction * 100f }, null);
        }
    }

    public void Disable()
    {
        Render(0);
        try
        {
            targetDevice?.HandleInputStop(this);
        }
        finally
        {
            targetDevice = null;
        }
    }

    private void Render(float length = 100.0f)
    {
        if (length == 0)
        {
            Renderer.enabled = false;
        }
        else
        {
            Renderer.enabled = true;
            Renderer.SetPosition(0, Origin);
            Renderer.SetPosition(1, Origin + OriginDirection * length);
        }
    }

    private void Render(Vector3 end)
    {
        Renderer.enabled = true;
        Renderer.SetPosition(0, Origin);
        Renderer.SetPosition(1, end);
    }

    private void Render(List<Vector3> positions)
    {
        Renderer.enabled = true;
        Renderer.positionCount = positions.Count;
        Renderer.SetPositions(positions.ToArray());
    }
}
