using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedModifier : MonoBehaviour
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

        if(other.CompareTag("Character"))
        {
            switch(ModType)
            {
                case ModType.Add:
                    CharacterController.instance.AddSpeed(value, changeDuration);
                    break;
                case ModType.MultiplyBy:
                    CharacterController.instance.MultiplySpeed(value, changeDuration);
                    break;
                case ModType.DivideBy:
                    CharacterController.instance.MultiplySpeed(1F / value, changeDuration);
                    break;
            }

            activated = true;
        }
    }

}

public enum ModType
{
    Add,
    MultiplyBy,
    DivideBy
}
