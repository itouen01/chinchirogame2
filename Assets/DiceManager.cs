using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private Animator ikasamaAnimator;
    public int diceCount = 4;   // ★ ここを変えるだけでOK！

    private List<DiceRoller> diceList = new List<DiceRoller>();
    public int[] diceResults;   // 出目配列
    public int outCount = 0;    // 場外の数
    public TMPro.TextMeshProUGUI resultText;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI scoreboardText;
    public TMPro.TextMeshProUGUI scoreboardsabText;
    public TMPro.TextMeshProUGUI sumscoreText;
    public GameObject resultBackground;
    public int[] countResults = new int[6];
    private bool allDiceProcessed = false;
    [SerializeField]
    private ScoreManager scoreManager;
    private int currentBiasFace = 1;
    private bool resultProcessed = false;
    public int biasFace = 1;
    private int urabiasFace = 1;
    [SerializeField]
    private ImageController imageController;
    public bool actflag = false;//振り直し用のフラグ
    private int sumscore = 0;
    private int rerollnumber;//リロール残数
    public TMPro.TextMeshProUGUI rerollcounterText;
    void Start()
    {
        sumscore = 0;
        diceResults = new int[diceCount];   // ★ 個数に合わせて配列を作る
        SpawnDice(diceCount);
        urabiasFace = 3;
        SetBiasFace(1);
        ikasamaAnimator.SetBool("Doing", false); //アニメーションストップ
        foreach (var dice in diceList)
            dice.biasMode = BiasMode.None;    //開始時いかさまOFF 
        SetPowarImage(0); // 開始時いかさまパワーメータ0(画像上のみ)
        actflag = true;
        imageController.RerollImage();
        rerollnumber = 20;
    }

    void Update()
    {
        if (AllDiceStopped() && !resultProcessed)
        {
            UpdateResults();
            CountDiceFaces();
            scoreManager.CheckrolePatterns(countResults);
            ShowResultsUI();
            resultProcessed = true;
            actflag = false;
            imageController.RerollImage();
            if(scoreManager.biaspoint>=scoreManager.fusinmater)
            {//CheckEvent()
                scoreManager.biaspoint = 0;
            }//biaspoint(いかさま具合)がfusinmator（警戒度）を超えるといかさまと判定される。かいくぐるとその回のスコアが大幅に増し(倍率で)、
            //みつかるとゲームオーバーのチキンレースができる。体験版はある程度回数こなすと強制的に終わる。
        }
        if (rerollnumber > 0) ;
        {
            if (!actflag && Keyboard.current.aKey.wasPressedThisFrame)
            {
                actflag = true;//trueでふれなくなるよ
                imageController.RerollImage();
                RerollAll();
                resultText.gameObject.SetActive(false);// 振り直し時に非表示
                scoreText.gameObject.SetActive(false);
                resultBackground.SetActive(false);
                sumscore = sumscore + scoreManager.finalScore;
                rerollnumber = (rerollnumber-1);
            }

            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                ikasamaAnimator.SetBool("Doing", true);//アニメーション再生
                foreach (var dice in diceList)
                    dice.biasMode = BiasMode.Strong;   // 強化版
                SetPowarImage(4);
                scoreManager.biasPowar = 4;
            }

            if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                ikasamaAnimator.SetBool("Doing", true); //アニメーション再生
                foreach (var dice in diceList)
                    dice.biasMode = BiasMode.MiniStrong;     // ちょい強い版
                SetPowarImage(3);
                scoreManager.biasPowar = 3;
            }
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                ikasamaAnimator.SetBool("Doing", true); //アニメーション再生
                foreach (var dice in diceList)
                    dice.biasMode = BiasMode.MiniWeak;     // ちょい弱い版
                SetPowarImage(2);
                scoreManager.biasPowar = 2;
            }
            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                ikasamaAnimator.SetBool("Doing", true); //アニメーション再生
                foreach (var dice in diceList)
                    dice.biasMode = BiasMode.Weak;     // 弱い版
                SetPowarImage(1);
                scoreManager.biasPowar = 1;
            }

            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                ikasamaAnimator.SetBool("Doing", false); //アニメーションストップ
                foreach (var dice in diceList)
                    dice.biasMode = BiasMode.None;     // 通常
                SetPowarImage(0);
                scoreManager.biasPowar = 0;
            }
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                imageController.panelOpen = !imageController.panelOpen;     //スコア表切り替え
                ShowScoreboardUI();
                ShowScoreboardUIsub();
                SumScoreUI();
                RerollCounterUI();
            }
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                urabiasFace = 3;
                SetBiasFace(1);
            }

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                urabiasFace = 1;
                SetBiasFace(2);
            }

            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                urabiasFace = 5;
                SetBiasFace(3);
            }

            if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                urabiasFace = 2;
                SetBiasFace(4);
            }

            if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                urabiasFace = 6;
                SetBiasFace(5);
            }

            if (Keyboard.current.digit6Key.wasPressedThisFrame)
            {
                urabiasFace = 4;
                SetBiasFace(6);
            }
        }


    }
    void SetBiasFace(int face)
    {
        Debug.Log(imageController);

        biasFace = face;
        imageController.ChangeBiasImage(face);

    }
    void SetPowarImage(int powar)
    {
        Debug.Log(imageController);

        biasFace = powar;
        imageController.ChangePowarImage(powar);

    }

    void SpawnDice(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = spawnArea.position + Random.insideUnitSphere * 1.5f;
            GameObject diceObj = Instantiate(dicePrefab, pos, Random.rotation);
            DiceRoller dice = diceObj.GetComponent<DiceRoller>();
            dice.transform.SetParent(null);



            DiceRoller roller = diceObj.GetComponent<DiceRoller>();
            diceList.Add(roller);

            roller.Roll();
        }
    }

    void RerollAll()
    {

        resultProcessed = false;
        allDiceProcessed = false;
        foreach (var dice in diceList)
        {
            dice.biasFace = urabiasFace;
            dice.TeleportAndReroll(teleportPoint.position);
            Debug.Log(
    "狙い目：" + biasFace
);
        }

        // ★ 振り直し時に出目配列をリセット
        for (int i = 0; i < diceResults.Length; i++)
        {
            diceResults[i] = 0;
        }
    }

    void UpdateResults()
    {
        outCount = 0;

        for (int i = 0; i < diceList.Count; i++)
        {
            int r = diceList[i].checker.Result;
            diceResults[i] = r;

            if (r == -1)
            {
                outCount++;
            }
        }
    }
    bool AllDiceStopped()
    {
        foreach (var dice in diceList)
        {
            if (!dice.checker.rb.IsSleeping())
                return false;

            if (dice.checker.Result == 0)
                return false;
        }

        return true;
    }

    void ShowResultsUI()//画面に結果を表示するよ
    {
        string text = "";

        for (int i = 0; i < 6; i++)
        {
            text += $"{countResults[i]}\n";
        }

        resultText.text = text;
        resultText.gameObject.SetActive(true);
        resultBackground.SetActive(true);
        string resulttext = "";
        resulttext=$"{scoreManager.finalScore}\n";
        scoreText.text = resulttext;
        scoreText.gameObject.SetActive(true);

    }
    void ShowScoreboardUI()//画面に結果を表示するよ
    {
        string text = "";
        text += $" {scoreManager.roleMultipliers[2]}\n";
        text += $"{scoreManager.roleMultipliers[0]}\n";
        text += $" {scoreManager.roleMultipliers[1]}\n";
        text += $"{scoreManager.roleMultipliers[7]}\n";
        text += $"{scoreManager.roleMultipliers[5]}\n";
        text += $" {scoreManager.roleMultipliers[3]}\n";//2のゾロを参照
        text += $" {scoreManager.roleMultipliers[8]}\n";//1のペアを参照
        text += $" 1\n";
        text += $" 1\n";
        scoreboardText.text = text;
        scoreboardText.gameObject.SetActive(true);

    }
    void ShowScoreboardUIsub()//画面に結果を表示するよ
    {
        string text = "";
        text += $"×\n";
        text += $"÷\n";
        text += $"×\n";
        text += $"÷\n";
        text += $"÷\n";
        text += $"×\n"; ;//2のゾロを参照
        text += $"×\n";//1のペアを参照
        text += $"×\n";
        text += $"×\n";


        scoreboardsabText.text = text;
        scoreboardsabText.gameObject.SetActive(true);
    }

    void SumScoreUI()//画面に結果を表示するよ
    {
        string text = "";
        text += $"{sumscore}\n";

        sumscoreText.text = text;
        sumscoreText.gameObject.SetActive(true);
    }
    void RerollCounterUI()
    {
        string text = "";
        text += $"{rerollnumber}\n";
        rerollcounterText.text = text;
        rerollcounterText.gameObject.SetActive(true);
    }
    void CountDiceFaces()
    {
        // まず全部0にリセット
        for (int i = 0; i < 6; i++)
            countResults[i] = 0;

        // 出目を集計
        for (int i = 0; i < diceResults.Length; i++)
        {
            int r = diceResults[i];

            if (r >= 1 && r <= 6)
            {
                countResults[r - 1]++;  // r=1 → index 0 に加算
            }
        }
        Debug.Log(
    $"1:{countResults[0]} " +
    $"2:{countResults[1]} " +
    $"3:{countResults[2]} " +
    $"4:{countResults[3]} " +
    $"5:{countResults[4]} " +
    $"6:{countResults[5]}"
);

    }


}
