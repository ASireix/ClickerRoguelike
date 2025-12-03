using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenuState
{
    public void Enter(IMenuState previousState);

    public void UpdateState();

    public void Exit(IMenuState nextState);
}
