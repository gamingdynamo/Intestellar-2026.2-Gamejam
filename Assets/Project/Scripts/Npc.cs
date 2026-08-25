using UnityEngine;
using NaughtyAttributes;

public class Npc : MonoBehaviour
{
    [SerializeField] private string npcName = "unamed npc";
    [SerializeField] private Vector3 spawnlocation = Vector3.zero;

    void Awake()
    {
        InteractionElement interactionElement = this.gameObject.GetComponent<InteractionElement>();
        if ( interactionElement == null)
        {
            interactionElement = this.gameObject.AddComponent<InteractionElement>();
            interactionElement.SetInteractionName(this.npcName);
        }
        
    }

    void SetComponentState<T>(bool state) where T : Component
    {

        T[] childComponents = GetComponentsInChildren<T>(true);
        foreach (T comp in childComponents)
        {
            SetEnabled(comp, state);
        }

        T[] parentComponents = GetComponentsInParent<T>(true);
        foreach (T comp in parentComponents)
        {
            SetEnabled(comp, state);
        }
    }

    private void SetEnabled(Component comp, bool state)
    {
        if (comp == null) return;

        if (comp is Behaviour behaviour)
        {
            behaviour.enabled = state;
        }
        else if (comp is Renderer renderer)
        {
            renderer.enabled = state;
        }
        else if (comp is Collider collider)
        {
            collider.enabled = state;
        }
    }

    void SetVisibility(bool visibility)
    {
        SetComponentState<MeshRenderer>(visibility);
        SetComponentState<Collider>(visibility);
    }

    [Button("Spawn")]
    public void Spawn()
    {
        this.transform.position = this.spawnlocation;
        this.SetVisibility(true);
    }

    [Button("DeSpawn")]
    public void DeSpawn()
    {
        this.SetVisibility(false);
    }

    void Start()
    {
        this.SetVisibility(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere( this.spawnlocation, 0.25f );
    }

    [Button("Set spawn at current location")]
    void SetSpawnAtLocation()
    {
        this.spawnlocation = this.gameObject.transform.position;
    }
}
