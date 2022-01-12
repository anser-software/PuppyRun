using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField]
    private Rigidbody target;
    
    [SerializeField]
    private float speed, overdoMagnitude, overdoAcceleration;

    private Vector3 offset;

    private Vector3 overdo = Vector3.zero;

    private void Start()
    {
        offset = transform.position - target.position;
    }

    private void FixedUpdate()
    {
        overdo = Vector3.Lerp(overdo, target.velocity * overdoMagnitude, Time.deltaTime * overdoAcceleration);
        //var targetPos =  target.position + overdo + offset;

        overdo.x = 0F;

        var targetPos = new Vector3(0F, offset.y, CrowdManager.instance.furthestPos.z + offset.z) + overdo;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
    }

}
