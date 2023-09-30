using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public enum PlayerID { PlayerA, PlayerB }
public enum PlayerState { Walking, Holding, Aiming }
public class O_Player : MonoBehaviour
{
    public PlayerID thisPlayer;
    public PlayerState currentState;
    public float moveSpeed;
    private Vector2 moveDirection;

    public float gridMoveSpeed;
    public bool isDPadMove;
    private float moveRange;
    private bool isOnMove;
    private Vector2 gridMoveDirection;

    private Vector2 aimDirection;
    private float detectionRange;
    public Transform selectedHighLightBox;

    private bool isConfirmed;

    public List<Transform> inRangeTiles = new List<Transform>();
    private Transform currentTargetTile;

    void Start()
    {
        detectionRange = GetComponentInChildren<BoxCollider>().size.x;
        moveRange = FindObjectOfType<TextureReader>().textureWidth / 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDPadMove) Action_GridMove();
        else Action_Move();

        SelectedTargetingTile();
        //PickUpTile();
    }

    public void Action_Move()
    {
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y) * Time.deltaTime * moveSpeed;
    }

    public void Action_GridMove()
    {
        if(!isOnMove && gridMoveDirection != Vector2.zero)
        {
            isOnMove = true;
            Vector3 targetPos = new Vector3(gridMoveDirection.x, 0, gridMoveDirection.y) * moveRange + transform.position;
            transform.DOMove(targetPos, gridMoveSpeed).OnComplete(()=>isOnMove = false);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    public void OnGridMove(InputAction.CallbackContext context)
    {
        gridMoveDirection = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        aimDirection = context.ReadValue<Vector2>().normalized;
    }

    public void OnConfirm(InputAction.CallbackContext callback)
    {
        PickUpTile();
        //isConfirmed = callback.ReadValue<bool>();
    }

    void PickUpTile()
    {
        Debug.Log("Pressed");
        if (currentTargetTile != null && currentState == PlayerState.Walking)
        {
            currentTargetTile.GetComponent<O_Tile>().StateChangeTo(TileState.OnHold);
            currentTargetTile = null;
            currentState = PlayerState.Holding;
        }
    }

    void SelectedTargetingTile()
    {
        if (currentState ==  PlayerState.Walking)
        if (aimDirection != Vector2.zero && inRangeTiles.Count != 0)
        {
            float rangeDistanceRecord = 10;
            int tileOnAimIndex = 0;
            for (int i = 0; i < inRangeTiles.Count; i++)
            {
                float distance = Vector3.Distance(inRangeTiles[i].position, transform.position + detectionRange * new Vector3(aimDirection.x, 0, aimDirection.y));
                if (distance < rangeDistanceRecord)
                {
                    rangeDistanceRecord = distance;
                    tileOnAimIndex = i;
                }
            }
            //Debug.Log("Entered");
            currentTargetTile = inRangeTiles[tileOnAimIndex];
            currentTargetTile.GetComponent<O_Tile>().StateChangeTo(TileState.Selected);
            selectedHighLightBox.position = currentTargetTile.position;
        }
    }
}
