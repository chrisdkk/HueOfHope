using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardCostReduction : MonoBehaviour
{
    [SerializeField] private GameObject normalText;
    [SerializeField] private TextMeshProUGUI reductionText;
    [SerializeField] private CardVisual visual;

    public void CheckForReducedCosts()
    {
        if (BattleManager.Instance.reduceCardCostsBy > 0)
        {
            int cost = visual.CardData.apCost - BattleManager.Instance.reduceCardCostsBy;
            cost = cost < 0 ? 0 : cost;
            reductionText.text = cost.ToString();
            reductionText.gameObject.SetActive(true);
            normalText.SetActive(false);
        }
        else
        {
            reductionText.gameObject.SetActive(false);
            normalText.SetActive(true);
        }
    }
}