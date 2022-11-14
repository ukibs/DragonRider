using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlyController : MonoBehaviour
{
    //
    [Header("Components")]
    public Transform bodyTransform;
    public Animator animator;
    [Header("Parameters")]
    public float startingSpeed = 30;
    public float accelerationRate = 30;
    public float flapForce = 10;    

    //
    private Rigidbody rb;
    private float currentSpeed = 0;
    private float currentVerticalSpeed;
    public bool focusedManeuvers = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = startingSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //
        float dt = Time.deltaTime;
        //
        UpdateVerticalSpeed(dt);
        //
        UpdateBodyRotation();
        UpdateMainRotation(dt);
        UpdateSpeed(dt);
        UpdateMovement(dt);
        //
        UpdateActions();
    }

    void UpdateSpeed(float dt)
    {
        float angleAdapted = bodyTransform.localEulerAngles.x > 180 ? bodyTransform.localEulerAngles.x - 360 : bodyTransform.localEulerAngles.x;
        currentSpeed += angleAdapted / 90 * accelerationRate * dt;
        currentSpeed = Mathf.Max(currentSpeed, 0);
        //
        if(currentSpeed == 0)
            animator.SetBool("Idle", true);
        else
            animator.SetBool("Idle", false);
    }

    void UpdateBodyRotation()
    {
        // bodyTransform.localEulerAngles = new Vector3(InputController.Instance.MovementAxis.y * 90, 0, -InputController.Instance.MovementAxis.x * 90);
        
        //
        float focusedManeuversMultiplier = focusedManeuvers ? 0.5f : 1;
        //
        Vector2 inputAxis = InputController.Instance.MovementAxis;
        Vector3 desiredEulers = new Vector3(inputAxis.y * 90 * focusedManeuversMultiplier, 0, -inputAxis.x * 90 * focusedManeuversMultiplier);
        //
        Vector3 currentEulers = bodyTransform.localEulerAngles;
        if (currentEulers.x > 180) currentEulers.x -= 360;
        if (currentEulers.z > 180) currentEulers.z -= 360;
        //
        bodyTransform.localEulerAngles = Vector3.Lerp(currentEulers, desiredEulers, 0.25f);
    }

    void UpdateMainRotation(float dt)
    {
        //
        float rotationToUse = bodyTransform.localEulerAngles.z > 180 ? bodyTransform.localEulerAngles.z - 360 : bodyTransform.localEulerAngles.z;
        //float rotationToUse = bodyTransform.localEulerAngles.z;
        // Debug.Log(bodyTransform.localEulerAngles.z + " - " + rotationToUse);
        //
        transform.Rotate(Vector3.up, -rotationToUse / 90 * 45 * dt);
    }

    void UpdateMovement(float dt)
    {
        Vector3 upSpeed = bodyTransform.up * currentVerticalSpeed * dt;
        Vector3 forwardSpeed = bodyTransform.forward * currentSpeed * dt;
        transform.position += forwardSpeed + upSpeed;
        // rb.velocity = bodyTransform.forward * currentSpeed * dt;
    }

    void UpdateActions()
    {
        //
        if (InputController.Instance.APressed)
        {
            animator.SetBool("Flapping", true);
        }
        else if (InputController.Instance.AReleased)
        {
            animator.SetBool("Flapping", false);
        }

        //
        if (InputController.Instance.RightShoulderPressed)
        {
            focusedManeuvers = true;
        }
        else if (InputController.Instance.RightShoulderReleased)
        {
            focusedManeuvers = false;
        }
    }

    public void AddVerticalSpeed()
    {
        //if(currentSpeed > 0)
        currentVerticalSpeed += flapForce;
        currentVerticalSpeed = Mathf.Min(currentVerticalSpeed, flapForce);
    }

    void UpdateVerticalSpeed(float dt)
    {
        currentVerticalSpeed -= 9.81f * dt;
        currentVerticalSpeed = Mathf.Max(currentVerticalSpeed, 0);
    }
}
