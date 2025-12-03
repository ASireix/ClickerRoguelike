using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuState : MonoBehaviour, IMenuState
{
    [SerializeField]
    protected Button menuButton;

    [System.NonSerialized]
    public MenuManager menuManager;

    void Start()
    {
        menuButton.onClick.AddListener(OnButtonClicked);
        OnStart();
    }

    protected virtual void OnStart() { }

    public virtual void Enter(IMenuState previousState)
    {
        throw new System.NotImplementedException();
    }

    public virtual void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Exit(IMenuState nextState)
    {
        throw new System.NotImplementedException();
    }

    void OnButtonClicked()
    {
        menuManager.TransitionTo(this);
    }
}
