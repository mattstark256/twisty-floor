using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailCurve : Rail
{
    public override void PositionVehicleOnRail(Transform vehicle, float railDistance, bool alignedWithTrack)
    {
        base.PositionVehicleOnRail(vehicle, railDistance, alignedWithTrack);

        vehicle.SetParent(transform);
        vehicle.localPosition = Quaternion.Euler(0, rotation, 0) * new Vector3(0.5f - Mathf.Cos(railDistance * 2) * 0.5f, 0, -0.5f + Mathf.Sin(railDistance * 2) * 0.5f);
        vehicle.localRotation = Quaternion.Euler(0, rotation + Mathf.Rad2Deg * railDistance * 2, 0);
        if (!alignedWithTrack) { vehicle.localRotation *= Quaternion.Euler(0, 180, 0); }
    }
}
