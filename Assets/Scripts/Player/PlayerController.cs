using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider2D;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;
    private Character character;
    public Vector2 inputDirection;

    [Header("监听事件")]
    public VoidEventSO newGameLaterEvent;
    public VoidEventSO pauseEvent;
    public VoidEventSO unpauseEvent;
    public VoidEventSO victoryEvent;

    [Header("基本参数")]
    public float speed;
    private float runSpeed;
    private float walkSpeed;
    public float jumpForce;
    public float wallJumpForce;
    public float hurtForce;
    public float slideDistance;
    public float sildeSpeed;
    public float slidePowerCost;
    private Vector2 originalOffset;
    private Vector2 originalSize;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("状态")]
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();
        originalOffset = capsuleCollider2D.offset;
        originalSize = capsuleCollider2D.size;
        inputControl = new PlayerInputControl();
        inputControl.Gameplay.Jump.started += Jump;

        //强制走路
        runSpeed = speed;
        walkSpeed = speed / 2.5f;
        inputControl.Gameplay.WalkButton.performed += _ =>
        {
            speed = walkSpeed;
        };
        inputControl.Gameplay.WalkButton.canceled += _ =>
        {
            speed = runSpeed;
        };

        //攻击
        inputControl.Gameplay.Attack.started += PlayerAttack;

        //滑铲
        inputControl.Gameplay.Slide.started += Slide;
    }

    private void OnEnable()
    {
        inputControl.Enable();
        newGameLaterEvent.OnEventRaised += OnNewGameLaterEvent;
        pauseEvent.OnEventRaised += OnPauseEvent;
        unpauseEvent.OnEventRaised += OnUnpauseEvent;
        victoryEvent.OnEventRaised += OnVictoryEvent;
    }

    private void OnDisable()
    {
        inputControl.Disable();
        newGameLaterEvent.OnEventRaised -= OnNewGameLaterEvent;
        pauseEvent.OnEventRaised -= OnPauseEvent;
        unpauseEvent.OnEventRaised -= OnUnpauseEvent;
        victoryEvent.OnEventRaised -= OnVictoryEvent;
    }

    private void OnVictoryEvent()
    {
        inputControl.Disable();
    }

    private void OnPauseEvent()
    {
        inputControl.Disable();
    }

    private void OnUnpauseEvent()
    {
        inputControl.Enable();
    }

    private void Update()
    {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();
        CheckState();
    }

    private void FixedUpdate()
    {
        if (!isHurt && !isAttack && !isSlide) Move();
    }

    public void Move()
    {
        if (!isCrouch && !wallJump) rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        int faceDir = (int)transform.localScale.x / Mathf.Abs((int)transform.localScale.x);
        if (inputDirection.x > 0) faceDir = 1;
        else if (inputDirection.x < 0) faceDir = -1;
        transform.localScale = new Vector3(faceDir * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        //下蹲
        if (isCrouch = inputDirection.y < -0.5 && physicsCheck.isGround)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            capsuleCollider2D.offset = new Vector2(-0.05f, 0.85f);
            capsuleCollider2D.size = new Vector2(0.7f, 1.7f);
        }
        else
        {
            capsuleCollider2D.offset = originalOffset;
            capsuleCollider2D.size = originalSize;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            //打断滑铲
            if (isSlide)
            {
                StopAllCoroutines();
                isSlide = false;
                gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-transform.localScale.x, 2.5f).normalized * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    private void PlayerAttack(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround)
        {
            playerAnimation.PlayAttack();
            isAttack = true;
        }
    }

    private void Slide(InputAction.CallbackContext context)
    {
        if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;
            var targetPos = new Vector2(transform.position.x + slideDistance * Mathf.Sign(transform.localScale.x), transform.position.y);
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            StartCoroutine(TriggerSlide(targetPos));
            character.Onslide(slidePowerCost);
        }
    }

    private IEnumerator TriggerSlide(Vector2 target)
    {
        do
        {
            yield return null;
            if (physicsCheck.touchFrontWall || !physicsCheck.isGround) break;
            rb.MovePosition(new Vector2(transform.position.x + Mathf.Sign(transform.localScale.x) * sildeSpeed, transform.position.y));
        } while (Mathf.Abs(target.x - transform.position.x) > 0.1f);
        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        inputControl.Gameplay.Disable();
        rb.velocity = Vector2.zero;
        Vector2 dir = (transform.position - attacker.position).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);

        int faceDir = (int)transform.localScale.x;
        if (dir.x > 0) faceDir = -Mathf.Abs(faceDir);
        else if (dir.x < 0) faceDir = Mathf.Abs(faceDir);
        transform.localScale = new Vector3(faceDir, transform.localScale.y, transform.localScale.z);
    }

    private void OnNewGameLaterEvent()
    {
        isDead = false;
        character.currentHealth = character.maxHealth;
        character.currentPower = character.maxPower;
        character.OnHealthChange?.Invoke(character);
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }

    private void CheckState()
    {
        capsuleCollider2D.sharedMaterial = physicsCheck.isGround ? normal : wall;

        if (physicsCheck.onWall) rb.velocity = Vector2.Scale(rb.velocity, new Vector2(1, 0.5f));
        if (wallJump && rb.velocity.y < 0) wallJump = false;
    }
}
