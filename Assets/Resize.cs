using UnityEngine;
using UnityEngine.UI;

public class Resize : MonoBehaviour
{
    public RectTransform canvasRect;
    public RectTransform rawImageRect;

    void Start()
    {
        // Set anchors to cover the whole canvas
        rawImageRect.anchorMin = Vector2.zero;
        rawImageRect.anchorMax = Vector2.one;
        rawImageRect.pivot = new Vector2(0.5f, 0.5f);

        // Calculate the scale required to fill the canvas
        float scaleX = canvasRect.rect.width / rawImageRect.rect.width;
        float scaleY = canvasRect.rect.height / rawImageRect.rect.height;

        // Apply the scale while maintaining aspect ratio
        float scale = Mathf.Max(scaleX, scaleY);
        rawImageRect.localScale = new Vector3(scale, scale, 1f);
    }
}
