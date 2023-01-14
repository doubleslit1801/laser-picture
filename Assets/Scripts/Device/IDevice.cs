using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDevice
{
    void HandleInput(Light light, Vector3 hitPos);
    void HandleInputStop();
}
