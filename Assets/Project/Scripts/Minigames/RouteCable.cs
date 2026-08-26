using System;
using System.Collections.Generic;
using UnityEngine;

public class RouteCable : BaseMinigame
{
    [SerializeField] private List<CablePin> leftPins;
    [SerializeField] private List<CablePin> rightPins;
    [SerializeField] private Transform wireContainer;

    public Transform WireContainer => wireContainer != null ? wireContainer : transform;


    public override void Launch(Action<bool> onComplete)
    {

        base.Launch(onComplete);
        SetupGame();
    }

    private void SetupGame()
    {

        foreach (Transform child in WireContainer)
        {
            Destroy(child.gameObject);
        }


        foreach (var pin in leftPins) pin.Init(this);
        foreach (var pin in rightPins) pin.Init(this);

        ShuffleRightPins();
        gameObject.SetActive(true);
    }

    private void ShuffleRightPins()
    {

        for (int i = 0; i < rightPins.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, rightPins.Count);
            rightPins[i].transform.SetSiblingIndex(randomIndex);
        }
    }

    public void CheckWinCondition()
    {
        foreach (var pin in leftPins)
        {
            if (!pin.IsConnected) return;
        }

        Finish(true);
        gameObject.SetActive(false);
    }

    public void OnCloseButtonClicked()
    {
        Finish(false);
        gameObject.SetActive(false);
    }
}