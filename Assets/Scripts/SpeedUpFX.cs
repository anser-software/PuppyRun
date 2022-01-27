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

        var furthest = CrowdManager.instance.furthestCharacter;

        var targetPos = CrowdManager.instance.furthestCharacter.position + furthest.right * offset.x + furthest.up * offset.y + furthest.forward * offset.z;

        transform.position = targetPos;

        if(CrowdManager.instance.characters.Count > 0)
            transform.forward = furthest.forward;
    }
}
