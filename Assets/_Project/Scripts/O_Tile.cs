using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public enum TileState { Normal, InRange, Selected, OnHold,OnFollow, OnFly, OnStack }
public class O_Tile : MonoBehaviour
{
    public TileState currentState;
    public PlayerID belonging;
    public bool isPermited = true;
    private Transform relatedPlayer;
    private float yAboveValue = 0.5f;
    private float flySpeed=15;
    private List<Vector3> path = new List<Vector3> ();
    private int moveIndex = 0;
    public Texture inRangeTex;
    public Texture outRangeTex;

    void Start()
    {
        //foreach(O_Player player in FindObjectsOfType<O_Player>())
        //{
        //    if (player.thisPlayer == belonging) relatedPlayer = player.transform; break;
        //}
 
    }

    public void BindingPlayer(O_Player targetPlayer)
    {
        relatedPlayer = targetPlayer.transform;
    }

    void Update()
    {
        if (currentState == TileState.OnFollow)
        {
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(relatedPlayer.position.x, yAboveValue, relatedPlayer.position.z + 1),
                Time.deltaTime * 10);
        }

        if (currentState == TileState.OnFly)
        {
            Vector3 currentTarget = path[moveIndex];
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, flySpeed * Time.deltaTime);
            transform.Rotate(Vector3.forward, 10);
            float distance = Vector3.Distance(currentTarget,transform.position);
            if (distance <= 0.1f) moveIndex++;

