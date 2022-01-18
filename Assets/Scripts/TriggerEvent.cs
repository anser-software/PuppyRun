using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class TriggerEvent : MonoBehaviour
{

    [SerializeField]
    private string[] targetTags;

    [SerializeField]
    private UnityEvent CallOnTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var tag in targetTags)
        {
            if(other.CompareTag(tag))
            {
                CallOnTriggerEnter?.Invoke();

                return;
            }
        }
    }

}
