using DG.Tweening;
using TMPro;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    [SerializeField] public float duration;

    [SerializeField] private TextMeshProUGUI textObject;
    [SerializeField] private CanvasGroup mask;


    // Start is called before the first frame update
    void Start()
    {
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        mask.DOFade(1, duration / 500).OnComplete(() => mask.DOFade(0, duration));
        transform.DOMoveY(transform.position.y + 1, duration);
    }

    public void SetText(string text)
    {
        textObject.text = text;
    }
}