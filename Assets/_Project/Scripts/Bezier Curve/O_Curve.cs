using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class O_Curve : MonoBehaviour
{
    [SerializeField]
    private Transform[] controlPoints;
    private Vector2 gizmosPosition;
    public bool is2DCurve;

    private void OnDrawGizmos()
    {
        if (is2DCurve) Draw2DCurve();
        else Draw3DCurve();
    }


    void Draw2DCurve()
    {
        for (float t = 0; t <= 1; t += 0.05f)
        {
            gizmosPosition = Mathf.Pow(1 - t, 3) * controlPoints[0].position +
      3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position +
      3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position +
      Mathf.Pow(t, 3) * controlPoints[3].position;

            Gizmos.DrawSphere(gizmosPosition, 0.25f);
        }

        Gizmos.DrawLine(new Vector2(controlPoints[0].position.x, controlPoints[0].position.y),
            new Vector2(controlPoints[1].position.x, controlPoints[1].position.y));

        Gizmos.DrawLine(new Vector2(controlPoints[2].position.x, controlPoints[2].position.y),
            new Vector2(controlPoints[3].position.x, controlPoints[3].position.y));
    }

    void Draw3DCurve()
    {
        for (float t = 0; t <= 1; t += 0.05f)
        {
            gizmosPosition = Mathf.Pow(1 - t, 3) * controlPoints[0].position +
      3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position +
      3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position +
      Mathf.Pow(t, 3) * controlPoints[3].position;

            Gizmos.DrawSphere(gizmosPosition, 0.25f);
        }

        Gizmos.DrawLine(controlPoints[0].position, controlPoints[1].position);
        Gizmos.DrawLine(controlPoints[2].position, controlPoints[3].position);
    }
}
