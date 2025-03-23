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
            // Jumping ���¿����� �浹 ����
            if (playerCtrl.state == PlayerCtrl.PlayerState.Jumping)
            {
                Physics.IgnoreCollision(playerCtrl.GetComponent<Collider>(), platformCollider, true);
            }
            // stopJump�� true�̸� �浹 �ٽ� Ȱ��ȭ
            else if (playerCtrl.state != PlayerCtrl.PlayerState.Jumping && playerCtrl.isGrounded)
            {
                Physics.IgnoreCollision(playerCtrl.GetComponent<Collider>(), platformCollider, false);
            }
        }
    }

    // �� + ���� �� �Ʒ��� �������� ���
    public void StartDropThrough()
    {
        if (playerCtrl != null && platformCollider != null)
        {
            Physics.IgnoreCollision(playerCtrl.GetComponent<Collider>(), platformCollider, true);
            Invoke(nameof(EnableCollision), 0.5f); // ���� �ð� �� �浹 �ٽ� Ȱ��ȭ
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
