using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ArrowRenderer : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private int nodeCount = 5;
    [SerializeField] private float scaleFactor;
    [SerializeField] private Transform origin;
    private Vector3 target;

    private Transform[] nodes;

    private void Start()
    {
        nodes = new Transform[nodeCount];
        for (int i = 0; i < nodeCount; i++)
        {
            GameObject node = Instantiate(nodePrefab, Vector3.zero, Quaternion.identity, transform);
            nodes[i] = node.GetComponent<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (origin != null)
        {
            Vector3 mousePos = Input.mousePosition;
            target = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, origin.localPosition.z));
            
            Vector3 direction = target - origin.position;
            float zAngle = Vector2.SignedAngle(Vector2.up, direction);
            Quaternion rotation = Quaternion.Euler(0, 0, zAngle);

            for (int i = 0; i < nodes.Length; i++)
            {
                Transform node = nodes[i];
                float scale = scaleFactor * (1f - 0.03f * (nodes.Length - 1 - i));
                node.localScale = new Vector3(scale, scale, scale);
                node.localRotation = rotation;
                node.position = origin.position + direction * ((float) i / nodes.Length);
            }
        }
    }
}
