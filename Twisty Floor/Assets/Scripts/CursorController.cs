using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField]
    Camera playerCamera;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    float reachDistance = 5;

    public void HandleInputs()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, layerMask))
        {
            Interactive interactive = hit.transform.GetComponentInParent<Interactive>();
            if (interactive != null)
            {
                interactive.Hover(hit);
                if (Input.GetButtonDown("Fire1"))
                {
                    interactive.LeftClick(hit);
                }
                if (Input.GetButtonDown("Fire2"))
                {
                    interactive.RightClick(hit);
                }
            }
        }
    }
}
