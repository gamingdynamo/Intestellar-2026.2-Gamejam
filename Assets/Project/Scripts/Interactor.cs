using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private LayerMask interactionLayer;

    void CastInteractionRay()
    {
        interactionText.text = "";

        RaycastHit hit;
        bool intersected = Physics.Raycast(transform.position, this.gameObject.transform.forward, out hit, 3.0f, interactionLayer );
        if ( intersected == false){ return; }

        InteractionElement interactionElement = hit.collider.gameObject.GetComponent<InteractionElement>();
        if ( interactionElement == null ){ return; }

        string name = interactionElement.GetInteractionName();
        interactionText.text = name;

        if (Mouse.current == null){ return; }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            interactionElement.Interact();
        }
        
    }

    void Update()
    {
        this.CastInteractionRay();
    }
}
