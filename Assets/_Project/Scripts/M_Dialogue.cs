using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using JetBrains.Annotations;

public class M_Dialogue : MonoBehaviour
{
    public SO_Dialogue dialogueContainer;
    private int dialogueIndex;
    public CanvasGroup dialogueA;
    public CanvasGroup dialogueB;
    public RectTransform playerA;
    public RectTransform playerB;

    private PlayerID currentActiveCharacter;
    private float fadeTime = 0.7f;
    private float moveTime = 0.7f;
    private bool isContinuePermited = true;
    [HideInInspector] public bool isOnConversation = false;

    void Start()
    {
        AssignName();
    }

    void Update()
    {
        
    }

    void AssignName()
    {
        dialogueA.transform.Find("Name").GetComponent<TMP_Text>().text = dialogueContainer.playerAName;
        dialogueB.transform.Find("Name").GetComponent<TMP_Text>().text = dialogueContainer.playerBName;
        playerA.GetComponent<Image>().sprite = M_BattelRepo.instance.playerPortrait[0];
        playerB.GetComponent<Image>().sprite = M_BattelRepo.instance.playerPortrait[1];
    }

    public void TheTalkingBegin()
    {
        isOnConversation = true;
        DOTween.To(() => playerA.anchoredPosition, x => playerA.anchoredPosition = x, new Vector2(0, 0), moveTime);
        DOTween.To(() => playerB.anchoredPosition, x => playerB.anchoredPosition = x, new Vector2(0, 0), moveTime).OnComplete(Talking);
    }

    public void TheTalkingEnd()
    {
        isOnConversation = false;
        DOTween.To(() => playerA.anchoredPosition, x => playerA.anchoredPosition = x, new Vector2(-500, 0), moveTime);
        DOTween.To(() => playerB.anchoredPosition, x => playerB.anchoredPosition = x, new Vector2(500, 0), moveTime).OnComplete(PanelFade);
    }

    void PanelFade()
    {
        CanvasGroup parentGroup = dialogueA.transform.parent.GetComponent<CanvasGroup>();
        DOTween.To(() => parentGroup.alpha, x => parentGroup.alpha = x, 0, 0.4f).OnComplete(FindObjectOfType<M_BattleManager>().GeneratePlayer);
    }

    public void TryTalk(PlayerID from)
    {
        //if (currentActiveCharacter == from) Talking();
        Talking();
    }

    void Talking()
    {
        if (isContinuePermited)
        {
            Debug.Log("Talk");
            isContinuePermited = false;
            if (dialogueContainer.dialogues[dialogueIndex].playerID == PlayerID.PlayerA)
            {
                //if (dialogueA.alpha == 0) CanvasGroupFade(PlayerID.PlayerA, 1, fadeTime);
                //if (dialogueB.alpha == 1) CanvasGroupFade(PlayerID.PlayerB, 0, fadeTime);
                CanvasGroupFade(PlayerID.PlayerA, 1, fadeTime);
                CanvasGroupFade(PlayerID.PlayerB, 0, fadeTime);
            }
            else
            {
                //if (dialogueA.alpha == 1) CanvasGroupFade(PlayerID.PlayerA, 0, fadeTime);
                //if (dialogueB.alpha == 0) CanvasGroupFade(PlayerID.PlayerB, 1, fadeTime);
                CanvasGroupFade(PlayerID.PlayerA, 0, fadeTime);
                CanvasGroupFade(PlayerID.PlayerB, 1, fadeTime);

            }

        }

    }

    void CanvasGroupFade(PlayerID targetPlayer, int targetValue, float time)
    {
        CanvasGroup targetGroup = targetPlayer == PlayerID.PlayerA ? dialogueA : dialogueB;
        TMP_Text targetText = targetGroup.transform.Find("Content").GetComponent<TMP_Text>();

        if (targetValue == 1)
            DOTween.To(() => targetGroup.alpha, x => targetGroup.alpha = x, targetValue, time)
                .OnComplete(() => StartCoroutine(TypeDialogue(targetText, dialogueContainer.dialogues[dialogueIndex].dialogue)));
        else
        {
            DOTween.To(() => targetGroup.alpha, x => targetGroup.alpha = x, targetValue, time).OnComplete(()=>targetText.text = "");
            targetText.transform.parent.Find("Continue").GetComponent<CanvasGroup>().alpha = 0;
        }

        if (targetValue == 1) currentActiveCharacter = targetPlayer;
    }

    IEnumerator TypeDialogue(TMP_Text targetText, string content)
    {
        targetText.text = content;
        targetText.maxVisibleCharacters = 0;
        for (int i = 0; i <= targetText.text.Length; i++)
        {
            targetText.maxVisibleCharacters++;
            yield return new WaitForSeconds(0.02f);
        }
        CanvasGroup continueHint = targetText.transform.parent.Find("Continue").GetComponent<CanvasGroup>();
        continueHint.alpha = 1;
        isContinuePermited = true;
        dialogueIndex++;
        if (dialogueIndex > dialogueContainer.dialogues.Length - 1) TheTalkingEnd();
    }
}
