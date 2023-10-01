using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class O_3PointsCurveLR : MonoBehaviour
{
    public Transform[] points;
    public LineRenderer lr;
    public int vertextCount = 12;
    private float lineHeightPeak = 5;
    [HideInInspector] public Transform bondingTile;

    void Start()
    {
        
    }

    void Update()
    {
      if(bondingTile!=null)  UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        var pointList = new List<Vector3>();
        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertextCount)
        {
            var tangentLineVertext1 = Vector3.Lerp(points[0].position, points[1].position, ratio);
            var tangentLineVertext2 = Vector3.Lerp(points[1].position, points[2].position, ratio);
            var bezierPoint = Vector3.Lerp(tangentLineVertext1, tangentLineVertext2, ratio);
            pointList.Add(bezierPoint);
        }
        lr.positionCount = pointList.Count;
        lr.SetPositions(pointList.ToArray());
    }

    public void UpdatePointsPosition(Vector3 direction,float distance)
    {
        Vector3 horiDirection = new Vector3(direction.x, 0, direction.y);

        points[0].position = bondingTile.position;
        points[2].position = bondingTile.position + horiDirection * distance;

        Vector3 beforeHeightResetPos = (points[2].position - points[0].position) / 2 + points[0].position;
        points[1].position = new Vector3(beforeHeightResetPos.x, lineHeightPeak, beforeHeightResetPos.z);
    }

    public IEnumerator LineMatFadeTo(float alphaValue)
    {

        while (GetComponent<LineRenderer>().material.GetFloat("_Alpha") <= 1)
        {
            float targetValue = GetComponent<LineRenderer>().material.GetFloat("_Alpha") + Time.deltaTime * 3;
            GetComponent<LineRenderer>().material.SetFloat("_Alpha", targetValue);
            yield return null;
        }
    
    }

    private void OnValidate()
    {
        UpdateLineRenderer();
    }
}
