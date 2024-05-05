using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace ArrowRendering
{
    public class ArrowRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private GameObject arrowHeadPrefab;
        [SerializeField] private int numberOfSegments = 10;
        [SerializeField] private Transform origin;
        private Vector3 startPoint;
        private Vector3 controlPoint1;
        private Vector3 controlPoint2;
        private Vector3 endPoint;
        
        private GameObject[] nodes;
        private GameObject arrowHead;

        private void Start()
        {
            startPoint = origin.position;
            nodes = new GameObject[numberOfSegments];
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                nodes[i] = Instantiate(nodePrefab, transform);
            }
            nodes[^1] = arrowHead = Instantiate(arrowHeadPrefab, transform);
        }

        private void Update()
        {
            // Set target as mouse
            Vector3 mousePosition =  Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endPoint = new Vector3(mousePosition.x, mousePosition.y, startPoint.z);
            controlPoint1 = startPoint + new Vector3((endPoint.x - startPoint.x) * -0.3f, (endPoint.y - startPoint.y) * 0.8f, endPoint.z - startPoint.z);
            controlPoint2 = startPoint + new Vector3((endPoint.x - startPoint.x) * 0.1f, (endPoint.y - startPoint.y) * 1.4f, endPoint.z - startPoint.z);

            Vector3[] points = CalculateCurvePoints();

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].transform.position = new Vector3(points[i].x, points[i].y, startPoint.z);
                nodes[i].transform.rotation = Quaternion.LookRotation(transform.forward, CalculateTangent(points, i));
            }
            nodes[^1].transform.position -= transform.forward;
        }

        private Vector3[] CalculateCurvePoints()
        {
            Vector3[] points = new Vector3[numberOfSegments];
            float step = 1f / (numberOfSegments - 1);

            for (int i = 0; i < numberOfSegments; i++)
            {
                float t = i * step;
                Vector3 point = CalculatePointOnCurve(t);
                points[i] = point;
            }

            return points;
        }

        private Vector3 CalculatePointOnCurve(float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * startPoint;
            p += 3 * uu * t * controlPoint1;
            p += 3 * u * tt * controlPoint2;
            p += ttt * endPoint;

            return p;
        }

        private Vector3 CalculateTangent(Vector3[] points, int index)
        {
            if (index == 0)
                return points[1] - points[index];
            if (index == points.Length - 1)
                return points[index] - points[index - 1];
            
            return (points[index + 1] - points[index - 1]).normalized;
        }
    }
}
