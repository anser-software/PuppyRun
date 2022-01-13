using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : MonoBehaviour
{

    [SerializeField]
    private float xDefault, xRange, zOffset, speed;

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        var x = transform.position.x;

        var y = transform.position.y;

        var z = CrowdManager.instance.furthestCharacter.position.z + zOffset;

        if (Input.GetMouseButton(0))
        {
            x = Mathf.Lerp(transform.position.x, 
                xDefault + Mathf.Lerp(xRange, -xRange, Mathf.Clamp01(InputManager.Instance.mouseViewportPosition.x)),
                Time.deltaTime * speed);
        }

        transform.position = new Vector3(x, y, z);
    }

}
