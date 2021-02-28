using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool _isRotatingCameraAroundTheWorld = false;

    [SerializeField]
    private float _cameraRotationSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        if(_isRotatingCameraAroundTheWorld)
        {
            transform.Rotate(0, _cameraRotationSpeed * Time.deltaTime, 0);
        }
    }

    public void StartRotateAroundTheWorld()
    {
        _isRotatingCameraAroundTheWorld = true;
    }

    public void StopRotateAroundTheWorld()
    {
        _isRotatingCameraAroundTheWorld = false;
    }

    public void DisableCamera()
    {
        gameObject.SetActive(false);
    }
}
