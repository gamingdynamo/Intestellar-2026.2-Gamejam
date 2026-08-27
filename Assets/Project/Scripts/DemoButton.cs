using UnityEngine;
using UnityEngine.UI;

public class DemoButton : MonoBehaviour
{
    [SerializeField] MinigameType minigame;
    public Button myButton;

    void Start()
    {
        myButton.onClick.AddListener(OnButtonClicked);
        Debug.Log("Start setup for button");
    }

    void OnButtonClicked()
    {
        Debug.Log("Button Clicked ");
        if (minigame == MinigameType.ArrangeFuses)
        {
            MinigameController.Instance.StartMinigame(minigame, SuccessCallback, FuseGameMode.Connect);
            return;
        }

        MinigameController.Instance.StartMinigame(minigame, SuccessCallback);
    }

    private void SuccessCallback(bool success)
    {
        Debug.Log("Success callback - " + success);
    }
}