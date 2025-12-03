using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Hurtbox : MonoBehaviour
{
    public IDamageable damageable; // player ou enemy
    [Tooltip("Optional, otherwise get the first Network Object in parent")]
    public NetworkObject netObj;

    private void Awake()
    {
        netObj = GetComponentInParent<NetworkObject>();
        damageable = netObj.GetComponent<IDamageable>();
    }
}
