using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum TileState { Normal, InRange, Selected, OnHold,OnFollow, OnFly, OnStack }
public class O_Tile : MonoBehaviour
{
    public TileState currentState;
    public PlayerID belonging;
    public bool isPermited = true;
    private Transform relatedPlayer;
    private float yAboveValue = 0.5f;

    void Start()
    {
        foreach(O_Player player in FindObjectsOfType<O_Player>())
        {
            if (player.thisPlayer == belonging) relatedPlayer = player.transform; break;
        }
 
    }

    void Update()
    {
        if (currentState == TileState.OnFollow)
        {
         
            transform.position = Vector3.Lerp(transform.position, new Vector3(relatedPlayer.position.x, yAboveValue, relatedPlayer.position.z + 1), Time.deltaTime*10);
        }
    }

    private void LateUpdate()
    {
        //transform.position = 
    }

    public void StateChangeTo(TileState targetState)
    {
        currentState = targetState;
        switch (currentState)
        {
            case TileState.Normal:
                GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                break;
            case TileState.InRange:
                GetComponentInChildren<MeshRenderer>().material.color = Color.cyan;
                break;
            case TileState.Selected:
                break;
            case TileState.OnHold:
                GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                isPermited = false;
                Sequence s = DOTween.Sequence();
                s.Append(transform.DOMoveY(-0.3f, 0.1f));
                s.Append(transform.DOMoveY(yAboveValue, 0.1f));
                s.AppendInterval(0.1f);
                s.AppendCallback(() => currentState = TileState.OnFollow);
                break;
            case TileState.OnFly: 
                break;
            case TileState.OnStack:
                break;
        }
    }
}
