using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class O_TileDetection : MonoBehaviour
{
    public List<Transform> inRangeTiles = new List<Transform>();
    public PlayerID thisPlayer;

    private void Start()
    {
        thisPlayer = GetComponentInParent<O_Player>().thisPlayer;
        GetComponentInParent<O_Player>().inRangeTiles = inRangeTiles;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<O_Tile>() != null)
        {
            O_Tile tile = other.GetComponent<O_Tile>();
            if (tile.belonging == thisPlayer && tile.currentState == TileState.Normal && tile.isPermited)
            {
                inRangeTiles.Add(other.transform);
                tile.StateChangeTo(TileState.InRange);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (inRangeTiles.Contains(other.transform))
        {
            inRangeTiles.Remove(other.transform);
            other.GetComponent<O_Tile>().StateChangeTo(TileState.Normal);
        }
    }

    public bool DetectTowardsTileMovable(Vector3 targetPos)
    {
        Collider[] inAreaObjects = Physics.OverlapBox(targetPos, new Vector3(0.1f, 1f, 0.1f));
        bool isMovable = false;
        foreach (Collider collider in inAreaObjects)
        {
            if (collider.gameObject == this) continue;
            if (collider.GetComponent<O_Tile>() != null)
                if (collider.GetComponent<O_Tile>().isPermited == true && collider.GetComponent<O_Tile>().belonging == GetComponentInParent<O_Player>().thisPlayer) isMovable = true;
        }

        return isMovable;

        //List<Transform> filtedTiles = new List<Transform>();
        //foreach (Collider collider in inAreaObjects)
        //{

        //    if (collider.GetComponent<O_Tile>() == null) continue;
        //    O_Tile tempTile = collider.GetComponent<O_Tile>();
        //    Debug.Log(tempTile.gameObject.name);
        //    if (!tempTile.isPermited) continue;

        //    if (tempTile.currentState == TileState.Normal || tempTile.currentState == TileState.InRange || tempTile.currentState == TileState.Selected)
        //        filtedTiles.Add(collider.transform);
        //}

        //return false;
    }
}
