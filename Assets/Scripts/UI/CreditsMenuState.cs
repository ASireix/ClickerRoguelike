using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenuState : MenuState
{
    [SerializeField] GameObject creditsScreen;

    protected override void OnStart()
    {

    }

    public override void Enter(IMenuState previousState)
    {
        menuButton.Select();
        creditsScreen.SetActive(true);
    }

    public override void Exit(IMenuState nextState)
    {
        creditsScreen.SetActive(false);
    }

    public override void UpdateState()
    {
        //
    }
}
