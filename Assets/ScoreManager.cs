using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private int baseScore = 100;
    private DiceManager diceManager;
    public int biasPowar;
    public float biaspoint;
    public float fusinmater = 100;
    public string[] rolePatternsName =
    {"ひふみ",
     "しごろ",
     "ぴんぞろ",
     "にぞろ",
     "さんぞろ",
     "よんぞろ",
     "ごぞろ"  ,
     "ろくぞろ",
     "いちぺあ",
     "にぺあ",
     "さんぺあ",
     "よんぺあ",
     "ごぺあ",
     "ろくぺあ",

    };
    public int[,] rolePatterns =
{
    {1,1, 2,1, 3,1},
    {4,1, 5,1, 6,1},
    {1,3,0,0,0,0},
    {2,3,0,0,0,0},
    {3,3,0,0,0,0},
    {4,3,0,0,0,0},
    {5,3,0,0,0,0},
    {6,3,0,0,0,0},
    {1,2,0,0,0,0},
    {2,2,0,0,0,0},
    {3,2,0,0,0,0},
    {4,2,0,0,0,0},
    {5,2,0,0,0,0},
    {6,2,0,0,0,0},
};
    public int[] roleMultipliers =
{
    -2, // ヒフミ
     5, // シゴロ
     10, // ピンゾロ
     3,//2
     3,//3
     -1,//4
     3,//5
     -2,//6
     2,//ワンペア↓
     2,
     2,
     2,
     2,
     2,//------------
};
    public int[] scoremultiplyer =
    new int[14];//各倍率を何回呼び出すか格納するよ
    public int finalScore;
    public float totalMultiplier;
   public void CheckrolePatterns(int[] countResults)
    {

        for (int i = 0; i < 14; i++)//セットを１４こ判定するよ
        {
            int minSetCount = int.MaxValue;

            for (int n = 0; n < 3; n++)
            {
                int faceIndex = 2 * n;//出目の部分を抜き出すよ
                int requireIndex = faceIndex + 1;//何回出たらいいか参照

                int face = rolePatterns[i, faceIndex];
                int required = rolePatterns[i, requireIndex];

                if (face == 0 || required == 0)
                    continue;

                int setCount =
                    countResults[face - 1] / required;

                minSetCount =
                    Mathf.Min(minSetCount, setCount);
            }

            if (minSetCount == int.MaxValue)
                minSetCount = 0;

            scoremultiplyer[i] = minSetCount;//最終的に何セット作れるかが格納されているよ


        }
        CalculateFinalScore();     
    }


    void CalculateFinalScore()
    {
        totalMultiplier = 1f;

        for (int i = 0; i < roleMultipliers.Length; i++)
        {
            Debug.Log(
          rolePatternsName[i]
          + " : "
          + scoremultiplyer[i]
           );
            int setCount = scoremultiplyer[i];

            if (setCount <= 0)
                continue;

            int multiplier = roleMultipliers[i];

            if (multiplier > 0)
            {
                totalMultiplier *= Mathf.Pow(
                    multiplier,
                    setCount
                );
            }
            else
            {
                totalMultiplier /= Mathf.Pow(
                    Mathf.Abs(multiplier),
                    setCount
                );
            }
        }

        finalScore =
            Mathf.RoundToInt(
                baseScore * totalMultiplier
            );
        Debug.Log(
    $"最終スコア : {finalScore}");
        biaspoint = biaspoint + totalMultiplier - 3;//でかい倍率を出すと不信感が増す。倍率がワンペア以下だと減る。
    }
    public void Update()
    { if(biasPowar!=0&&diceManager.actflag)//いかさま検出用スコア
        { biaspoint=biaspoint+biasPowar*Time.deltaTime;}
    }
    public void CheckEvent()
    { }
}
