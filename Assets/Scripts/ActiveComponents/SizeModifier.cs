using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeModifier : MonoBehaviour
{
    [SerializeField]
    private ModType ModType;

    [SerializeField]
    private float value;

    [SerializeField]
    private float changeDuration;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (other.CompareTag("Character"))
        {
            switch (ModType)
            {
                case ModType.Add:
                    CharacterController.instance.AddScale(value, changeDuration);
                    break;
                case ModType.MultiplyBy:
                    CharacterController.instance.MultiplyScale(value, changeDuration);
                    break;
                case ModType.DivideBy:
                    CharacterController.instance.MultiplyScale(1F / value, changeDuration);
                    break;
            }

            activated = true;
        }
    }
}
