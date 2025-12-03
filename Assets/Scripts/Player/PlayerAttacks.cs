using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttacks : NetworkBehaviour, IDamager
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private AttackCharacteristics stats;
    [SerializeField] private Transform clickPosition;
    [SerializeField] private LayerMask hurtboxLayer;

    private float lastClickTime = 0f;
    [SerializeField] private float doubleClickThreshold = 0.25f;

    private float clickCooldown;
    bool canClick = true;
    bool isClicking;
    bool isScanning;

    private Vector2 scanStartPosition;
    private Vector2 scanEndPosition;

    [SerializeField]
    private Transform box;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    float spriteWidth;
    float spriteHeight;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            clickCooldown = 0;
            if (inputReader != null)
            {
                SubscribeToInputReader();
            }

            spriteWidth = spriteRenderer.bounds.size.x;
            spriteHeight = spriteRenderer.bounds.size.y;
        }
    }

    void Update()
    {
        if (!IsOwner)
            return;

        clickCooldown += Time.deltaTime;
        if (clickCooldown >= 1f / stats.clickSpeed)
        {
            canClick = true;
        }

        if (isClicking)
            DoClick();

        if (isScanning)
            UpdateScanBox();
    }

    void DoClick()
    {
        if (canClick)
        {
            clickCooldown = 0;
            canClick = false;

            Vector2 center = clickPosition ? clickPosition.position : transform.position;

            float range = stats.clickRange;

            Collider2D[] hits = Physics2D.OverlapCircleAll(center, range, hurtboxLayer);

            if (hits.Length == 0)
                return;

            foreach (Collider2D hit in hits)
            {
                Hurtbox hurtbox = hit.GetComponent<Hurtbox>();
                if (hurtbox != null)
                {
                    IDamageable dmg = hurtbox.netObj.GetComponent<IDamageable>();
                    if (dmg != null)
                        dmg.TakeDamage(GetDamage());
                }

                IInteractable interactable = hit.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    if (Time.time - lastClickTime <= doubleClickThreshold)
                    {
                        // DOUBLE CLICK
                        interactable.Interact();
                        lastClickTime = 0f; // reset clean
                    }
                    else
                    {
                        // FIRST CLICK
                        lastClickTime = Time.time;
                    }
                }
            }
        }
    }

    void OnClick() => isClicking = true;

    void OnCancelClick() => isClicking = false;

    void OnScan()
    {
        scanStartPosition = clickPosition.position;
        isScanning = true;
        box.gameObject.SetActive(true);
    }

    void OnCancelScan()
    {
        isScanning = false;

        // calcul final
        Vector2 min = Vector2.Min(scanStartPosition, scanEndPosition);
        Vector2 max = Vector2.Max(scanStartPosition, scanEndPosition);

        box.gameObject.SetActive(false);

        // recup tous les colliders dans un AABB
        Collider2D[] hits = Physics2D.OverlapAreaAll(min, max, hurtboxLayer);

        if (hits.Length == 0)
            return;

        foreach (Collider2D hit in hits)
        {
            Hurtbox hurtbox = hit.GetComponent<Hurtbox>();
            if (hurtbox != null)
            {
                IDamageable dmg = hurtbox.netObj.GetComponent<IDamageable>();
                if (dmg != null)
                    dmg.TakeDamage(GetDamage());
            }
        }
    }

    void UpdateScanBox()
    {
        scanEndPosition = clickPosition.position;

        Vector2 min = Vector2.Min(scanStartPosition, scanEndPosition);
        Vector2 max = Vector2.Max(scanStartPosition, scanEndPosition);
        Vector2 size = max - min;

        Vector2 center = (min + max) * 0.5f;

        // Position du parent
        box.position = center;

        // Nouvelle taille SLICED (pas scale !!)
        spriteRenderer.size = size;
    }

    #region Inputs Subscriptions
    private void OnEnable()
    {
        if (IsOwner && inputReader != null)
        {
            SubscribeToInputReader();
        }
    }

    void SubscribeToInputReader()
    {
        inputReader.clickStartedEvent += OnClick;
        inputReader.clickEndEvent += OnCancelClick;
        inputReader.scanEventStarted += OnScan;
        inputReader.scanEventCanceled += OnCancelScan;
    }

    private void OnDisable()
    {
        if (IsOwner && inputReader != null)
        {
            inputReader.clickStartedEvent -= OnClick;
            inputReader.clickEndEvent -= OnCancelClick;
            inputReader.scanEventStarted -= OnScan;
            inputReader.scanEventCanceled -= OnCancelScan;
        }
    }

    #endregion
    public float GetDamage()
    {
        return stats.clickDamage;
    }

    public Vector2 GetKnockback()
    {
        return Vector2.one * stats.clickKnockback;
    }
}
