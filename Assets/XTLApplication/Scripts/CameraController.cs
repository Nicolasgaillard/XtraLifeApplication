using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool _isRotatingCameraAroundTheWorld = false;

    public Canvas BaseCanvas;

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

    public void SetupPlayerCamera()
    {
        /*
        Camera.main.transform.SetParent(GameObject.FindGameObjectWithTag("ParentPlayerCamera").transform);
        Camera.main.transform.localPosition = new Vector3(0, -0.3f, -3.3f);
        */
    }
}
