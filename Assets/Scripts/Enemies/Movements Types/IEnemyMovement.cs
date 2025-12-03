using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMovement
{
    void TickMovement(Enemy enemy, Rigidbody2D rb);
}
