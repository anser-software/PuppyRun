using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRemover : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        var charController = other.GetComponent<CharacterController>();

        if (charController)
        {
            CrowdManager.instance.RemoveCharacter(charController);
        }
    }
}
