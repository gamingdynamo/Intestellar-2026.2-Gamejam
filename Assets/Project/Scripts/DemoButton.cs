using UnityEngine;
using UnityEngine.UI;

public class DemoButton : MonoBehaviour
{
    public Button myButton;

    void Start()
    {
        myButton.onClick.AddListener(OnButtonClicked);
        Debug.Log("Start setup for button");
    }

    void OnButtonClicked()
    {
        Debug.Log("Button Clicked ");

        MinigameController.Instance.StartMinigame(MinigameType.RouteCables, (success) =>
        {
            Debug.Log("Success callback - " + success);
        });
    }
}