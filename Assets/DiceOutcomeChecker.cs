using UnityEngine;

public class DiceOutcomeChecker : MonoBehaviour
{
    private int result = 0;
    public int Result => result;   // ★ これがないとアクセスできない！
    public Rigidbody rb;

    private bool isOut = false;

    public void ResetResult()
    {
        result = 0;
        isOut = false;
    }

    void Update()
    {
        if (isOut) return;

        // 完全停止したら出目判定
        if (GetComponent<Rigidbody>().IsSleeping() && result == 0)
        {
            result = DetectFace();
        }
    }

    int DetectFace()
    {
        // サイコロの各面の方向（ローカル）
        Vector3[] faceDirections =
        {
            Vector3.right,       // 1の面
            Vector3.left,    // 6の面
            Vector3.back,     // 3の面
            Vector3.forward,    // 4の面
            Vector3.up, // 2の面
            Vector3.down    // 5の面
        };

        int[] faceValues = { 1, 6, 3, 4, 2, 5 };

        float maxDot = -1f;
        int bestFace = 1;

        // ワールドの上方向
        Vector3 worldUp = Vector3.up;

        // 一番上を向いている面を探す
        for (int i = 0; i < faceDirections.Length; i++)
        {
            Vector3 dir = transform.TransformDirection(faceDirections[i]);
            float dot = Vector3.Dot(dir, worldUp);

            if (dot > maxDot)
            {
                maxDot = dot;
                bestFace = faceValues[i];
            }
        }

        return bestFace;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOut = true;
            result = -1;
        }
    }
}
