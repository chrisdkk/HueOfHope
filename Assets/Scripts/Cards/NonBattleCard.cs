using UnityEngine;
using UnityEngine.EventSystems;

public class NonBattleCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public float increasedScale;
    private Vector3 baseScale;
    private CardVisual cardVisual;
    
    // Start is called before the first frame update
    void Start()
    {
        baseScale = transform.localScale;
        cardVisual = GetComponent<CardVisual>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale *= increasedScale;
        FindObjectOfType<AudioManager>().Play("Hover3");
        cardVisual.ToggleDetails();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = baseScale;
        cardVisual.ToggleDetails();
    }
}
