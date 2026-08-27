using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FuseSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int requiredRating;

    [SerializeField] private Color validHoverColor = new(0.2f, 1f, 0.2f, 0.4f);
    [SerializeField] private Color invalidHoverColor = new(1f, 0.2f, 0.2f, 0.4f);

    private Image slotImage;
    private Color originalColor;
    public FuseBulb InsertedBulb { get; private set; }

    private void Awake()
    {
        slotImage = GetComponent<Image>();
        originalColor = slotImage.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || InsertedBulb != null) return;

        if (eventData.pointerDrag.TryGetComponent<FuseBulb>(out var draggedBulb))
        {
            slotImage.color = draggedBulb.fuseRating == requiredRating ? validHoverColor : invalidHoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slotImage.color = originalColor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (InsertedBulb != null) return;

        FuseBulb bulb = eventData.pointerDrag.GetComponent<FuseBulb>();
        if (bulb != null && bulb.fuseRating == requiredRating)
        {
            InsertedBulb = bulb;
            bulb.SnapToSlot(this);
        }
    }

    public void ClearSlot()
    {
        if (InsertedBulb != null)
        {
            InsertedBulb.RemoveFromSlot();
            InsertedBulb = null;
        }
    }

    public bool IsValidAndTightened()
    {
        return InsertedBulb != null && InsertedBulb.IsFullyTightened;
    }


}