using System;
using System.Collections.Generic;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    public static MinigameController Instance { get; private set; }

    private FpsController player;
    [SerializeField] private List<BaseMinigame> minigames;

    private Dictionary<MinigameType, IMinigame> minigameRegistry;
    private IMinigame activeMinigame;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        minigameRegistry = new Dictionary<MinigameType, IMinigame>();
        foreach (var game in minigames)
        {
            if (game != null && !minigameRegistry.ContainsKey(game.Type))
            {
                minigameRegistry.Add(game.Type, game);
                // This is to make sure minigame are not shown by default
                game.gameObject.SetActive(false);
            }
        }
    }

    public void Start()
    {
        player = FpsController.GetFpsControllerRefrence();
        Debug.Log("Player found - " + player != null);
        // TODO - Remove later, this is just for demo scene
        if (player) player.SetFreeze(true);
    }

    public void StartMinigame(MinigameType type, Action<bool> callback = null, object payload = null)
    {
        if (!minigameRegistry.TryGetValue(type, out var minigame))
        {
            Debug.LogError($"Minigame {type} not found! Assign it in to minigame controller first.");
        }

        activeMinigame = minigame;

        if (player) player.SetFreeze(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Launching Minigame");

        activeMinigame.Launch((success) =>
        {
            EndMinigame();
            callback?.Invoke(success);
        }, payload);
    }

    private void EndMinigame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (player) player.SetFreeze(false);
        activeMinigame = null;
    }
}
