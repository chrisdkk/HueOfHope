using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    private GameObject target;

    public delegate void TargetClickedEventHandler(GameObject clickedObject, bool isPlayer);

    public event TargetClickedEventHandler OnClick;

    private void Start()
    {
        target = transform.Find("Target").gameObject;
        target.SetActive(false);
    }

    public void SetFocus(bool isFocused)
    {
        target.SetActive(isFocused);
    }

    private void OnMouseDown()
    {
        OnClick?.Invoke(gameObject, isPlayer);
    }
}
