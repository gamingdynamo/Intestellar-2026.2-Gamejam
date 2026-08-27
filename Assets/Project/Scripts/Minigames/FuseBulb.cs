using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class FuseBulb : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int fuseRating;

    [SerializeField] private float dragSensitivity = 0.5f;
    [SerializeField] private float requiredProgress = 100f;


    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform trayContainer;
    private int originalSiblingIndex;
    private FuseSlot currentSlot;
    private ArrangeFuses game;

    public float ScrewProgress { get; private set; } = 0f;
    public bool IsFullyTightened => ScrewProgress >= requiredProgress;
    public bool IsFullyUnscrewed => ScrewProgress <= 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        trayContainer = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
    }

    public void Init(ArrangeFuses game, bool startsInserted, FuseSlot slot = null)
    {
        this.game = game;
        currentSlot = slot;

        if (startsInserted)
        {
            ScrewProgress = requiredProgress;
            rectTransform.localEulerAngles = Vector3.zero;
        }
        else
        {
            ScrewProgress = 0f;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentSlot != null)
        {
            if (game.CurrentMode == FuseGameMode.Connect)
            {
                return;
            }

            if (game.CurrentMode == FuseGameMode.Disconnect)
            {
                if (!IsFullyUnscrewed) return;

                currentSlot.ClearSlot();
                currentSlot = null;
            }
        }

        canvasGroup.blocksRaycasts = false;
        transform.SetParent(rectTransform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentSlot == null)
        {
            rectTransform.position = eventData.position;
        }
        else
        {
            HandleScrewing(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (currentSlot == null)
        {
            transform.SetParent(trayContainer);
            transform.SetSiblingIndex(originalSiblingIndex);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    public void SnapToSlot(FuseSlot slot)
    {
        currentSlot = slot;
        transform.SetParent(slot.transform);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void RemoveFromSlot()
    {
        currentSlot = null;
    }

    private void HandleScrewing(PointerEventData eventData)
    {
        if (game.CurrentMode == FuseGameMode.Connect && IsFullyTightened) return;
        if (game.CurrentMode == FuseGameMode.Disconnect && IsFullyUnscrewed) return;

        float rawDelta = (eventData.delta.x - eventData.delta.y) * dragSensitivity;

        if (game.CurrentMode == FuseGameMode.Connect)
        {
            if (rawDelta > 0)
            {
                ScrewProgress = Mathf.Min(ScrewProgress + rawDelta, requiredProgress);
                rectTransform.Rotate(0, 0, -rawDelta * 3f);
            }
        }
        else if (game.CurrentMode == FuseGameMode.Disconnect)
        {
            if (rawDelta < 0)
            {
                ScrewProgress = Mathf.Max(ScrewProgress + rawDelta, 0f);
                rectTransform.Rotate(0, 0, -rawDelta * 3f);
            }
        }
    }
}