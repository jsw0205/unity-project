using UnityEngine;

public class LadderGizmos : MonoBehaviour
{
    public float width = 1f;  // 사다리 너비
    public float height = 5f; // 사다리 높이
    public Color gizmoColor = Color.green;

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // 박스 형태로 사다리 영역 표시
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0.1f));
    }
}
