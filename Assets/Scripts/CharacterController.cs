using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{

    public static CharacterController instance { get; private set; }

    [SerializeField]
    private float defaultSpeed, rotationAngle, rotationSpeed;

    private Rigidbody rb;

    private Vector3 moveVector;

    private float speed;

    private Vector3 targetScale;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        speed = defaultSpeed;

        targetScale = transform.localScale;

        moveVector = Vector3.forward * speed;
    }

    private void FixedUpdate()
    {
        Move();

        FaceMoveDirection();
    }

    private void Move()
    {
        if (Input.GetMouseButton(0))
        {
            var moveDirectionOffset = (Mathf.Clamp01(InputManager.Instance.mouseViewportPosition.x) - 0.5F) * 2F;

            moveVector = Quaternion.AngleAxis(moveDirectionOffset * rotationAngle, Vector3.up) * Vector3.forward * speed;
        }

        rb.velocity = Vector3.Lerp(rb.velocity, moveVector, Time.deltaTime * rotationSpeed);
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
