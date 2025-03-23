using UnityEngine;

public class OneWayPlatform3D : MonoBehaviour
{
    private Collider platformCollider;
    private PlayerCtrl playerCtrl;

    void Start()
    {
        platformCollider = GetComponent<Collider>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerCtrl = player.GetComponent<PlayerCtrl>();
        }
        else
        {
            Debug.LogError("Player with 'PlayerCtrl' script not found.");
        }
    }

    void Update()
    {
        if (playerCtrl != null && platformCollider != null)
        {
            // Jumping 상태에서는 충돌 해제
            if (playerCtrl.state == PlayerCtrl.PlayerState.Jumping)
            {
                Physics.IgnoreCollision(playerCtrl.GetComponent<Collider>(), platformCollider, true);
            }
            // stopJump가 true이면 충돌 다시 활성화
            else if (playerCtrl.state != PlayerCtrl.PlayerState.Jumping && playerCtrl.isGrounded)
            {
                Physics.IgnoreCollision(playerCtrl.GetComponent<Collider>(), platformCollider, false);
            }
        }
    }

    // ↓ + 점프 시 아래로 내려가는 기능
    public void StartDropThrough()
    {
        if (playerCtrl != null && platformCollider != null)
        {
            Physics.IgnoreCollision(playerCtrl.GetComponent<Collider>(), platformCollider, true);
            Invoke(nameof(EnableCollision), 0.5f); // 일정 시간 후 충돌 다시 활성화
        }
    }

    private void EnableCollision()
    {
        if (playerCtrl != null && platformCollider != null)
        {
            Physics.IgnoreCollision(playerCtrl.GetComponent<Collider>(), platformCollider, false);
        }
    }
}
