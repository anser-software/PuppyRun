using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FinishCamera : MonoBehaviour
{

    [SerializeField]
    private Transform finishCam;

    [SerializeField]
    private float transitionDuration;

    private void Start()
    {
        Finish.instance.OnFinished += SetFinishCamera;
    }

    private void SetFinishCamera()
    {
        transform.DOMove(finishCam.position, transitionDuration).SetEase(Ease.InOutSine);
        transform.DORotateQuaternion(finishCam.rotation, transitionDuration).SetEase(Ease.InOutSine);
    }
}
