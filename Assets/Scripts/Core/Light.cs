using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
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

    public Light(Vector3 origin, Vector3 direction, LineRenderer renderer, Color color)
    {
        Origin = origin;
        OriginDirection = direction;
        Renderer = renderer;
        LightColor = color;
    }

    public void Update(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        OriginDirection = direction;
    }

    public void Enable()
    {
        List<Vector3> positions = new List<Vector3>() { Origin };
        positions.AddRange(Ray(Origin, OriginDirection));
        Render(positions);
    }

    private List<Vector3> Ray(Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;
        List<Blackhole> blackholes = Physics.OverlapSphere(Origin + Direction * 0.1f, 0.0f, 1 << 7)
            .Select(c => c.GetComponent<Blackhole>())
            .ToList();

        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, (1 << 6) | (1 << 7)) || blackholes.Count > 0)
        {
            if (hit.collider.gameObject.layer == 7 || blackholes.Count > 0) //Blackhole
            {
                List<Vector3> positions = new();
                Vector3 currentPos = origin;
                Vector3 prevPos;
                Vector3 currentDirection = direction;

                do
                {
                    blackholes = Physics.OverlapSphere(currentPos + currentDirection * 0.1f, 0.0f, 1 << 7)
                        .Select(c => c.GetComponent<Blackhole>())
                        .ToList();

                    foreach (var b in blackholes)
                    {
                        Vector3 toCenter = b.transform.position - currentPos;
                        if (toCenter.magnitude <= b.InnerRadius)
                        {
                            positions.Add(currentPos);
                            return positions;
                        }
                        currentDirection += toCenter.normalized * Mathf.Pow(b.Radius / toCenter.magnitude, 2) * 0.005f;
                    }
                    currentDirection = currentDirection.normalized;
                    prevPos = currentPos;
                    currentPos += currentDirection * 0.1f;
                    if (Physics.Raycast(prevPos, currentDirection, out hit, 0.1f, 1 << 6))
                    {
                        positions.Add(hit.point);
                        IDevice newTarget = GameManager.Instance.SearchDevice(hit.collider);

                        if (targetDevice != newTarget)
                        {
                            MonoBehaviour.print("aaaaaaaaaaa");
                            targetDevice?.HandleInputStop(this);
                            targetDevice = newTarget;
                        }
                        Direction = currentDirection;
                        targetDevice?.HandleInput(this, hit.point);

                        return positions;
                    }
                    positions.Add(currentPos);
                } while (blackholes.Count > 0);

                //Blackhole 안에서 Device를 만나지 않음
                MonoBehaviour.print("bbbbbbbbbbbbbb");
                targetDevice?.HandleInputStop(this);
                targetDevice = null;
                positions.AddRange(Ray(currentPos, currentDirection));
                return positions;
            }
            else //Device
            {
                Direction = direction;
                Render(hit.point);
                Debug.DrawRay(origin, direction * hit.distance, Color.red);

                IDevice newTarget = GameManager.Instance.SearchDevice(hit.collider);
                //target device change
                if (targetDevice != newTarget)
                {
                    MonoBehaviour.print(targetDevice + ">" + newTarget);
                    targetDevice?.HandleInputStop(this);
                    targetDevice = newTarget;
                }
                targetDevice?.HandleInput(this, hit.point);
                return new List<Vector3>() { hit.point };
            }

        }
        else
        {
            //Device와 Blackhole 만나지 않음
            targetDevice?.HandleInputStop(this);
            targetDevice = null;
            return new List<Vector3>() { origin + direction * 100f };
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
