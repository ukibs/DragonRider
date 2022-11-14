using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //
    //private Vector3 currentEuler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotation();
    }

    void UpdateRotation()
    {
        //
        Vector2 inputAxis = InputController.Instance.CameraAxis;
        Vector3 desiredEulers = new Vector3(inputAxis.y * 90, inputAxis.x * 180, 0);
        //
        Vector3 currentEulers = transform.localEulerAngles;
        if (currentEulers.x > 180) currentEulers.x -= 360;
        if (currentEulers.y > 180) currentEulers.y -= 360;
        //
        transform.localEulerAngles = Vector3.Lerp(currentEulers, desiredEulers, 0.25f);
    }
}
