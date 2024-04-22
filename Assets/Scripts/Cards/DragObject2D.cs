using System;
using UnityEngine;

public class DragObject2D: MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 planeNormal;

    private bool dragging;

    private void Update()
    {
        if (dragging)
        {
            // Cast a ray from the cursor position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
             
            // Define the plane
            Plane plane = new Plane(planeNormal, Vector3.zero);
     
            if (plane.Raycast(ray, out var distanceToPlane))
            {
                // Calculate the intersection point
                Vector3 intersectionPoint = ray.GetPoint(distanceToPlane);
                transform.position = intersectionPoint;
            }
        }
    }

    private void OnMouseDrag()
    {
        dragging = true;
        Debug.Log("Hello");
    }

    private void OnMouseUp()
    {
        dragging = false;
    }
}