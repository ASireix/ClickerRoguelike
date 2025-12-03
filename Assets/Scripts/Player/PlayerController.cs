using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 10f;

    private float health;
    private PlayerState state;
    private Rigidbody2D rb;
    private bool isDead;

    public PlayerMovement Movement { get; private set; }
    public PlayerAttacks Attack { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Movement = GetComponent<PlayerMovement>();
        Attack = GetComponent<PlayerAttacks>();
        health = maxHealth;
    }

    public void SetState(PlayerState s) => state = s;
    public PlayerState GetState() => state;

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamage(float amount)
    {
        Debug.Log("Player "+gameObject.name+" took dmg");

        health -= amount;
        if (health <= 0)
            Die();
    }

    public void ApplyKnockback(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void Die()
    {
        isDead = true;
        Movement.enabled = false;
        Attack.enabled = false;
        // notify GameManager
    }
}

