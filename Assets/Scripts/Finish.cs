using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
public class Finish : MonoBehaviour
{

    public static Finish instance { get; private set; }

    public Action OnFinished;

    [SerializeField]
    private Transform foodPile;

    [SerializeField]
    private float startEatingDelay, eatDuration, changeInHeightPerCharacter, bowlRadius;

    [SerializeField] [Range(-1F, 1F)]
    private float gapBetweenCharacters, initialOffset, offsetPerCircle;

    private bool activated;

    private void Awake()
    {
        instance = this;
    }

    public void Activate()
    {
        if (activated)
            return;

        activated = true;

        CrowdManager.instance.ActivateFinish();

        DOTween.Sequence().SetDelay(startEatingDelay).OnComplete(() => StartEating(CrowdManager.instance.characters.Count));

        OnFinished?.Invoke();
    }

    public Vector3[] GetTargetPositions(int count)
    {
        Vector3[] targetPositions = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            var circle = Mathf.Floor(gapBetweenCharacters * i);

            var angle = (initialOffset + gapBetweenCharacters * i + circle * offsetPerCircle) * Mathf.PI * 2F;

            targetPositions[i] = transform.position + new Vector3(Mathf.Cos(angle), 0F, Mathf.Sin(angle)) * bowlRadius;
        }

        return targetPositions;
    }

    public void StartEating(int totalCharacters)
    {
        var totalChangeInHeight = changeInHeightPerCharacter * totalCharacters;

        Debug.Log("totalChangeInHeight" + totalChangeInHeight);

        foodPile.transform.DOBlendableMoveBy(Vector3.up * totalChangeInHeight, eatDuration).SetEase(Ease.OutSine);
    }
}
