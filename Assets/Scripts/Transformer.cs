using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
public class Transformer : MonoBehaviour
{

    [SerializeField]
    private bool playOnAwake;

    [SerializeField]
    private TargetTransform[] targets;

    private void Start()
    {
        if (playOnAwake)
            Play();
    }

    public void Play()
    {
        var sequence = DOTween.Sequence();

        foreach (var target in targets)
        {
            sequence.AppendInterval(target.delay);

            sequence.Append(transform.DOMove(target.target.position, target.duration).SetEase(target.ease));
            sequence.Join(transform.DORotateQuaternion(target.target.rotation, target.duration).SetEase(target.ease));

            if(target.OnComplete.GetPersistentEventCount() > 0)
            {
                sequence.AppendCallback(() => target.OnComplete.Invoke());
            }
        }

        sequence.Play();
    }

}

[System.Serializable]
public class TargetTransform
{

    public Transform target;

    public float delay, duration;

    public Ease ease;

    public UnityEvent OnComplete;
}