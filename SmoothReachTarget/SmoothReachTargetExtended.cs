using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class SmoothReachTarget : MonoBehaviour
{
    [FoldoutGroup("Unity Events")]
    public UnityEvent OnStart, OnReach;
    public Transform Target;
    [HorizontalGroup("H1")]
    public bool AutoStart = false, DeactivateOnReach = false;
    public float StopDistance = 0.3f;
    [Space]
    [HorizontalGroup("H0", LabelWidth = 100)]
    public float Speed = 1f, TurnSmoothing = 1f;
    public bool FadeOutSpeed = false;
    [ShowIf("FadeOutSpeed")][Tooltip("Fade out speed along distance to player from start : 1 is Start")]
    public AnimationCurve FadeSpeedCurve;
    public bool IncreaseTurnSmoothing = false;
    [ShowIf("IncreaseTurnSmoothing")]
    [Tooltip("Increase turn Smoothing the closer to player : 1 is Start")]
    public AnimationCurve TurnSmoothingCurve;

    private Vector3 targetPosition = Vector3.positiveInfinity;
    private bool started = false;
    private float startingDistance = 0f, curspeed = 0f, curTurnSmoothing = 0f;
    private Vector3 originalPos, startingDirection, curDirection; private Quaternion originalRot;

    [ButtonGroup("B1")]
    [Button("Start")]
    public void StartFromEditor()
    {
        originalPos = transform.position;
        originalRot = transform.rotation;
        StartReachingEditor(transform.forward);
    }


    [ButtonGroup("B1")]
    [Button("Reset Transform")]
    public void ResetTransform()
    {
        transform.position = originalPos;
        transform.rotation = originalRot;
        started = false;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if (Target == null || Speed == 0f)
            return;

        if (AutoStart)
        {
            started = true;
            StartReaching(Target, transform.forward);
        }
            
        //else
        //    started = false;
        
    }



    public void StartReachingEditor(Vector3 startingVelocity)
    {
        if (Target == null || Speed == 0f)
            return;

        initialize(Target.position, startingVelocity);
    }

    public void StartReaching(Transform _Target, Vector3 startingVelocity)
    {
        Target = _Target;
        if (Target == null || Speed == 0f)
            return;

        initialize(Target.position, startingVelocity);
    }

    public void StartReaching(Vector3 _targetPosition, Vector3 startingVelocity)
    {
        Target = null;
        targetPosition = _targetPosition;
        if (targetPosition == Vector3.positiveInfinity || Speed == 0f)
            return;

        initialize(targetPosition, startingVelocity);
    }

    private void initialize(Vector3 _targetPosition, Vector3 startingVelocity)
    {
        targetPosition = _targetPosition;
        startingDistance = Vector3.Distance(targetPosition, transform.position);
        startingDirection = startingVelocity;
        curDirection = Vector3.zero;
        curspeed = Speed; curTurnSmoothing = TurnSmoothing;
        started = true;
        OnStart?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPosition == Vector3.positiveInfinity || Speed == 0f || !started)
            return;

        if (Target != null)
            targetPosition = Target.position;

        float curDistance = Vector3.Distance(targetPosition, transform.position);
        float distanceRatio = Mathf.Clamp(curDistance / startingDistance, 0f, 1f);

        if(FadeOutSpeed) curspeed = FadeSpeedCurve.Evaluate(distanceRatio) * Speed;
        if(IncreaseTurnSmoothing) curTurnSmoothing = TurnSmoothingCurve.Evaluate(distanceRatio) * TurnSmoothing;

        Vector3 Direction = Vector3.Normalize(targetPosition - transform.position);

        if (curDirection == Vector3.zero)
            curDirection = Vector3.Normalize(startingDirection);
        else
            curDirection = Vector3.Lerp(curDirection, Direction, Time.deltaTime * curTurnSmoothing);

        // Vector3 smoothTurnDirection = Vector3.Lerp(transform.forward, Direction, Time.deltaTime * curTurnSmoothing);
        // Vector3 smoothTurnDirection = Vector3.Lerp(curDirection, Direction, Time.deltaTime * curTurnSmoothing);

        // transform.position += transform.forward * curspeed * Time.deltaTime;
        transform.position += curDirection * curspeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(curDirection, Vector3.up); // smoothTurnDirection
        
        if (curDistance <= StopDistance)
        {
            OnReach?.Invoke();
            started = false;
            if (DeactivateOnReach)
                this.gameObject.SetActive(false);
        }
            
    }
}
