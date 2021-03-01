using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<CheckPointManager>().Trigger(gameObject);
    }
}
