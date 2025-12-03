using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private MovementCharacteristics stats;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float baseSensitivity = 0.01f;

    private Vector2 currentVelocity;
    private Vector2 velocitySmoothRef;
    private Vector2 inputDelta;

    private Rigidbody2D rb;
    private PlayerController controller;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner && inputReader != null)
        {
            SubscribeToInputReader();
        }
    }

    private void OnEnable()
    {
        if (IsOwner && inputReader != null)
        {
            SubscribeToInputReader();
        }
    }

    void SubscribeToInputReader()
    {
        inputReader.moveEvent += OnMove;
        inputReader.focusEventStarted += OnFocusStart;
        inputReader.focusEventCanceled += OnFocusCanceled;
    }

    private void OnDisable()
    {
        if (IsOwner && inputReader != null)
        {
            inputReader.moveEvent -= OnMove;
            inputReader.focusEventStarted -= OnFocusStart;
            inputReader.focusEventCanceled -= OnFocusCanceled;
        }
    }

    private void Update()
    {
        if (IsOwner)
            Move();
    }

    private void Move()
    {
        var s = controller.GetState() == PlayerState.FocusMode ?
                (stats.fSpeedMultiplier, stats.fMaxSpeed, stats.fDrag, stats.fSmoothTime) :
                (stats.speedMultiplier, stats.maxSpeed, stats.drag, stats.smoothTime);

        ContextMove(s.Item1, s.Item2, s.Item3, s.Item4);
    }

    private void ContextMove(float speedMultiplier, float maxSpeed, float drag, float smoothTime)
    {
        float frameScale = 1f / Time.deltaTime;

        Vector2 targetVelocity = inputDelta * speedMultiplier * frameScale;
        targetVelocity = Vector2.ClampMagnitude(targetVelocity, maxSpeed);

        currentVelocity = Vector2.SmoothDamp(
            currentVelocity,
            targetVelocity,
            ref velocitySmoothRef,
            smoothTime
        );

        if (inputDelta == Vector2.zero)
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, drag * Time.deltaTime);

        Vector2 newPos = rb.position + currentVelocity * baseSensitivity;
        rb.MovePosition(newPos);
    }

    private void OnMove(Vector2 delta) => inputDelta = delta;
    private void OnFocusStart() => controller.SetState(PlayerState.FocusMode);
    private void OnFocusCanceled() => controller.SetState(PlayerState.NormalMode);
}