            if (moveIndex > path.Count - 1)
            {
                StackTile();
            }
        }
    }

    public void StateChangeTo(TileState targetState)
    {
        if (isPermited)
        {
            currentState = targetState;
            switch (currentState)
            {
                case TileState.Normal:
                    if (outRangeTex != null) GetComponentInChildren<MeshRenderer>().material.mainTexture = outRangeTex;
                    else GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                    break;
                case TileState.InRange:
                    if (inRangeTex != null) GetComponentInChildren<MeshRenderer>().material.mainTexture = inRangeTex;
                    else GetComponentInChildren<MeshRenderer>().material.color = Color.cyan;
                    //GetComponentInChildren<MeshRenderer>().material.color = Color.cyan;
                    break;
                case TileState.Selected:
                    break;
                case TileState.OnHold:
                    if (outRangeTex != null) GetComponentInChildren<MeshRenderer>().material.mainTexture = outRangeTex;
                    else GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                    //GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                    isPermited = false;
                    Sequence s = DOTween.Sequence();
                    s.Append(transform.DOMoveY(-0.3f, 0.1f));
                    s.AppendCallback(() => M_Audio.PlayOneShotAudio("Pick"));
                    s.Append(transform.DOMoveY(yAboveValue, 0.1f));
                    s.AppendCallback(() => M_Audio.PlayOneShotAudio("Hold"));
                    s.AppendInterval(0.1f);
                    s.AppendCallback(() => currentState = TileState.OnFollow);
                    s.AppendCallback(() => relatedPlayer.GetComponent<O_Player>().currentState = PlayerState.Holding);
                    break;
                case TileState.OnFly:
                    
                    break;
                case TileState.OnStack:
                    break;
            }
        }

    }

    public void PathAssign(List<Vector3> pathToAssign)
    {
        path.Clear();
        foreach (Vector3 item in pathToAssign) path.Add(item);
        currentState = TileState.OnFly;
        M_Audio.PlayOneShotAudio("Shoot");
        isPermited = false;
    }

    private void StackTile()
    {
        currentState = TileState.OnStack;
        Collider[] inAreaObjects = Physics.OverlapBox(transform.position,new Vector3(1,1,1));
        //Debug.Log(inAreaObjects.Length);
        List<Transform> filtedTiles = new List<Transform>();
        foreach (Collider collider in inAreaObjects)
        {
           
            if (collider.GetComponent<O_Tile>() == null) continue;
            O_Tile tempTile = collider.GetComponent<O_Tile>();
            //Debug.Log(tempTile.gameObject.name);
            if (!tempTile.isPermited) continue;
     
            if (tempTile.currentState == TileState.Normal || tempTile.currentState == TileState.InRange || tempTile.currentState == TileState.Selected) 
            filtedTiles.Add(collider.transform);
        }
        //Debug.Log(filtedTiles.Count);

        float rangeDistanceRecord = 10;
        int tileOnAimIndex = 999;
        for (int i = 0; i < filtedTiles.Count; i++)
        {
            float distance = Vector3.Distance(filtedTiles[i].position, transform.position);
            if (distance < rangeDistanceRecord)
            {
                rangeDistanceRecord = distance;
                tileOnAimIndex = i;
            }
        }
        //Debug.Log(tileOnAimIndex);
        if (tileOnAimIndex!=999)
        {
            Transform targetTile = filtedTiles[tileOnAimIndex];
            ChangeTileToDeselectbale(targetTile.GetComponent<O_Tile>());
            Sequence s = DOTween.Sequence();
            s.Append(transform.DORotate(Vector3.zero, 0.3f));
            s.AppendInterval(0.2f);
            s.Append(transform.DOMove(new Vector3(targetTile.position.x, transform.position.y, targetTile.position.z), 0.6f));
            s.AppendInterval(0.1f);
            s.Append(transform.DOMoveY(0.2f, 0.1f)).OnComplete(InstantiateHitVFX);
            s.AppendCallback(() => DetectIsPlayerHitted());
        }
        else
        {
            Sequence s = DOTween.Sequence();
            s.Append(transform.DORotate(Vector3.zero, 0.3f));
            s.AppendInterval(0.2f);
            s.Append(transform.DOPunchScale(new Vector3(0.7f, 0.7f, 0.7f), 0.3f, 6, 0.3f));
            s.AppendInterval(0.3f);
            s.AppendCallback(() => M_Audio.PlayOneShotAudio("Miss"));
            s.Append(transform.DOMoveY(-20, 3f));
        }
  
        void InstantiateHitVFX()
        {
            GameObject go = Instantiate(M_BattelRepo.instance.vfx_DonutExplosive, transform.position, Quaternion.identity);
            go.GetComponent<ParticleSystem>().Play();
            M_Audio.PlayOneShotAudio("Land");
            Destroy(go, 5f);
        }
    }

    void ChangeTileToDeselectbale(O_Tile targetTile)
    {
        targetTile.StateChangeTo(TileState.Normal);
        targetTile.relatedPlayer.GetComponent<O_Player>().RemoveCurrentSelectedTile(targetTile);
        targetTile.isPermited = false;
        targetTile.currentState = TileState.OnStack;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("Entered");
    //        if(collision.gameObject.GetComponent<O_Player>().thisPlayer != belonging)
    //        FindObjectOfType<M_BattleManager>().GameEnd(collision.gameObject.GetComponent<O_Player>().thisPlayer);
    //    }
    //}

    private void DetectIsPlayerHitted()
    {
        Collider[] inAreaObjects = Physics.OverlapBox(transform.position, new Vector3(0.2f, 1, 0.2f));
        //Debug.Log(inAreaObjects.Length);
        List<Transform> filtedTiles = new List<Transform>();
        foreach (Collider collider in inAreaObjects)
        {
            if (collider.GetComponent<O_Player>() != null)
                if (collider.gameObject.GetComponent<O_Player>().thisPlayer != belonging)
                {
                    FindObjectOfType<M_BattleManager>().GameEnd(collider.gameObject.GetComponent<O_Player>().thisPlayer);
                    Sequence s = DOTween.Sequence();
                    s.Append(collider.transform.DOScale(1.3f, 0.1f));
                    s.Append(collider.transform.DOScale(0f, 0.3f));
                    InstantiateHitToDeathVFX();
                }
        }

        void InstantiateHitToDeathVFX()
        {
            GameObject go = Instantiate(M_BattelRepo.instance.vfx_PlayerDeath, transform.position, Quaternion.identity);
            go.GetComponent<ParticleSystem>().Play();
            M_Audio.PlayOneShotAudio("Death");
            Destroy(go, 5f);
        }
    }
}
