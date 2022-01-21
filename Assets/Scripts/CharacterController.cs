using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{

    public static CharacterController instance { get; private set; }

    [SerializeField]
    private float rotationAngle, rotationSpeed, eatBoneDistance, startScale, scaleUpDuration, eatDelay;

    [SerializeField]
    private Animator animator;

    private Rigidbody rb;

    private Vector3 moveVector;

    private Vector3 targetScale;

    private bool caught, finished, eating;

    private Vector3 catchTargetPos;

    private Vector3 lastPos;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        targetScale = transform.localScale;

        moveVector = Vector3.forward * CrowdManager.instance.speed;

        transform.localScale = Vector3.one * startScale;

        lastPos = transform.position;

        transform.DOScale(1F, scaleUpDuration);
    }

    private void FixedUpdate()
    {
        if (finished)
            return;

        if (!caught)
        {
            Move();

            CheckDistance();
        } else
        {
            rb.velocity = (catchTargetPos - transform.position);
            transform.forward = moveVector.normalized;
        }
    }

    private void LateUpdate()
    {
        if(!caught)
            FaceMoveDirection();
    }

    private void Move()
    {
        var centeringDisplacement = CrowdManager.instance.furthestCharacter.position - transform.position;

        if (CrowdManager.instance.furthestCharacter == transform)
        {
            var bone = BonesController.instance.targetBone;

            if (bone)
            {
                var displacementVector = bone.position - transform.position;

                moveVector = displacementVector.normalized * CrowdManager.instance.speed;

                if (displacementVector.sqrMagnitude < eatBoneDistance * eatBoneDistance)
                    BonesController.instance.EatFirstBone();
            }
        } else
        {
            moveVector = centeringDisplacement.normalized * CrowdManager.instance.speed;
        }

        rb.velocity = rb.velocity.normalized * CrowdManager.instance.speed;

        rb.velocity = Vector3.Lerp(rb.velocity, moveVector, Time.deltaTime * rotationSpeed);   
    }

    private void CheckDistance()
    {
        if((CrowdManager.instance.furthestCharacter.position - transform.position).sqrMagnitude > CrowdManager.instance.maxDistanceToFurthest * CrowdManager.instance.maxDistanceToFurthest)
        {
            CrowdManager.instance.RemoveCharacter(this);
        }
    }

    public void Eat(Vector3 targetPos)
    {
        eating = true;

        transform.DORotateQuaternion(Quaternion.LookRotation(Finish.instance.transform.position - targetPos, Vector3.up), 0.4F);

        animator.SetTrigger("Eat");
    }

    public void Sit()
    {
        animator.SetTrigger("Sit");
    }

    public void SetFinish(Vector3 targetPos)
    {
        finished = true;

        rb.isKinematic = true;

        DOTween.Sequence().SetDelay(eatDelay).OnComplete(() => Eat(targetPos));
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
        if (eating)
            return;       

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rb.velocity.normalized, Vector3.up), Time.deltaTime * rotationSpeed);
        //transform.forward = Vector3.Lerp(transform.forward, rb.velocity.normalized, Time.deltaTime * rotationSpeed);
        lastPos = transform.position;
    }

    public void Catch(Vector3 targetPos)
    {
        caught = true;

        rb.velocity = rb.velocity * 0.2F;

        catchTargetPos = targetPos;
    }
}
