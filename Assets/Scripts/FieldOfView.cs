using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

    public float viewRadius;

    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Dictionary<Transform, float> visibleTargets = new Dictionary<Transform, float>();

    private Team myTeam;

    private void Start()
    {
        viewRadius = transform.parent.GetComponentInChildren<GunScript>().rangeInMeter;
        myTeam = transform.parent.GetComponentInChildren<FlagCaptureScript>().playerTeam;
    }

    private void FixedUpdate()
    {
        FindVisibleTargets();
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for(int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    if(target.GetComponent<FlagCaptureScript>().playerTeam != myTeam)
                    visibleTargets.Add(target, dstToTarget);
                }
            }
        }
        visibleTargets.OrderBy(x => x.Value);
    }

    public Vector3 DirFromAngle(float angleInDgrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDgrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDgrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDgrees * Mathf.Deg2Rad));
    }
}
