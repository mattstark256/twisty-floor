using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour
{
    public float trackLength;
    public int entryDirection;
    public int exitDirection;
    public float rotation;

    public virtual void PositionVehicleOnRail(Transform vehicle, float railDistance, bool alignedWithTrack)
    {

    }
}
