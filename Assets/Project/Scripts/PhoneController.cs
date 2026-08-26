using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneController : MonoBehaviour
{
    [Header("Phone UI")]
    [SerializeField] private GameObject phoneCanvas;


    public bool IsPhoneOpen { get; private set; }

    public event Action OnPhoneOpened;
    public event Action OnPhoneClosed;

    private void Awake()
    {
        SetPhoneVisible(false);
        IsPhoneOpen = false;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            TogglePhone();
        }
    }

    public void TogglePhone()
    {
        if (IsPhoneOpen)
            ClosePhone();
        else
            OpenPhone();
    }

    public void OpenPhone()
    {
        if (IsPhoneOpen)
            return;

        IsPhoneOpen = true;
        SetPhoneVisible(true);

        OnPhoneOpened?.Invoke();
    }

    public void ClosePhone()
    {
        if (!IsPhoneOpen)
            return;

        IsPhoneOpen = false;
        SetPhoneVisible(false);

        OnPhoneClosed?.Invoke();
    }

    private void SetPhoneVisible(bool visible)
    {
        if (phoneCanvas != null)
        {
            phoneCanvas.SetActive(visible);
        }
    }
}