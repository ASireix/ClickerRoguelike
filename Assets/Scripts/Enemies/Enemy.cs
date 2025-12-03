using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : NetworkBehaviour, IDamageable
{
    public EnemyStat stats;
    private readonly SyncVar<float> health = new SyncVar<float>();

    private List<PlayerController> playerControllers;

    private Rigidbody2D rb;

    protected IEnemyMovement movement;
    private Vector2 externalForce;
    private bool isDead;

    [System.NonSerialized]
    public UnityEvent<Enemy> onEnemyDie = new UnityEvent<Enemy>();

    // Start is called before the first frame update

    public override void OnStartClient()
    {
        base.OnStartClient();

        health.SetInitialValues(stats.maxHealth);
        health.OnChange += OnHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitMovement(IEnemyMovement movementModule)
    {
        movement = movementModule;
    }

    void FixedUpdate()
    {
        if (!IsServerStarted) return;

        Vector2 finalVelocity = Vector2.zero;

        // movement logic
        if (movement != null)
        {
            movement.TickMovement(this, rb);
        }

        // Apply knockback / pushback
        if (externalForce.sqrMagnitude > 0.001f)
        {
            rb.AddForce(externalForce, ForceMode2D.Force);
            externalForce = Vector2.Lerp(externalForce, Vector2.zero, 10f * Time.fixedDeltaTime);
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void InitEnemy(List<PlayerController> playerControllers)
    {
        this.playerControllers = playerControllers;
    }

    public Transform GetNearestPlayer(float detectionRadius)
    {
        Debug.DrawLine(transform.position, transform.forward * detectionRadius, Color.red);
        if (playerControllers == null)
            return null;
        Transform res = null;
        float minimalDistance = -1f;
        float distance = 0f;
        foreach(PlayerController player in playerControllers)
        {
            distance = Vector2.Distance(transform.position, player.transform.position);
            if ((distance < minimalDistance || minimalDistance == -1f) && distance < detectionRadius)
            {
                res = player.transform;
            }
        }

        return res;
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }
        Debug.Log("Enemy " + gameObject.name + " took dmg");

        OnHealth(health.Value, health.Value - amount, true);

        if (health.Value <= 0)
        {
            OnHealth(health.Value, 0, true);
            Die();
        }
    }

    private void OnHealth(float prev, float next, bool asServer){}

    [ServerRpc]
    public void Die()
    {
        isDead = true;

        onEnemyDie?.Invoke(this);

        DestroyEnemy();

        Debug.Log("Enemy died");
    }

    [ObserversRpc]
    void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }
}
