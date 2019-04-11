using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailStraight : Rail
{
    public override void PositionVehicleOnRail(Transform vehicle, float railDistance, bool alignedWithTrack)
    {
        base.PositionVehicleOnRail(vehicle, railDistance, alignedWithTrack);

        vehicle.SetParent(transform);
        vehicle.localPosition = Quaternion.Euler(0, rotation, 0) * Vector3.forward * (railDistance - 0.5f);
        vehicle.localRotation = Quaternion.Euler(0, rotation, 0);
        if (!alignedWithTrack) { vehicle.localRotation *= Quaternion.Euler(0, 180, 0); }
    }
}
