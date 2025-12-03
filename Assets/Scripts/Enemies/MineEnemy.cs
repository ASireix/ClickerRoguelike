using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineEnemy : Enemy
{
    public override void OnStartServer()
    {
        movement = new MoveTowardPlayer();
    }
}
