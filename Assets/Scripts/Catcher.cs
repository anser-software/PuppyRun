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

    private float timer;

    private Queue<Vector3> targetPositions = new Queue<Vector3>();

    private Vector3 currentTargetPosition;

    private bool catching, postCatch;

    private bool active = true;

    private void Start()
    {
        SamplePosition();

        currentTargetPosition = targetPositions.Dequeue();

        Finish.instance.OnFinished += OnFinish;
    }

    private void OnFinish()
    {
        active = false;

        transform.DOScale(Vector3.zero, 1F).OnComplete(() => Destroy(gameObject));
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0F)
        {
            SamplePosition();
            timer = sampleInterval;
        }

        if (catching || !active)
            return;

        var currentSpeed = postCatch ? speedPostCatch : speed;

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
        catching = true;

        animator.SetTrigger("Catch");

        var catchSequence = DOTween.Sequence();

        catchSequence.AppendInterval(catchDuration / 2F);

        catchSequence.AppendCallback(CrowdManager.instance.RemoveLast);

        catchSequence.AppendInterval(catchDuration / 2F);

        catchSequence.AppendCallback(() => 
        { 
            catching = false;
            postCatch = true;
        });

        catchSequence.AppendInterval(postCatchDuration);

        catchSequence.AppendCallback(() => postCatch = false);
    }

}
