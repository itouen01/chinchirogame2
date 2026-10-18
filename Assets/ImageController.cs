using UnityEngine;
using UnityEngine.UI;

public class ImageController : MonoBehaviour
{//スプライト本体
    [SerializeField] private Image targetImage;
    [SerializeField] private Image powarImage;
    public GameObject rerollImage;
    [SerializeField] public Image hanteiImage;

    //スプライトのアニメーション1枚ずつ
    [SerializeField] private Sprite[] biasSprites;
    [SerializeField] private Sprite[] powarSprites;
    [SerializeField] private Sprite[] hanteiSprites;
    //再生以外の動作系に
    RectTransform panel;
    [SerializeField] RectTransform scorePanel;
    public bool panelOpen = true;
    [SerializeField]
    private DiceManager diceManager;
    void Start()
    {panelOpen = !panelOpen; }



    public void ChangeBiasImage(int biasFace)
    {
        targetImage.sprite = biasSprites[biasFace - 1];
    }
    public void ChangePowarImage(int powar)
    {
        powarImage.sprite = powarSprites[powar];
    }
    public void RerollImage()
    {if(diceManager.actflag)
        { rerollImage.SetActive(false); }
    else
        { rerollImage.SetActive(true); }

    }
    void Update()
    {
        Vector2 hiddenPos = new Vector2(500, 150);
        Vector2 showPos = new Vector2(-100, 150);
        Vector2 target =
            panelOpen ?
            showPos :
            hiddenPos;

        scorePanel.anchoredPosition =
            Vector2.Lerp(
                scorePanel.anchoredPosition,
                target,
                8f * Time.deltaTime
            );
    }
    public void ChangehanteiImage(int score)
    {

            targetImage.sprite = biasSprites[0];
            if (score >=2000 )
            {
            targetImage.sprite = biasSprites[1];
            if (score >= 10000)
                {
                    targetImage.sprite = biasSprites[2];
                }
            }
  
    }
}