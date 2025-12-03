using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount);
    void ApplyKnockback(Vector2 force);
}
