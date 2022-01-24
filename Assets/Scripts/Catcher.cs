using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Catcher : MonoBehaviour
{

    [SerializeField]
    private float speed, speedPostCatch, postCatchDuration, sampleInterval,
        goToNextPointDistance, catchDistance, catchDuration, surprisedDuration, startChasingDifferenceZ, turnDifferenceZ;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform netCenter;

    [SerializeField]
    private GameObject catchFX;

    private float timer;

    private Queue<Vector3> targetPositions = new Queue<Vector3>();

    private Vector3 currentTargetPosition;

    private CatcherState CatcherState = CatcherState.Waiting;

    private void Start()
    {
        Finish.instance.OnFinished += OnFinish;

        GameManager.instance.OnLose += OnLose;
    }
    private void OnFinish()
    {
        CatcherState = CatcherState.Inactive;

        transform.DOScale(Vector3.zero, 1F).OnComplete(() => Destroy(gameObject));
    }

    private void OnLose()
    {
        CatcherState = CatcherState.Inactive;

        animator.SetTrigger("Idle");
    }

    private void StartChasing()
    {
        animator.SetTrigger("Surprised");

        CatcherState = CatcherState.Surprised;

        DOTween.Sequence().SetDelay(surprisedDuration).OnComplete(() => CatcherState = CatcherState.Chasing);
    }

    private void Update()
    {
        if(CatcherState == CatcherState.Waiting)
        {
            if ((CrowdManager.instance.nearestCharacter.position - transform.position).sqrMagnitude < catchDistance * catchDistance * 0.25F)
            {
                Catch();
            }

            if(CrowdManager.instance.nearestCharacter.position.z > transform.position.z + startChasingDifferenceZ)
            {
                StartChasing();
            }

            return;
        }

        if (CatcherState != CatcherState.Chasing && CatcherState != CatcherState.PostCatch)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0F)
        {
            SamplePosition();
            timer = sampleInterval;
        }

        Move();
    }

    private void Move()
    {
        var currentSpeed = CatcherState == CatcherState.PostCatch ? speedPostCatch : speed;

        var displacement = CrowdManager.instance.furthestCharacter.position - transform.position;

        if (displacement.z < turnDifferenceZ)
            displacement = Vector3.back;

        transform.Translate(displacement.normalized * currentSpeed * Time.deltaTime, Space.World);

        //if (displacement.sqrMagnitude < goToNextPointDistance * goToNextPointDistance)
        //    currentTargetPosition = targetPositions.Dequeue();

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(displacement.normalized, Vector3.up), Time.deltaTime * 10F);

        if ((CrowdManager.instance.nearestCharacter.position - transform.position).sqrMagnitude < catchDistance * catchDistance)
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
                Instantiate(catchFX, CrowdManager.instance.nearestCharacter.position, Quaternion.identity);
            }
            CrowdManager.instance.RemoveLast();
        });

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
                CatcherState = CatcherState.Chasing;
            }
        });
    }

}

public enum CatcherState
{
    Waiting,
    Surprised,
    Chasing,
    Catching,
    PostCatch,
    Inactive
}