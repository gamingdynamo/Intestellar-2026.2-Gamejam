using System;
using System.Collections.Generic;
using UnityEngine;

public class ArrangeFuses : BaseMinigame
{
    public FuseGameMode CurrentMode = FuseGameMode.Connect;
    [SerializeField] private List<FuseSlot> fuseSlots;
    [SerializeField] private List<FuseBulb> fuseBulbs;
    [SerializeField] private BreakerSwitch breakerSwitch;
    [SerializeField] private Transform fusesContainer;
    [SerializeField] private float completionDelay = 0.6f;
    private bool isFinishing = false;

    public override void Launch(Action<bool> onComplete, object payload = null)
    {
        if (payload is FuseGameMode mode)
        {
            CurrentMode = mode;
        }
        isFinishing = false;
        base.Launch(onComplete, payload);
        SetupBoard();
    }

    public void SetGameMode(FuseGameMode mode)
    {
        CurrentMode = mode;
    }

    private void SetupBoard()
    {
        bool switchStartsOn = CurrentMode == FuseGameMode.Disconnect;
        breakerSwitch.Init(this, switchStartsOn);

        if (CurrentMode == FuseGameMode.Connect)
        {
            for (int i = 0; i < fuseBulbs.Count; i++)
            {
                var bulb = fuseBulbs[i];
                bulb.transform.SetParent(fusesContainer);
                bulb.Init(this, startsInserted: false);
            }

            foreach (var slot in fuseSlots)
            {
                slot.ClearSlot();
            }
        }
        else if (CurrentMode == FuseGameMode.Disconnect)
        {
            for (int i = 0; i < fuseSlots.Count; i++)
            {
                var slot = fuseSlots[i];
                var matchingBulb = fuseBulbs.Find(b => b.fuseRating == slot.requiredRating);

                if (matchingBulb != null)
                {
                    matchingBulb.SnapToSlot(slot);
                    matchingBulb.Init(this, startsInserted: true, slot: slot);
                }
            }
        }
    }

    public bool AreAllFusesSecured()
    {
        foreach (var slot in fuseSlots)
        {
            if (!slot.IsValidAndTightened()) return false;
        }
        return true;
    }

    public void CheckGameCompletion()
    {
        if (isFinishing) return;

        if (CurrentMode == FuseGameMode.Connect)
        {
            if (breakerSwitch.IsSwitchOn && AreAllFusesSecured())
            {
                StartCoroutine(DelayedFinishRoutine(true));
            }
        }
        else if (CurrentMode == FuseGameMode.Disconnect)
        {
            bool allEmpty = true;
            foreach (var slot in fuseSlots)
            {
                if (slot.InsertedBulb != null) allEmpty = false;
            }

            if (!breakerSwitch.IsSwitchOn && allEmpty)
            {
                StartCoroutine(DelayedFinishRoutine(true));
            }
        }
    }

    private IEnumerator<WaitForSeconds> DelayedFinishRoutine(bool success)
    {
        isFinishing = true;
        // TODO - Win SFX and maybe sparks VFX
        yield return new WaitForSeconds(completionDelay);

        Finish(success);
    }
}