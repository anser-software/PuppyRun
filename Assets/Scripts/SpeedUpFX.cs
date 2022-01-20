using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpFX : MonoBehaviour
{

    [SerializeField]
    private Vector3 offset;
    private void Update()
    {
        var targetPos = CrowdManager.instance.furthestCharacter.position + offset;

        targetPos.x = 0F;

        transform.position = targetPos;
    }
}
