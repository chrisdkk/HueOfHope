using System;
using System.Collections.Generic;
using UnityEngine;

public class NonBattleCard : MonoBehaviour
{
    [SerializeField] private Vector3 increasedScale;
    [SerializeField] private List<Material> hightlightMaterial;
    [SerializeField] private List<Material> defaultMaterial;

    public CardData data;
    private CardVisual visual;
    private Vector3 baseScale;

    public delegate void CardClickedEventHandler(NonBattleCard clickedCard);

    public event CardClickedEventHandler OnClick;

    void Start()
    {
        // baseScale = transform.localScale;
    }

    void OnMouseOver()
    {
        // transform.localScale = increasedScale;
    }

    void OnMouseExit()
    {
        // transform.localScale = baseScale;
    }

    void OnMouseUp()
    {
        OnClick?.Invoke(this);
    }

    public void OnOtherRewardChosen()
    {
        GetComponentInChildren<MeshRenderer>().SetMaterials(defaultMaterial);
    }

    public void OnRewardChosen()
    {
        GetComponentInChildren<MeshRenderer>().SetMaterials(hightlightMaterial);
    }
}