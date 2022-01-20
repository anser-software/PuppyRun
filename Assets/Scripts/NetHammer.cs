using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
public class NetHammer : MonoBehaviour
{

    [SerializeField]
    private float hitInterval, hitDuration, releaseDuration, hitAngle, catchRadius;

    [SerializeField]
    private Transform netParent, netCenter, netShadow;

    [SerializeField]
    private Vector3 shadowMinScale, shadowMaxScale;

    [SerializeField]
    private GameObject catchFX;

    private float timer;

    private bool inHit;

    private bool active = true;

    private int id;

    private void Start()
    {
        id = Random.Range(0, int.MaxValue);
    }

    private void Update()
    {
        if (inHit || !active)
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
        hitSequence.SetId("NetHammer_" + id.ToString());
        hitSequence.Append(netParent.DOBlendableRotateBy(Vector3.back * hitAngle, hitDuration).SetEase(Ease.InSine));
        hitSequence.Join(netShadow.DOScale(shadowMaxScale, hitDuration).SetEase(Ease.InSine));
        hitSequence.AppendCallback(() => TryCatch());
        hitSequence.Append(netParent.DOBlendableRotateBy(Vector3.forward * hitAngle, releaseDuration).SetEase(Ease.InOutSine));
        hitSequence.Join(netShadow.DOScale(shadowMinScale, releaseDuration).SetEase(Ease.InOutSine));
        hitSequence.OnComplete(() => inHit = false);
    }

    private void TryCatch()
    {
        var caught = Physics.OverlapSphere(netCenter.position, catchRadius).Where(c => c.CompareTag("Character")).ToArray();

        if (caught.Length > 0)
        {
            foreach (var character in caught)
            {
                CrowdManager.instance.FreezeCharacter(character.GetComponent<CharacterController>(), netCenter.position);
            }

            DOTween.Kill("NetHammer_" + id.ToString());

            if(catchFX)
            {
                Instantiate(catchFX, netCenter.position, Quaternion.identity);
            }

            active = false;
        }
    }

}
