using System;

public interface IMinigame
{
    MinigameType Type { get; }
    void Launch(Action<bool> onComplete, object payload = null);
    void Abort();
}