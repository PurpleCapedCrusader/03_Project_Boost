using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform PlayerTransform;

    private Vector3 _cameraOffsetForCameraFollow;

    [Range(0.01f,1.0f)]
    public float SmoothFactor = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        _cameraOffsetForCameraFollow = transform.position - PlayerTransform.position;
    }

    // LateUpdate is called after Update methods
    void Update()
    {
        Vector3 newPos = PlayerTransform.position + _cameraOffsetForCameraFollow;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
    }

 }
           