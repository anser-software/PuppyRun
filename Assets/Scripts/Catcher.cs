using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Catcher : MonoBehaviour
{

    [SerializeField]
    private float speed, sampleInterval, goToNextPointDistance, catchDistance, catchDuration;

    [SerializeField]
    private Animator animator;

    private float timer;

    private Queue<Vector3> targetPositions = new Queue<Vector3>();

    private Vector3 currentTargetPosition;

    private bool catching;

    private void Start()
    {
        SamplePosition();

        currentTargetPosition = targetPositions.Dequeue();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0F)
        {
            SamplePosition();
            timer = sampleInterval;
        }

        if (catching)
            return;

        var displacement = currentTargetPosition - transform.position;

        transform.Translate(displacement.normalized * speed * Time.deltaTime);

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

        DOTween.Sequence().SetDelay(catchDuration).OnComplete(() => catching = false);
    }

}
