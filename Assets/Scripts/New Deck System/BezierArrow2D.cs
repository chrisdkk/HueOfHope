using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BezierArrow2D : MonoBehaviour
{
    [SerializeField] private GameObject arrowHeadPrefab;
    [SerializeField] private GameObject arrowNodePrefab;

    [SerializeField] private int nodeCount;
    [SerializeField] private float scaleFactor;
    
    public Vector3 origin;
    public Vector3 target = Vector3.zero;

    private List<Transform> arrowNodes = new List<Transform>();
    private List<Vector2> controlPoints = new List<Vector2>();

    private readonly List<Vector2> controlPointFactors = 
        new List<Vector2>() {new Vector2(-0.3f, 0.8f), new Vector2(0.1f, 1.4f)};
        
    private void Start()
    {
        for (int i = 0; i < this.nodeCount; i++)
        {
            arrowNodes.Add(Instantiate(arrowNodePrefab, transform).GetComponent<Transform>());
        }
        
        arrowNodes.Add(Instantiate(arrowHeadPrefab, transform).GetComponent<Transform>());

        for (int i = 0; i < 4; i++)
        {
            controlPoints.Add(Vector3.zero);
        }

        Vector2 x = new Vector2(1, 1) * new Vector2(2, 2);
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        target = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 5f));
        // P0 is at the arrow emitter point
        controlPoints[0] = origin;
        // P3 is at the target position
        controlPoints[3] = target;
        
        // P1, P2 determined by P0 and P3
        controlPoints[1] = controlPoints[0] + (controlPoints[3] - controlPoints[0]) * controlPointFactors[0];
        controlPoints[2] = controlPoints[0] + (controlPoints[3] - controlPoints[0]) * controlPointFactors[1];

        for (int i = 0; i < arrowNodes.Count; i++)
        {
            var t = Mathf.Log(1f * i / (arrowNodes.Count - 1) + 1f, 2f);
            
            // Cubic Bezier Curve
            // B(t) = (1-t)^3 * P0 + 3 * (1-t)^2 * t * P1 + 3 * (1-t) * t^2 * P2 * t^3 * P3
            arrowNodes[i].position =
                Mathf.Pow(1 - t, 3) * controlPoints[0] +
                3 * Mathf.Pow(1 - t, 2) * controlPoints[1] +
                controlPoints[2] * controlPoints[3] * (3 * (1 - t) * Mathf.Pow(t, 2) * Mathf.Pow(t, 3));

            // Calculate rotation for each arrow node
            if (i > 0)
            {
                var euler = new Vector3(0, 0,
                    Vector2.SignedAngle(Vector2.up, arrowNodes[i].position - arrowNodes[i - 1].position));
                arrowNodes[i].rotation = Quaternion.Euler(euler);
            }
            
            // Calculate scale for each arrow node
            var scale = scaleFactor * (1f - 0.03f * (arrowNodes.Count - 1 - i));
            arrowNodes[i].localScale = new Vector3(scale, scale, 1f);
        }

        this.arrowNodes[0].transform.rotation = arrowNodes[1].transform.rotation;
    }
}
