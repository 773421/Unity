using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointEvent 
{
    public static bool RayCast(Ray ray,RaycastHit raycastHit)
    {
        return Physics.Raycast(ray, out raycastHit);
    }
}
