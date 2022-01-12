using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{

    public static CharacterController instance { get; private set; }

    [SerializeField]
    private float defaultSpeed, rotationAngle, rotationSpeed, centeringForce;

    private Rigidbody rb;

    private Vector3 moveVector;

    private float speed;

    private Vector3 targetScale;

    private Transform bone;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        speed = defaultSpeed;

        targetScale = transform.localScale;

        bone = FindObjectOfType<Bone>().transform;

        moveVector = Vector3.forward * speed;
    }

    private void FixedUpdate()
    {
        Move();

        FaceMoveDirection();

        CheckDistance();
    }

    private void Move()
    {
        var centeringVector = (CrowdManager.instance.furthestPos - transform.position) * centeringForce;

        moveVector = (bone.position - transform.position).normalized * speed + centeringVector;

        rb.velocity = Vector3.Lerp(rb.velocity, moveVector, Time.deltaTime * rotationSpeed);
    }

    private void CheckDistance()
    {
        if((CrowdManager.instance.furthestPos - transform.position).magnitude > CrowdManager.instance.maxDistanceToFurthest)
        {
            CrowdManager.instance.RemoveCharacter(this);
        }
    }

    public void MultiplySpeed(float value, float changeDuration)
    {
        DOTween.To(() => speed, x => speed = x, speed * value, changeDuration);
    }

    public void AddSpeed(float value, float changeDuration)
    {
        DOTween.To(() => speed, x => speed = x, speed + value, changeDuration);
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
}
