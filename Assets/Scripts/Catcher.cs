using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Catcher : MonoBehaviour
{

    [SerializeField]
    private float speed, speedPostCatch, postCatchDuration, sampleInterval, goToNextPointDistance, catchDistance, catchDuration;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform netCenter;

    [SerializeField]
    private GameObject catchFX;

    private float timer;

    private Queue<Vector3> targetPositions = new Queue<Vector3>();

    private Vector3 currentTargetPosition;

    private CatcherState CatcherState = CatcherState.Chasing;

    private void Start()
    {
        SamplePosition();

        currentTargetPosition = targetPositions.Dequeue();

        Finish.instance.OnFinished += OnFinish;
    }

    private void OnFinish()
    {
        CatcherState = CatcherState.Inactive;

        transform.DOScale(Vector3.zero, 1F).OnComplete(() => Destroy(gameObject));
    }

    private void StartChasing()
    {
        CatcherState = CatcherState.Chasing;

        Debug.Log("CHASING");
    }

    private void Update()
    {
        if(CatcherState == CatcherState.Waiting)
        {
            if ((CrowdManager.instance.nearestCharacter.position - transform.position).sqrMagnitude < catchDistance * catchDistance)
            {
                Catch();
            }

            if(CrowdManager.instance.nearestCharacter.position.z > transform.position.z + 4F)
            {
                StartChasing();
            }

            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0F)
        {
            SamplePosition();
            timer = sampleInterval;
        }

        if (CatcherState != CatcherState.Chasing && CatcherState != CatcherState.PostCatch)
            return;

        var currentSpeed = CatcherState == CatcherState.PostCatch ? speedPostCatch : speed;

        var displacement = currentTargetPosition - transform.position;

        transform.Translate(displacement.normalized * currentSpeed * Time.deltaTime);

        if (displacement.sqrMagnitude < goToNextPointDistance * goToNextPointDistance)
            currentTargetPosition = targetPositions.Dequeue();

        transform.forward = Vector3.Lerp(transform.forward, displacement.normalized, Time.deltaTime * 5F);

        if((CrowdManager.instance.nearestCharacter.position - transform.position).sqrMagnitude < catchDistance * catchDistance)
        {
            Catch();
        }
    }

    private void SamplePosition()
    {
        targetPositions.Enqueue(CrowdManager.instance.furthestCharacter.position);
    }

    private void Catch()
    {
        CatcherState = CatcherState.Catching;

        animator.SetTrigger("Catch");

        var catchSequence = DOTween.Sequence();

        catchSequence.AppendInterval(catchDuration / 2F);

        catchSequence.AppendCallback(() => {
            if(catchFX)
            {
                Instantiate(catchFX, netCenter.position, Quaternion.identity);
            }
            CrowdManager.instance.RemoveLast();
        }); ;

        catchSequence.AppendInterval(catchDuration / 2F);

        catchSequence.AppendCallback(() => 
        {
            CatcherState = CatcherState.PostCatch;
        });

        catchSequence.AppendInterval(postCatchDuration);

        catchSequence.AppendCallback(() =>
        {
            if (CatcherState != CatcherState.Inactive)
            {
                CatcherState = CatcherState.Catching;
            }
        });
    }

}

public enum CatcherState
{
    Waiting,
    Chasing,
    Catching,
    PostCatch,
    Inactive
}