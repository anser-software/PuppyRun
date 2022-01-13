using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdCountModifier : MonoBehaviour
{

    [SerializeField]
    private ModType ModType;

    [SerializeField]
    private int value;

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
                    if (value > 0)
                    {
                        for (int i = 0; i < value; i++)
                        {
                            CrowdManager.instance.AddCharacter();
                        }
                    } else if(value < 0)
                    {
                        for (int i = 0; i < value; i++)
                        {
                            CrowdManager.instance.RemoveRandomCharacter();
                        }
                    }
                    break;
                case ModType.MultiplyBy:
                    var countToAdd = CrowdManager.instance.characters.Count * (value - 1);

                    for (int i = 0; i < countToAdd; i++)
                    {
                        CrowdManager.instance.AddCharacter();
                    }
                    break;
                case ModType.DivideBy:
                    var countToRemove = CrowdManager.instance.characters.Count - (CrowdManager.instance.characters.Count / value);

                    for (int i = 0; i < countToRemove; i++)
                    {
                        CrowdManager.instance.RemoveRandomCharacter();
                    }
                    break;
            }

            activated = true;
        }
    }

}