using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BreakerSwitch : MonoBehaviour
{
    [SerializeField] private Image switchHandleImage;
    [SerializeField] private Sprite switchOnSprite;
    [SerializeField] private Sprite switchOffSprite;

    private Button switchButton;
    public bool IsSwitchOn { get; private set; }
    private ArrangeFuses game;

    private void Awake()
    {
        switchButton = GetComponent<Button>();
    }

    public void Init(ArrangeFuses game, bool startsOn)
    {
        this.game = game;
        IsSwitchOn = startsOn;
        UpdateVisuals();

        switchButton.onClick.RemoveAllListeners();
        switchButton.onClick.AddListener(OnSwitchClicked);
    }

    public void OnSwitchClicked()
    {
        if (game.CurrentMode == FuseGameMode.Connect)
        {
            if (game.AreAllFusesSecured())
            {
                IsSwitchOn = true;
                UpdateVisuals();
                game.CheckGameCompletion();
            }
            else
            {
                Debug.Log("All fuses not secured!");
                // TODO - Show Warning in UI with something that fuses are missing
            }
        }
        else if (game.CurrentMode == FuseGameMode.Disconnect)
        {
            IsSwitchOn = false;
            UpdateVisuals();
            game.CheckGameCompletion();
        }
    }

    private void UpdateVisuals()
    {
        if (switchHandleImage != null)
        {
            switchHandleImage.sprite = IsSwitchOn ? switchOnSprite : switchOffSprite;
        }
    }
}