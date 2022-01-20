using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{

    public static CharacterController instance { get; private set; }

    [SerializeField]
    private float rotationAngle, rotationSpeed, centeringForce, eatBoneDistance;

    [SerializeField]
    private GameObject speedUpFX;

    private Rigidbody rb;

    private Vector3 moveVector;

    private Vector3 targetScale;

    private bool caught, finished;

    private Vector3 catchTargetPos;

    

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        targetScale = transform.localScale;

        moveVector = Vector3.forward * CrowdManager.instance.speed;
    }

    private void FixedUpdate()
    {
        if (finished)
            return;

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

            moveVector = displacementVector.normalized * CrowdManager.instance.speed + centeringVector;

            if (displacementVector.sqrMagnitude < eatBoneDistance * eatBoneDistance)
                BonesController.instance.EatFirstBone();
        } else
        {
            moveVector = transform.forward * CrowdManager.instance.speed + centeringVector;
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

    public void Finish()
    {
        finished = true;

        rb.isKinematic = true;
    }

    public void SpeedUpFX(float duration)
    {
        if(speedUpFX)
        {
            speedUpFX.SetActive(true);

            DOTween.Sequence().SetDelay(duration).OnComplete(() => speedUpFX.SetActive(false));
        }
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

        rb.velocity = rb.velocity * 0.2F;

        catchTargetPos = targetPos;
    }
}
