using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody2D rb;

    [Header("检测参数")]
    public bool isPlayer;
    public Vector2 bottomOffset;
    public Vector2 frontOffset;
    public Vector2 backOffset;
    public float checkRadius;
    public LayerMask groundLayer;

    [Header("状态")]
    public bool isGround;
    public bool touchFrontWall;
    public bool touchBackWall;
    public bool onWall;

    private void Awake()
    {
        if (isPlayer) playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Check();
    }

    private void Update()
    {
        Check();
    }

    public void Check()
    {
        if (onWall)
            isGround = Physics2D.OverlapCircle((Vector2)transform.position +
            Vector2.Scale(bottomOffset, (Vector2)transform.localScale), checkRadius, groundLayer);
        else
            isGround = Physics2D.OverlapCircle((Vector2)transform.position +
            Vector2.Scale(new Vector2(bottomOffset.x, 0), (Vector2)transform.localScale), checkRadius, groundLayer);
        touchFrontWall = Physics2D.OverlapCircle((Vector2)transform.position +
        Vector2.Scale(frontOffset, (Vector2)transform.localScale), checkRadius, groundLayer);
        touchBackWall = Physics2D.OverlapCircle((Vector2)transform.position +
        Vector2.Scale(backOffset, (Vector2)transform.localScale), checkRadius, groundLayer);
        if (isPlayer) onWall = touchFrontWall && rb.velocity.y < 0 && (playerController.inputDirection.x > 0.1 && transform.localScale.x > 0 || playerController.inputDirection.x < -0.1 && transform.localScale.x < 0);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position +
        Vector2.Scale(bottomOffset, (Vector2)transform.localScale), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position +
        Vector2.Scale(frontOffset, (Vector2)transform.localScale), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position +
        Vector2.Scale(backOffset, (Vector2)transform.localScale), checkRadius);
    }
}
