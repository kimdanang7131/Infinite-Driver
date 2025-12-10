using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] float cameraIntervalZ;

    void Start()
    {
        cameraIntervalZ = transform.position.z;
        
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, playerTransform.position.z + cameraIntervalZ);
    }
}
