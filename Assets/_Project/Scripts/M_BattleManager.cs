using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;

public class M_BattleManager : MonoBehaviour
{
    public O_Player playerA;
    public O_Player playerB;
    public Transform[] pivots;
    public Transform tileParents;
    [HideInInspector] public bool isGameStart;
    private List<Transform> tileListA = new List<Transform>();
    private List<Transform> tileListB = new List<Transform>();
    public CanvasGroup winnerPanel;
    [HideInInspector] public bool isGameEnd;

    void Start()
    {
        AssignTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerJoined()
    {
        //Debug.Log("PlayerJoined");
    }

    public void OnPlayerLeft()
    {

    }

    public void AssignTiles()
    {
        for (int i = 0; i < tileParents.childCount; i++)
        {
            O_Tile currentTile = tileParents.GetChild(i).GetComponent<O_Tile>();
            if (currentTile.belonging == PlayerID.PlayerA) { tileListA.Add(tileParents.GetChild(i));  }
            else { tileListB.Add(tileParents.GetChild(i));  }

                tileParents.GetChild(i).localScale = Vector3.zero;
        }
    }

    public void GeneratePlayer()
    {
        StartCoroutine(InstantiatePlayer(PlayerID.PlayerA));
        StartCoroutine(InstantiatePlayer(PlayerID.PlayerB));
    }

    public IEnumerator InstantiatePlayer(PlayerID targetPlayer)
    {
        List<Transform> tempList = targetPlayer == PlayerID.PlayerA ? tileListA : tileListB;
        for (int i = 0; i < tempList.Count; i++)
        {
            tempList[i].DOScale(Vector3.one, 0.1f);

            if(targetPlayer == PlayerID.PlayerA) M_Audio.PlayOneShotAudio("PopOut 1");
            //M_Audio.PlayOneShotAudio(targetPlayer == PlayerID.PlayerA ? "PopOut 1" : "PopOut 2");
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.5f);

        Transform targetPlayerTrans = targetPlayer == PlayerID.PlayerA ? playerA.transform : playerB.transform;
        Sequence s = DOTween.Sequence();
        s.Append(targetPlayerTrans.DOScale(1.2f, 0.3f));
        s.Append(targetPlayerTrans.DOScale(0.9f, 0.1f));
        s.Append(targetPlayerTrans.DOScale(1f, 0.1f)).OnComplete(() => targetPlayerTrans.GetComponent<Rigidbody>().isKinematic = false);

        yield return new WaitForSeconds(0.6f);
        if (targetPlayer == PlayerID.PlayerB) GameStart();
    }

    void GameStart()
    {
        foreach (Transform tile in tileListA) tile.GetComponent<O_Tile>().BindingPlayer(playerA);
        foreach (Transform tile in tileListB) tile.GetComponent<O_Tile>().BindingPlayer(playerB);
        isGameStart = true;
    }

   public void GameEnd(PlayerID playerOnLose)
    {
        isGameStart = false;
        isGameEnd = true;
        winnerPanel.transform.Find("Light").localScale = playerOnLose == PlayerID.PlayerB ? Vector3.zero : new Vector3(-1, 1, 1);
        winnerPanel.transform.Find("Light").GetChild(0).localScale = playerOnLose == PlayerID.PlayerB ? Vector3.zero : new Vector3(-1, 1, 1);
        DOTween.To(() => winnerPanel.alpha, x => winnerPanel.alpha = x, 1, 1);
        Debug.Log(playerOnLose + " is Lose");
    }
}
