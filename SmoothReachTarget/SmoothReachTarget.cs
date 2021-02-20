using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothReachTarget : MonoBehaviour
{
    public Transform Target;

    [SerializeField] private float speed = 1f, turnSmoothing = 1f;
    [SerializeField] private AnimationCurve speedCurve, turnSmoothingCurve;

    private Vector3 curDirection = Vector3.zero;
    private float startingDistance = 0f;

    private void OnEnable()
    {
        ReachTarget(Target, Vector3.forward);
    }

    public void ReachTarget(Transform _Target, Vector3 _OriginalDirection)
    {
        if (_Target == null) return;

        Target = _Target;
        curDirection = _OriginalDirection;
        startingDistance = Vector3.Distance(transform.position, Target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null)
            return;


        float curDistance = Vector3.Distance(transform.position, Target.transform.position);
        float distanceRatio = Mathf.Clamp01(curDistance / startingDistance);

        float curSpeed = speedCurve.Evaluate(distanceRatio) * speed;
        float curTurnSmoothing = turnSmoothingCurve.Evaluate(distanceRatio) * turnSmoothing;

        Vector3 Direction = Vector3.Normalize(Target.transform.position - transform.position);
        curDirection = Vector3.Lerp(curDirection, Direction, Time.deltaTime * curTurnSmoothing);

        transform.position += curDirection * curSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(curDirection, Vector3.up);
    }
}
