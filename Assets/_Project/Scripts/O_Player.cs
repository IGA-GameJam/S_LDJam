using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Jahaha.SceneTransition;

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
    private M_BattleManager bm;


    void Start()
    {
        detectionRange = GetComponentInChildren<BoxCollider>().size.x;
        moveRange = FindObjectOfType<TextureReader>().textureWidth / 100;
        bm = FindObjectOfType<M_BattleManager>();
    }

    void Update()
    {
        if (bm.isGameStart)
        {
            if (isDPadMove) Action_GridMove();
            else Action_Move();

            SelectedTargetingTile();
            UpdateAimingLine();
        }
    }

    public void ActiveLine()
    {
        transform.parent.GetComponentInChildren<O_3PointsCurveLR>().gameObject.SetActive(true);
    }

    public void Action_Move()
    {
        transform.position += new Vector3(moveDirection.x, 0, moveDirection.y) * Time.deltaTime * moveSpeed;
    }

    public void Action_GridMove()
    {
        if(!isOnMove && gridMoveDirection != Vector2.zero)
        {
            if (Mathf.Abs(gridMoveDirection.x) < Mathf.Abs(gridMoveDirection.y)) gridMoveDirection = new Vector2(0, gridMoveDirection.y);
            else gridMoveDirection = new Vector2(gridMoveDirection.x, 0);

            Vector3 targetPos = new Vector3(gridMoveDirection.x, 0, gridMoveDirection.y) * moveRange + transform.position;
            bool isMovable = GetComponentInChildren<O_TileDetection>().DetectTowardsTileMovable(targetPos);
            if (isMovable)
            {
                isOnMove = true;
                transform.DOMove(targetPos, gridMoveSpeed).OnComplete(() => isOnMove = false);
                mmf_Move.PlayFeedbacks();
                gridMoveDirection = Vector2.zero;
                M_Audio.PlayOneShotAudio("Walk");
            }
            else
            {

            }

        }
    }

    public void OnPressDown(InputAction.CallbackContext context)
    {
        if (!FindObjectOfType<M_Dialogue>().isOnConversation) Debug.Log("das");
        else FindObjectOfType<M_Dialogue>().TryTalk(thisPlayer);

        if (bm.isGameEnd) { M_SceneTransition.instance.TriggerTransition(1); M_SceneTransition.instance.DestroryUnDestroyable(M_BattelRepo.instance.gameObject); }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isDPadMove) moveDirection = context.ReadValue<Vector2>();
        else
        {
            //if(context.phase == InputActionPhase.Performed)
            //{
            //    Vector2 tempValue = context.ReadValue<Vector2>().normalized;
            //    if (Mathf.Abs(tempValue.x) > 0.9f || Mathf.Abs(tempValue.y) > 0.9f)
            //    {
            //        if (Mathf.Abs(tempValue.x) > Mathf.Abs(tempValue.y)) gridMoveDirection = new Vector3(tempValue.x > 0 ? 1 : -1, 0);
            //        else gridMoveDirection = new Vector3(0, tempValue.y > 0 ? 1 : -1);
            //    }
            //}

                //gridMoveDirection = context.ReadValue<Vector2>().normalized;

            //gridMoveDirection = context.ReadValue<Vector2>();
        }
    }

    public void OnGridMove(InputAction.CallbackContext context)
    {
        gridMoveDirection = context.ReadValue<Vector2>().normalized;
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
        if (currentTargetTile != null && currentState == PlayerState.Walking && inRangeTiles.Contains(currentTargetTile))
        {
            if (currentTargetTile.GetComponent<O_Tile>().isPermited)
            {
                //M_Audio.PlayOneShotAudio("Hold");
                currentTargetTile.GetComponent<O_Tile>().StateChangeTo(TileState.OnHold);
                //currentTargetTile = null;
                currentState = PlayerState.Holding;
            }

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

    public void RemoveCurrentSelectedTile(O_Tile tileToEnsure)
    {
        if (inRangeTiles.Contains(tileToEnsure.transform))
        {
            Debug.Log("Entered");
            GetComponentInChildren<O_TileDetection>().inRangeTiles.Remove(tileToEnsure.transform);
            inRangeTiles.Remove(tileToEnsure.transform);
            //currentTargetTile = null;
        }
    }
}
