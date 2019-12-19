using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    public float viewRadius;
    [SerializeField]
    [Range(0,359)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obsticleMask;
    [HideInInspector]
    public List<Transform> visibleTargets;

    public float meshResolution;
    private void Start()
    {
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while(true){
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        visibleTargets.Clear();
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            //check if its in view angle
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                //performa raycast to check for obsticles
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obsticleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    private void Update()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            Debug.DrawLine(transform.position, transform.position + DirectionFromAngle(angle, true) * viewRadius, Color.yellow);
        }
        for (int i = 0; i < visibleTargets.Count; i++)
        {
            Debug.DrawLine(transform.position, visibleTargets[i].position, Color.red);
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad) , 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }


}
