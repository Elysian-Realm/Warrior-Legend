using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PhysicsCheck physicsCheck;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currentSpeed;
    public float hurtForce;
    [HideInInspector] public float faceDir;
    [HideInInspector] public Transform attacker;
    [HideInInspector] public Vector3 spawnPoint;

    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("计时器")]
    public float waitTime;
    [HideInInspector] public float waitTimeCounter;
    public bool wait;
    public float lostTime;
    [HideInInspector] public float lostTimeCounter;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;
    [HideInInspector] public BaseState currentState;
    [HideInInspector] public BaseState patrolState;
    [HideInInspector] public BaseState chaseState;
    [HideInInspector] public BaseState skillState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;
        spawnPoint = transform.position;
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void Update()
    {
        faceDir = -transform.localScale.x / Mathf.Abs(transform.localScale.x);
        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        if (!wait && !isHurt && !isDead) Move();
        currentState.PhysicsUpdate();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    public virtual void Move()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PreMove") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Recover"))
            rb.velocity = new Vector2(faceDir * currentSpeed * Time.deltaTime, rb.velocity.y);
    }

    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
            }
        }

        if (!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        else if (FoundPlayer())
        {
            lostTimeCounter = lostTime;
        }
    }

    public virtual bool FoundPlayer()
    {
        return Physics2D.BoxCast((Vector2)transform.position + centerOffset,
        checkSize, 0, new Vector2(faceDir, 0), checkDistance, attackLayer);
    }

    public void SwitchState(BaseState state)
    {
        currentState.OnExit();
        currentState = state;
        currentState.OnEnter(this);
    }

    public virtual Vector3 GetNewPoint()
    {
        return transform.position;
    }

    public void OnTakeDamage(Transform attacker)
    {
        this.attacker = attacker;

        if (attacker.position.x < transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        isHurt = true;
        rb.velocity = Vector2.zero;
        if (!isDead) animator.SetTrigger("hurt");
        Vector2 dir = (transform.position - attacker.position).normalized;
        StartCoroutine(OnHurt(dir));
    }

    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }

    public void OnDie()
    {
        gameObject.layer = 2;
        isDead = true;
        animator.SetBool("dead", true);
    }

    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * (-transform.localScale.x), 0), 0.2f);
    }
}
