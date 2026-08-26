using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum WireColor { Red, Blue, Yellow, Green }

[RequireComponent(typeof(Image))]
public class CablePin : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public WireColor wireColor;
    public bool isLeft;
    public Color displayColor;

    [SerializeField] private GameObject wirePrefab;

    private Image pinImage;
    private CableWire currentWire;
    private RouteCable gameManager;
    public bool IsConnected { get; private set; }

    private void Awake()
    {
        pinImage = GetComponent<Image>();
        if (pinImage != null) pinImage.color = displayColor;
    }

    public void Init(RouteCable manager)
    {
        gameManager = manager;
        IsConnected = false;
        if (currentWire != null)
        {
            Destroy(currentWire.gameObject);
            currentWire = null;
        }
    }

    public void OnBeginDrag(PointerEventData pointerEvent)
    {
        if (!isLeft || IsConnected) return;

        GameObject wireObj = Instantiate(wirePrefab, gameManager.WireContainer);
        currentWire = wireObj.GetComponent<CableWire>();
        currentWire.SetColor(displayColor);
        currentWire.UpdateWire(transform.position, pointerEvent.position);
    }

    public void OnDrag(PointerEventData pointerEvent)
    {
        Debug.Log("OnDrag " + pointerEvent);
        if (!isLeft || IsConnected || currentWire == null) return;
        currentWire.UpdateWire(transform.position, pointerEvent.position);
    }

    public void OnEndDrag(PointerEventData pointerEvent)
    {
        if (!isLeft || IsConnected) return;

        if (currentWire != null)
        {
            Destroy(currentWire.gameObject);
            currentWire = null;
        }
    }

    public void OnDrop(PointerEventData pointerEvent)
    {
        if (isLeft || IsConnected) return;

        CablePin draggedPin = pointerEvent.pointerDrag.GetComponent<CablePin>();
        Debug.Log("dragged Pin" + draggedPin);
        if (draggedPin != null && draggedPin.isLeft && !draggedPin.IsConnected)
        {
            if (draggedPin.wireColor == wireColor)
            {
                draggedPin.LockConnection(transform.position);
                IsConnected = true;
                gameManager.CheckWinCondition();
            }
        }
    }

    public void LockConnection(Vector3 targetPosition)
    {
        IsConnected = true;
        if (currentWire != null)
        {
            currentWire.UpdateWire(transform.position, targetPosition);
        }
    }
}