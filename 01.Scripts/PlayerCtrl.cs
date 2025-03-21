using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public enum PlayerState { Idle, Walking, Jumping, WallSliding, WallJumping }
    public PlayerState state = PlayerState.Idle;

    public float moveSpeed = 10.0f;
    public float jumpForce = 10.0f;
    public float wallSlideSpeed = 2.0f;
    public float wallJumpForce = 15.0f;
    private bool stopJump = false;

    private float moveDir;
    private bool canMove = true;
    public bool isGrounded = true;
    private Rigidbody rb;
    private Animator animator;

    public GameObject bulletPrefab; // 총알 프리팹
    public Transform firePoint; // 총알이 발사될 위치
    public float bulletForce = 20f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        transform.position = GameManager.instance.respawnPosition;
    }

    void Update()
    {
        if (canMove)
        {
            moveDir = Input.GetAxis("Horizontal");
            animator.SetBool("isWalking", moveDir != 0);
        }
        if (isGrounded && Input.GetKey(KeyCode.S) && Input.GetButtonDown("Jump"))
        {
            OneWayPlatform3D platform = GetCurrentPlatform();
            if (platform != null)
            {
                platform.StartDropThrough(); // 플랫폼 아래로 내려가기 실행
                return; // 점프 실행을 막음
            }
        }


        if (Input.GetButtonUp("Jump") && state == PlayerState.Jumping)
        {
            stopJump = true;
        }

        if (stopJump && rb.velocity.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z); // 점프 높이 감소
            stopJump = false;
        }

        switch (state)
        {
            case PlayerState.Idle:
            case PlayerState.Walking:
                if (isGrounded && Input.GetButtonDown("Jump"))
                {
                    Jump();
                }
                break;

            case PlayerState.WallSliding:
                if (Input.GetButtonDown("Jump"))
                {
                    WallJump();
                }
                break;
        }

        if (state == PlayerState.Jumping && rb.velocity.y <= 0 && isGrounded)
        {
            state = PlayerState.Idle;
            animator.SetBool("isJumping", false);
        }
        if (Input.GetKeyDown(KeyCode.X)) // 기본 공격 키 입력
        {
            FireBullet();
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
        rb.velocity = new Vector3(moveDir * moveSpeed, rb.velocity.y, 0);

        if (moveDir > 0)
            transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (moveDir < 0)
            transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    void Jump()
    {
        if (state == PlayerState.Jumping) return;

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

        float wallJumpDir = moveDir <= 0 ? 1 : -1;
        moveDir = wallJumpDir;

        transform.rotation = Quaternion.Euler(0, wallJumpDir > 0 ? 90 : -90, 0);

        Vector3 jumpDir = new Vector3(wallJumpDir * 3.5f, 5.5f, 0).normalized;

        rb.velocity = Vector3.zero;
        rb.AddForce(jumpDir * wallJumpForce, ForceMode.Impulse);
    }

    void FireBullet()
    {
        if (firePoint == null)
        {
            Debug.LogError("FirePoint가 설정되지 않았습니다! Player 오브젝트의 FirePoint를 확인하세요.");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb != null)
        {
            bulletRb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("Bullet에 Rigidbody가 없습니다!");
        }
    }

    OneWayPlatform3D GetCurrentPlatform()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Platform"))
            {
                return col.GetComponent<OneWayPlatform3D>();
            }
        }
        return null;
    }
    void Die()
    {
        Debug.Log("플레이어 사망!");
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
            if (state == PlayerState.WallJumping)
            {
                state = PlayerState.WallSliding;
                rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0);
                animator.SetBool("isJumping", false);
            }
            else if (state != PlayerState.WallJumping)
            {
                state = PlayerState.WallSliding;
                rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0);
                animator.SetBool("isJumping", false);
            }
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
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (state != PlayerState.WallJumping)
            {
                state = PlayerState.WallSliding;
                rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, 0);
            }
        }
    }
}
