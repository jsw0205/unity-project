using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    public enum PlayerState { Idle, Walking, Jumping, WallSliding, WallJumping }
    public PlayerState state = PlayerState.Idle;

    public float moveSpeed = 10.0f;
    public float jumpForce = 10.0f;
    public float wallSlideSpeed = 2.0f;
    public float wallJumpForce = 15.0f;
    private bool stopJump = false;

    private Vector2 moveDir;
    private bool canMove = true;
    public bool isGrounded = true;
    private Rigidbody rb;
    private Animator animator;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;

    private PlayerControls controls;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => moveDir = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveDir = Vector2.zero;

        controls.Gameplay.Jump.performed += ctx => TryJump();
        controls.Gameplay.Jump.canceled += ctx => stopJump = true;

        controls.Gameplay.Attack.performed += ctx => FireBullet();
    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    void Start()
    {
        transform.position = GameManager.instance.respawnPosition;
    }

    void Update()
    {
        if (canMove)
        {
            animator.SetBool("isWalking", moveDir.x != 0);
        }

        if (stopJump && rb.velocity.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z);
            stopJump = false;
        }

        if (state == PlayerState.Jumping && rb.velocity.y <= 0 && isGrounded)
        {
            state = PlayerState.Idle;
            animator.SetBool("isJumping", false);
        }

        if (transform.position.y < -10)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        if (canMove && state != PlayerState.WallJumping)
        {
            Move();
        }
    }

    void Move()
    {
        rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, 0);

        if (moveDir.x > 0)
            transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (moveDir.x < 0)
            transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    void TryJump()
    {
        if (state == PlayerState.Jumping) return;

        if (state == PlayerState.WallSliding)
        {
            WallJump();
            return;
        }

        if (isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        state = PlayerState.Jumping;
        isGrounded = false;
        animator.SetBool("isJumping", true);
        animator.SetBool("grounded", false);

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        stopJump = false;
    }

    void WallJump()
    {
        state = PlayerState.WallJumping;
        canMove = false;
        isGrounded = false;
        animator.SetBool("isJumping", true);
        animator.SetBool("grounded", false);

        float wallJumpDir = moveDir.x <= 0 ? 1 : -1;
        moveDir.x = wallJumpDir;

        transform.rotation = Quaternion.Euler(0, wallJumpDir > 0 ? 90 : -90, 0);

        Vector3 jumpDir = new Vector3(wallJumpDir * 3.5f, 5.5f, 0).normalized;

        rb.velocity = Vector3.zero;
        rb.AddForce(jumpDir * wallJumpForce, ForceMode.Impulse);
    }

    void FireBullet()
    {
        if (firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            bulletRb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }
    }

    OneWayPlatform3D GetCurrentPlatform()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Platform"))
                return col.GetComponent<OneWayPlatform3D>();
        }
        return null;
    }

    void Die()
    {
        GameManager.instance.RespawnPlayer(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            state = PlayerState.Idle;
            isGrounded = true;
            canMove = true;
            animator.SetBool("isJumping", false);
            animator.SetBool("grounded", true);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            state = PlayerState.WallSliding;
            rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0);
            animator.SetBool("isJumping", false);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            animator.SetBool("grounded", false);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") && state != PlayerState.WallJumping)
        {
            state = PlayerState.WallSliding;
            rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0);
        }
    }
}
