using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{

    public static CharacterController instance { get; private set; }

    [SerializeField]
    private bool modifyDefaultSpeed;

    [SerializeField]
    private float defaultSpeed, rotationAngle, rotationSpeed, centeringForce, eatBoneDistance;

    private Rigidbody rb;

    private Vector3 moveVector;

    private float speed;

    private Vector3 targetScale;

    private float targetSpeed;

    private bool caught;

    private Vector3 catchTargetPos;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        targetSpeed = speed = defaultSpeed;

        targetScale = transform.localScale;

        moveVector = Vector3.forward * speed;
    }

    private void FixedUpdate()
    {
        if (!caught)
        {
            Move();

            FaceMoveDirection();

            CheckDistance();
        } else
        {
            rb.velocity = (catchTargetPos - transform.position);
        }
    }

    private void Move()
    {
        var centeringVector = (CrowdManager.instance.furthestCharacter.position - transform.position) * centeringForce;

        var bone = BonesController.instance.targetBone;

        if (bone)
        {
            var displacementVector = bone.position - transform.position;

            moveVector = displacementVector.normalized * speed + centeringVector;

            if (displacementVector.sqrMagnitude < eatBoneDistance * eatBoneDistance)
                BonesController.instance.EatFirstBone();
        } else
        {
            moveVector = transform.forward * speed + centeringVector;
        }

        rb.velocity = rb.velocity.normalized * speed;

        rb.velocity = Vector3.Lerp(rb.velocity, moveVector, Time.deltaTime * rotationSpeed);   
    }

    private void CheckDistance()
    {
        if((CrowdManager.instance.furthestCharacter.position - transform.position).sqrMagnitude > CrowdManager.instance.maxDistanceToFurthest * CrowdManager.instance.maxDistanceToFurthest)
        {
            CrowdManager.instance.RemoveCharacter(this);
        }
    }

    public void MultiplySpeed(float value, float changeDuration, float totalDuration)
    {
        var seq = DOTween.Sequence();

        seq.AppendInterval(totalDuration);

        targetSpeed = modifyDefaultSpeed? defaultSpeed * value : targetSpeed * value;

        seq.Join(DOTween.To(() => speed, x => speed = x, targetSpeed, changeDuration));

        DOTween.Kill("RESET");

        seq.Append(DOTween.To(() => speed, x => speed = x, defaultSpeed, changeDuration).SetId("RESET").OnComplete(() => targetSpeed = defaultSpeed));
    }

    public void AddSpeed(float value, float changeDuration, float totalDuration)
    {
        var seq = DOTween.Sequence();

        seq.AppendInterval(totalDuration);

        targetSpeed = modifyDefaultSpeed ? defaultSpeed + value : targetSpeed + value;

        seq.Join(DOTween.To(() => speed, x => speed = x, targetSpeed, changeDuration));

        DOTween.Kill("RESET");

        seq.Append(DOTween.To(() => speed, x => speed = x, defaultSpeed, changeDuration).SetId("RESET").OnComplete(() => targetSpeed = defaultSpeed));
    }


    public void MultiplyScale(float value, float changeDuration)
    {
        targetScale = targetScale * value;
        Vector3 difInScale = targetScale - transform.localScale;

        transform.DOBlendableScaleBy(difInScale, changeDuration).SetEase(Ease.InOutSine);
    }

    public void AddScale(float value, float changeDuration)
    {
        targetScale = transform.localScale + Vector3.one * value;
        transform.DOBlendableScaleBy(Vector3.one * value, changeDuration).SetEase(Ease.InOutSine);
    }

    private void FaceMoveDirection()
    {
        transform.forward = Vector3.Lerp(transform.forward, rb.velocity.normalized, Time.deltaTime * rotationSpeed);
    }

    public void Catch(Vector3 targetPos)
    {
        caught = true;

        catchTargetPos = targetPos;
    }
}
