using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grabbable))]
public class DestroyOnRelease : MonoBehaviour
{
    [SerializeField]
    float delay = 2.0f;

    private void Start()
    {
        // Attach callbacks
        GetComponent<Grabbable>().onRelease.AddListener((_,_) => StartCoroutine("DelayedDestroy"));
        GetComponent<Grabbable>().onGrab.AddListener((_, _) => StopCoroutine("DelayedDestroy"));
    }

    private IEnumerator DelayedDestroy()
    {
        Debug.Log("Delayed destroy");
        yield return new WaitForSeconds(delay);

        if (enabled)
        {
            Destroy(this.gameObject);
        }
    }

}
