using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    [SerializeField]
    private float speed;

    private Vector3 offset;

    private float maxZ;

    private void Start()
    {
        offset = transform.position - CrowdManager.instance.furthestCharacter.position;

        maxZ = CrowdManager.instance.furthestCharacter.position.z;
    }

    private void FixedUpdate()
    {
        maxZ = Mathf.Max(CrowdManager.instance.furthestCharacter.position.z, maxZ);

        var targetPos = new Vector3(0F, offset.y, maxZ + offset.z);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
    }

}
