using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneController : MonoBehaviour
{
    [SerializeField] private Animator phoneAnimator;

    private bool isOpen;

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.mKey.wasPressedThisFrame)
        {
            TogglePhone();
        }
    }

    private void TogglePhone()
    {
        if (isOpen)
        {
            phoneAnimator.SetTrigger("ClosePhone");
            isOpen = false;
        }
        else
        {
            phoneAnimator.SetTrigger("OpenPhone");
            isOpen = true;
        }
    }
}