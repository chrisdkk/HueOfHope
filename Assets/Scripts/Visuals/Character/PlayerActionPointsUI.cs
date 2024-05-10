using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerActionPointsUI : MonoBehaviour
{
    // Update is called once per frame
    public GameObject[] actionPoints;
    public float rotationSpeed = 1.0f; // Speed of oscillation
    public float rotationAmplitude = 15.0f; // Maximum rotation angle (in degrees)
    public float[] rotationDelays; // Timer to track delay for each object

    private void Start()
    {
        BattleManager.Instance.PlayerScript.OnActionPointChange += UpdateActionPointUI;
        for(int i=0;i<actionPoints.Length;i++)
        {
            RotateWithDelay(actionPoints[i], rotationDelays[i]);
        }
    }

    void RotateWithDelay(GameObject obj, float rotationDelay)
    {
        obj.transform.DOLocalRotate(new Vector3(0, 0, rotationAmplitude), rotationSpeed)
            .SetDelay(rotationDelay)
            .SetLoops(-1, LoopType.Yoyo); // Yoyo to make the rotation back and forth
    }

    private void UpdateActionPointUI()
    {
        int currentActionPoints = BattleManager.Instance.PlayerScript.CurrentActionPoints;

        for (int i = 0; i < actionPoints.Length; i++)
        {
            Image image = actionPoints[i].GetComponent<Image>(); // Get Image component
            if (image != null)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b,
                    i < currentActionPoints ? 1.0f : 0.2f); // Set alpha based on condition
            }
        }
    }
}