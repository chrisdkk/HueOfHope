using UnityEngine;
using UnityEngine.UI;

public class ResizeSpriteToCanvas : MonoBehaviour
{
    public RectTransform canvasRect;
    public RectTransform spriteRect;

    void Start()
    {
        // Set anchors to cover the whole canvas
        spriteRect.anchorMin = Vector2.zero;
        spriteRect.anchorMax = Vector2.one;
        spriteRect.pivot = new Vector2(0.5f, 0.5f);

        // Set sprite size to match canvas size
        spriteRect.sizeDelta = canvasRect.sizeDelta;
    }
}
