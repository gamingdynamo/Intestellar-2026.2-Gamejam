using UnityEngine;
using NaughtyAttributes;
using System.Collections;
using System;
using TMPro;

public class Npc : MonoBehaviour
{
    [SerializeField] private string npcName = "unamed npc";
    [SerializeField] private Vector3 spawnlocation = Vector3.zero;
    [SerializeField] private TMP_Text dialogText;

    private Coroutine currentTypewriterCoroutine;

    void SetDialogText(string text)
    {
        if (dialogText == null) { return; }
        dialogText.text = text;
    }

    public void SayText(string text, float characterTiming, Action onComplete = null)
    {
        if (dialogText == null) return;

        if (currentTypewriterCoroutine != null)
        {
            StopCoroutine(currentTypewriterCoroutine);
        }

        currentTypewriterCoroutine = StartCoroutine(TypewriterEffectRoutine(text, characterTiming, onComplete));
    }

    private IEnumerator TypewriterEffectRoutine(string textToType, float delay, Action onComplete)
    {
        dialogText.text = "";

        foreach (char c in textToType)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(delay);
        }

        currentTypewriterCoroutine = null;

        // Trigger the second action once typing completes
        onComplete?.Invoke();
    }

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

    [Button("Test Dialogue")]
    public void TestDialogue()
    {
        
        SayText("Hello traveler!", 0.05f, () => {
            SayText("Welcome to our village!", 0.05f);
        });
    }
}
