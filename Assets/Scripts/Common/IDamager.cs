using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamager
{
    float GetDamage();
    Vector2 GetKnockback();
}
