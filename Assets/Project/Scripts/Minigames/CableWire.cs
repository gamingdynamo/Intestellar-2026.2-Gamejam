using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CableWire : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image image;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        rectTransform.pivot = new Vector2(0.0f, 0.5f);
    }

    public void SetColor(Color color)
    {
        if (image == null) image = GetComponent<Image>();
        image.color = color;
    }

    public void UpdateWire(Vector2 startPos, Vector2 endPos)
    {
        rectTransform.position = startPos;

        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);
    }
}