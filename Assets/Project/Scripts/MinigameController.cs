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
        player = FindAnyObjectByType<FpsController>();
    }

    public void StartMinigame(MinigameType type, Action<bool> callback = null)
    {
        if (!minigameRegistry.TryGetValue(type, out var minigame))
        {
            Debug.LogError($"Minigame {type} not found! Assign it in to minigame controller first.");
        }

        activeMinigame = minigame;

        if (player) player.enabled = false;
        // I am not changing cursor yet. But we can add it here if needed.
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;

        activeMinigame.Launch((success) =>
        {
            EndMinigame();
            callback?.Invoke(success);
        });
    }

    private void EndMinigame()
    {
        // Same as above about cursor state
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        if (player) player.enabled = true;
        activeMinigame = null;
    }
}
