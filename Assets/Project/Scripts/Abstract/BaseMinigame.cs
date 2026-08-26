using System;
using UnityEngine;

public abstract class BaseMinigame : MonoBehaviour, IMinigame
{
    [SerializeField] private MinigameType minigameType;
    public MinigameType Type => minigameType;

    protected Action<bool> onCompleteCallback;

    public virtual void Abort()
    {
        gameObject.SetActive(false);
        onCompleteCallback = null;
    }

    public virtual void Launch(Action<bool> onComplete)
    {
        gameObject.SetActive(true);
        onCompleteCallback = onComplete;
    }

    protected void Finish(bool success)
    {
        gameObject.SetActive(false);
        onCompleteCallback?.Invoke(success);
        onCompleteCallback = null;
    }
}