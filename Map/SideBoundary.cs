using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBoundary : MonoBehaviour
{
    public Transform playerTransform;

    void Update()
    {
        this.transform.position = new Vector3( 0, 0, playerTransform.position.z);
    }
}
