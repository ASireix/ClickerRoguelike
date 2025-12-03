using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardPlayer : IEnemyMovement
{
    Vector2 velocityRef;

    public void TickMovement(Enemy enemy, Rigidbody2D rb)
    {
        var target = enemy.GetNearestPlayer(enemy.stats.detectionRadius);
        if (target == null)
            return;

        Vector2 dir = (target.position - enemy.transform.position).normalized;
        Vector2 desiredVel = dir * enemy.stats.speed;

        Vector2 smoothedVel = Vector2.SmoothDamp(
            rb.velocity,
            desiredVel,
            ref velocityRef,
            enemy.stats.smoothTime
        );

        rb.MovePosition(rb.position + smoothedVel * Time.fixedDeltaTime);
    }
}
