using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public IMenuState CurrentState { get; private set; }
    public event Action<IMenuState> stateChanged;
    [SerializeField] MenuState baseState;

    [SerializeField] List<MenuState> states = new List<MenuState>();

    private void Awake()
    {
        if (!states.Contains(baseState))
        {
            states.Add(baseState);
        }

        foreach (MenuState state in states)
        {
            state.menuManager = this;
        }
    }

    private void Start()
    {
        if ((IMenuState)baseState != null)
        {
            Initialize((IMenuState)baseState);
        }
    }

    public void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }

    // set the starting state
    public void Initialize(IMenuState state)
    {
        foreach(MenuState menuState in states)
        {
            menuState.Exit(null);
        }

        CurrentState = state;
        state.Enter(null);

        // notify other objects that state has changed
        stateChanged?.Invoke(state);
    }


    // exit this state and enter another
    public void TransitionTo(IMenuState nextState)
    {
        CurrentState.Exit(nextState);
        nextState.Enter(CurrentState);
        CurrentState = nextState;

        // notify other objects that state has changed
        stateChanged?.Invoke(nextState);
    }


    // allow the StateMachine to update this state
    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdateState();
        }
    }
}
