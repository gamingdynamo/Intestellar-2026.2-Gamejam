using UnityEngine;
using UnityEngine.Events;

public class InteractionElement : MonoBehaviour
{
    [SerializeField] private string interactionName;
    [SerializeField] private UnityEvent onInteraction;

    public void Interact()
    {
        Debug.Log("Invoked: "+this.interactionName);
        onInteraction?.Invoke();
    }

    public string GetInteractionName()
    {
        return this.interactionName;
    }
}