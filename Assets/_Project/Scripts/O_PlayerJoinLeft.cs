using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class O_PlayerJoinLeft : MonoBehaviour
{
    void Start()
    {
        PlayerInitialize();
    }

    void Update()
    {
        
    }

    private void PlayerInitialize()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        transform.localScale = Vector3.zero;
        M_BattleManager bm = FindObjectOfType<M_BattleManager>();

        if (bm.playerA == null) AssignPlayerToSlot(PlayerID.PlayerA);
        else AssignPlayerToSlot(PlayerID.PlayerB);

        void AssignPlayerToSlot(PlayerID targetPlayer)
        {
            if (targetPlayer == PlayerID.PlayerA)
            {
                bm.playerA = GetComponent<O_Player>();
                GetComponent<O_Player>().thisPlayer = PlayerID.PlayerA;
                GetComponentInChildren<SpriteRenderer>().sprite = M_BattelRepo.instance.playerPortrait[0];
                transform.position = bm.pivots[0].position;
            }
            else
            {
                bm.playerB = GetComponent<O_Player>();
                GetComponent<O_Player>().thisPlayer = PlayerID.PlayerB;
                GetComponentInChildren<SpriteRenderer>().sprite = M_BattelRepo.instance.playerPortrait[1];
                transform.position = bm.pivots[1].position;
            }

            if (targetPlayer == PlayerID.PlayerB) FindObjectOfType<M_Dialogue>().TheTalkingBegin();
        }

     
    }
}
