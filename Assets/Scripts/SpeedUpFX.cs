using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpFX : MonoBehaviour
{

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private float scaleFactor;

    private void Update()
    {
        transform.localScale = Vector3.one * CrowdManager.instance.characters.Count * scaleFactor;

        var targetPos = CrowdManager.instance.furthestCharacter.position + offset;

        transform.position = targetPos;
    }
}
