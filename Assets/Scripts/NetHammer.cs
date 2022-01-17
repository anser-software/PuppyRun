using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class NetHammer : MonoBehaviour
{

    [SerializeField]
    private float hitInterval, hitDuration, releaseDuration, hitAngle;

    private float timer;

    private bool inHit;

    private void Update()
    {
        if (inHit)
            return;

        timer -= Time.deltaTime;

        if(timer <= 0F)
        {
            Hit();

            timer = hitInterval;
        }
        
    }

    private void Hit()
    {
        inHit = true;

        var hitSequence = DOTween.Sequence();

        hitSequence.Append(transform.DOBlendableRotateBy(Vector3.back * hitAngle, hitDuration).SetEase(Ease.InSine));
        hitSequence.Append(transform.DOBlendableRotateBy(Vector3.forward * hitAngle, releaseDuration).SetEase(Ease.InOutSine));
        hitSequence.OnComplete(() => inHit = false);
    }

}
