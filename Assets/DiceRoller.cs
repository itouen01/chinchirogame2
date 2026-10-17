using UnityEngine;
 public enum BiasMode
    {
        None,       // 補正なし（通常）
        Weak,       // 弱い補正
        MiniWeak,
        MiniStrong,
        Strong      // 強い補正
    }

public class DiceRoller : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] public DiceOutcomeChecker checker;


    public bool biasEnabled = false;  // ZキーでONになる
    public int biasFace = 1;          // 出やすくしたい目
    public float biasForce = 50f;     // 補正の強さ
    public BiasMode biasMode = BiasMode.None;

   


     void Update()
     {
        if (checker.Result == 0)   // 判定がまだ出てない
        {
           
            rb.linearDamping = 0.0f;
            rb.angularDamping = 0.0f;
            if (biasMode == BiasMode.Weak)
            {
                biasForce = 15;
                Doikasama();
            }
            if (biasMode == BiasMode.MiniWeak)
            {
                biasForce = 30;
                Doikasama();
            }
            if (biasMode == BiasMode.MiniStrong)
            {
                biasForce = 45;
             Doikasama();
            }
            if (biasMode == BiasMode.Strong)
            {
                biasForce = 60;
              Doikasama();
            }

        }
       
    }
    public void Doikasama()
    {
        Vector3 biasDir = GetBiasDirection(biasFace); // ★ サイコロごとに違う方向
        if (checker.Result == 0)
        {


            rb.AddForce(biasDir * (biasForce * 5f) * Time.deltaTime);
        }


        // ★ 1. 強力な押し付け（面を下にする）
        rb.AddForce(biasDir * (biasForce * 2.0f) * Time.deltaTime);

        // ★ 2. 回転方向も支配する（上になりたい面を回す）
        rb.AddTorque(biasDir * (biasForce * 1.5f) * Time.deltaTime);

        // ★ 3. 横ブレを減らす（インチキ度UP）
        rb.angularDamping = 0.05f;   // 回転が暴れないようにする
        rb.linearDamping = 0.1f;          // 転がりにくくする
        rb.angularDamping = 0.05f;  // 回転を抑える }
    }

    public void Roll()
    {
        Vector3 biasDir = GetBiasDirection(biasFace);

        Vector3 randomDir = Random.onUnitSphere;
       rb.AddForce(randomDir * Random.Range(300, 400));
    
      Vector3 randomTorque = Random.onUnitSphere;
      rb.AddTorque(randomTorque * Random.Range(300, 400));

        if (biasMode == BiasMode.Weak)
        {
            rb.AddForce(biasDir * (biasForce * 0.3f));   // 弱い補正
        }
        else if (biasMode == BiasMode.Strong)
        {
            rb.AddForce(biasDir * biasForce);            // 強い補正
        }

    }

    Vector3 GetBiasDirection(int face)
    {
        switch (face)
        {
            case 1: return transform.TransformDirection(Vector3.down);
            case 2: return transform.TransformDirection(Vector3.back);
            case 3: return transform.TransformDirection(Vector3.left);
            case 4: return transform.TransformDirection(Vector3.right);
            case 5: return transform.TransformDirection(Vector3.forward);
            case 6: return transform.TransformDirection(Vector3.up);
        }
        return Vector3.zero;
    }


    //Vector3 GetBiasDirection(int face)
    //{
   // switch (face)
    //{
    //    case 1: return Vector3.right;
    //    case 2: return Vector3.up;
     //   case 3: return Vector3.back;
     //   case 4: return Vector3.forward;
     //   case 5: return Vector3.down;
      //  case 6: return Vector3.left;
    //}
   // return Vector3.zero;
    //}
    public void TeleportAndReroll(Vector3 pos)
    {   checker.ResetResult(); // 出目リセット

        transform.position = pos;
        transform.rotation = Random.rotation;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Roll();
    }
}
