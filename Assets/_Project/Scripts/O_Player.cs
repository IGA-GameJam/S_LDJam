using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using MoreMountains.Feedbacks;

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
    public O_3PointsCurveLR beizerCurve_3P;
    public float curveHoriDefaultDistance;
    public float curveHoriMaxDistance;
    private float curveHoriCurrentDistance;
    public float curveStretchSpeed;
    public MMF_Player mmf_Move;

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
        UpdateAimingLine();
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
            Vector3 targetPos = new Vector3(gridMoveDirection.x, 0, gridMoveDirection.y) * moveRange + transform.position;

            bool isMovable = GetComponentInChildren<O_TileDetection>().DetectTowardsTileMovable(targetPos);
            if (isMovable)
            {
                isOnMove = true;
                transform.DOMove(targetPos, gridMoveSpeed).OnComplete(() => isOnMove = false);
                mmf_Move.PlayFeedbacks();
            }
            else
            {

            }

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
        Vector2 tempValue = context.ReadValue<Vector2>().normalized;
        if (Mathf.Abs( tempValue.x)+ Mathf.Abs(tempValue.y) > 0.5f)
            aimDirection = context.ReadValue<Vector2>().normalized;
        else aimDirection = Vector2.zero;
    }

    public void OnConfirm(InputAction.CallbackContext callback)
    {
        if (callback.phase == InputActionPhase.Started && currentState == PlayerState.Walking) PickUpTile();
    }

    public void OnLaunch(InputAction.CallbackContext callback)
    {
        if (callback.phase == InputActionPhase.Started && currentState == PlayerState.Aiming)
            StartCoroutine(LaunchTile());
    }

    void PickUpTile()
    {
        //Debug.Log("Pressed");
        if (currentTargetTile != null && currentState == PlayerState.Walking)
        {
            currentTargetTile.GetComponent<O_Tile>().StateChangeTo(TileState.OnHold);
            //currentTargetTile = null;
            currentState = PlayerState.Holding;
        }
    }

    void SelectedTargetingTile()
    {
        if (currentState == PlayerState.Walking)
            if (aimDirection != Vector2.zero && inRangeTiles.Count != 0)
            {
                float rangeDistanceRecord = 10;
                int tileOnAimIndex = 0;
                for (int i = 0; i < inRangeTiles.Count; i++)
                {
                    float distance = Vector3.Distance(inRangeTiles[i].position, transform.position + detectionRange*2 * new Vector3(aimDirection.x, 0, aimDirection.y).normalized);
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

    void UpdateAimingLine()
    {

        if (aimDirection != Vector2.zero && currentState != PlayerState.Walking)
        {
            if (currentState != PlayerState.Walking)
            {
                beizerCurve_3P.gameObject.SetActive(true);
                StartCoroutine(beizerCurve_3P.LineMatFadeTo(1));
                beizerCurve_3P.bondingTile = currentTargetTile;
            }
            if (currentState == PlayerState.Holding)
            {
                currentState = PlayerState.Aiming;

                beizerCurve_3P.UpdatePointsPosition(aimDirection, curveHoriDefaultDistance);
                curveHoriCurrentDistance = curveHoriDefaultDistance;
            }
            if (currentState == PlayerState.Aiming)
            {
                curveHoriCurrentDistance = curveHoriCurrentDistance + curveStretchSpeed * Time.deltaTime;
                if (curveHoriCurrentDistance > curveHoriMaxDistance) curveHoriCurrentDistance = curveHoriMaxDistance;
                beizerCurve_3P.UpdatePointsPosition(aimDirection, curveHoriCurrentDistance);
            }
        }
        else
        {
            if (beizerCurve_3P.GetComponent<LineRenderer>().material.GetFloat("_Alpha") > 0)
            {
                float targetValue = beizerCurve_3P.GetComponent<LineRenderer>().material.GetFloat("_Alpha") - Time.deltaTime * 3;
                beizerCurve_3P.GetComponent<LineRenderer>().material.SetFloat("_Alpha", targetValue);
            }
            else
            {
                curveHoriCurrentDistance = curveHoriDefaultDistance;
                beizerCurve_3P.gameObject.SetActive(false);
            }
        }


    }

    IEnumerator LaunchTile()
    {
        List<Vector3> tempPoses = new List<Vector3> ();
        for (int i = 0; i < beizerCurve_3P.lr.positionCount; i++)
        {
            tempPoses.Add(beizerCurve_3P.lr.GetPosition(i));
        }
        currentTargetTile.GetComponent<O_Tile>().PathAssign(tempPoses);
        aimDirection = Vector2.zero;
        beizerCurve_3P.bondingTile = null;
        currentTargetTile = null;
        currentState = PlayerState.Walking;
        yield return null;
    }
}
