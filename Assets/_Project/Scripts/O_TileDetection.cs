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
            if (tile.belonging == thisPlayer && tile.currentState == TileState.Normal)
            {
                inRangeTiles.Add(other.transform);
                tile.StateChangeTo(TileState.InRange);
            }

            // ȥ������������TileҲ���ǽŵ��µģ����ǰɣ�ÿ�����Ȼ������ȥ��������һֱ���㣬��BUG
            //float rangeDistanceRecord = 999;
            //int tileOnFootIndex = 0;
            //for (int i = 0; i < inRangeTiles.Count; i++)
            //{
            //    float distance = Vector3.Distance(inRangeTiles[i].position, transform.position);
            //    if (distance < rangeDistanceRecord)
            //    {
            //        rangeDistanceRecord = distance;
            //        tileOnFootIndex = i;
            //    }
            //}
            //Debug.Log(tileOnFootIndex);
            //inRangeTiles.RemoveAt(tileOnFootIndex);


            //foreach (Transform tileToChange in inRangeTiles)
            //{
            //    tileToChange.GetComponent<O_Tile>().StateChangeTo(TileState.InRange);
            //}
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
}
