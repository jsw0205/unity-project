using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private bool canClimb = false; // 사다리 범위 안에 있는지 여부
    private bool isClimbing = false; // 사다리 오르기 상태
    private Rigidbody playerRb;
    private float climbSpeed = 5.0f;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canClimb = true; // 사다리 범위 안에 있음
            playerRb = other.GetComponent<Rigidbody>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canClimb = false; // 사다리 범위를 벗어나면 climbing 불가능
            isClimbing = false;
            playerRb.useGravity = true;
            playerRb.velocity = Vector3.zero; // 사다리를 벗어날 때 속도 초기화
        }
    }

    void Update()
    {
        if (canClimb) // 사다리 범위 안에 있을 때만 climbing 가능
        {
            float verticalInput = Input.GetAxis("Vertical");

            if (verticalInput != 0) // 위/아래 키를 눌러야 climbing 상태 활성화
            {
                isClimbing = true;
                playerRb.useGravity = false;
                playerRb.velocity = new Vector3(0, verticalInput * climbSpeed, 0);
            }

            if (isClimbing && verticalInput == 0) // 키를 떼면 멈춤
            {
                playerRb.velocity = Vector3.zero;
            }

            if (isClimbing && Input.GetButtonDown("Jump")) // 점프하면 사다리에서 내려옴
            {
                isClimbing = false;
                canClimb = false; // 사다리에서 벗어나도록 설정
                playerRb.useGravity = true;
                playerRb.AddForce(Vector3.up * 3.0f, ForceMode.Impulse); // 살짝 위로 점프
            }
        }
    }
}
