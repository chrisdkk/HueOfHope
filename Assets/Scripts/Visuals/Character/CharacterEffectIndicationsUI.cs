using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterEffectIndicationsUI : MonoBehaviour
{
	[SerializeField] private GameObject burnIndicator;
	[SerializeField] private GameObject insightIndicator;
	[SerializeField] private GameObject ignoreBlockIndicator;
    [SerializeField] private GameObject attackDebuffIndicator;
	[SerializeField] private Character characterScript;
	[SerializeField] private float y;
	[SerializeField] private float baseX;
	[SerializeField] private float offsetX;

	void Start()
	{
		burnIndicator.SetActive(false);
		insightIndicator.SetActive(false);
		ignoreBlockIndicator.SetActive(false);
        attackDebuffIndicator.SetActive(false);
		characterScript.CharacterStats.OnStatChange += UpdateIndications;
	}

	public void UpdateIndications()
	{
		float x = baseX;

		UpdateIgnoreBlockIndicator(ref x, offsetX);
		UpdateInsightIndicator(ref x, offsetX);
		UpdateBurnIndicator(ref x, offsetX);
		UpdateAttackDebuffIndicator(ref x, offsetX);
	}

	/*Update the burn indication of the enemy*/
	public void UpdateBurnIndicator(ref float x, float offsetX)
	{
		if (characterScript.CharacterStats.Burn > 0)
		{
			burnIndicator.transform.localPosition = new Vector3(x, y, 0);
			x += offsetX;
			burnIndicator.SetActive(true);
			burnIndicator.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
				characterScript.CharacterStats.Burn.ToString();
		}
		else
		{
			burnIndicator.SetActive(false);
		}
	}

	/*Update the insight indication of the enemy*/
	public void UpdateInsightIndicator(ref float x, float offsetX)
	{
		if (characterScript.CharacterStats.Insight > 0)
		{
			insightIndicator.transform.localPosition = new Vector3(x, y, 0);
			x += offsetX;
			insightIndicator.SetActive(true);
			insightIndicator.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
				characterScript.CharacterStats.Insight.ToString();
		}
		else
		{
			insightIndicator.SetActive(false);
		}
	}

	/*Update the insight indication of the enemy*/
	public void UpdateIgnoreBlockIndicator(ref float x, float offsetX)
	{
		if (characterScript.CharacterStats.IgnoreBlockOnNext > 0)
		{
			ignoreBlockIndicator.transform.localPosition = new Vector3(x, y, 0);
			x += offsetX;
			ignoreBlockIndicator.SetActive(true);
			ignoreBlockIndicator.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
				characterScript.CharacterStats.IgnoreBlockOnNext.ToString();
		}
		else
		{
			ignoreBlockIndicator.SetActive(false);
		}
	}
	
	/*Update the insight indication of the enemy*/
	public void UpdateAttackDebuffIndicator(ref float x, float offsetX)
	{
		if (characterScript.CharacterStats.AttackDebuff > 0)
		{
			attackDebuffIndicator.transform.localPosition = new Vector3(x, y, 0);
			x += offsetX;
			attackDebuffIndicator.SetActive(true);
			attackDebuffIndicator.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = characterScript.CharacterStats.AttackDebuff.ToString();
		}
		else
		{
			attackDebuffIndicator.SetActive(false);
		}
	}
}
