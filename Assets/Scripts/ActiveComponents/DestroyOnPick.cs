using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnPick : MonoBehaviour
{
    [SerializeField]
    private float delay;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            StartCoroutine(Destroy());
        }
    }

    private IEnumerator Destroy()
    {
        yield return null;

        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }
}
