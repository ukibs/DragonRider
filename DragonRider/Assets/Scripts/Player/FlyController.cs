using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlyController : MonoBehaviour
{
    //
    [Header("Components")]
    public Transform bodyTransform;
    public Transform rotationReplicator;    // NOTA: Este solo se usa en seguimiento automatizado
    public Animator animator;
    public Camera cam;
    [Header("Parameters")]
    public float bodyRotationSpeed = 90;
    public float mainRotationSpeed = 45;
    public float startingSpeed = 30;
    public float accelerationRate = 30;
    public float brakeRate = 45;
    public float flapForce = 10;
    public float maxSpeed = 100;
    public Vector2 cameraFovs = new Vector2(30, 60);

    //
    private Rigidbody rb;
    private float currentSpeed = 0;
    private float currentVerticalSpeed;
    private bool focusedManeuvers = false;
    private bool braking = false;

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
        if(CameraControl.Instance.LockedObjective == null)
        {
            UpdateBodyRotationManually(dt);
        }
        else
        {
            UpdateBodyRotationAuto(dt);
        }
        //
        UpdateMainRotation(dt);
        UpdateSpeed(dt);
        UpdateMovement(dt);
        UpdateCameraFov(dt);
        //
        UpdateActions();
    }

    void UpdateSpeed(float dt)
    {
        float angleAdapted = bodyTransform.localEulerAngles.x > 180 ? bodyTransform.localEulerAngles.x - 360 : bodyTransform.localEulerAngles.x;
        currentSpeed += angleAdapted / 90 * accelerationRate * dt;
        currentSpeed -= braking ? brakeRate * dt : 0;
        currentSpeed = Mathf.Max(currentSpeed, 0);
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        //
        if(currentSpeed == 0)
            animator.SetBool("Idle", true);
        else
            animator.SetBool("Idle", false);
    }

    //void UpdateBodyRotationManually(float dt)
    //{
    //    // bodyTransform.localEulerAngles = new Vector3(InputController.Instance.MovementAxis.y * 90, 0, -InputController.Instance.MovementAxis.x * 90);
        
    //    //
    //    float focusedManeuversMultiplier = focusedManeuvers ? 0.5f : 1;
    //    //
    //    Vector2 inputAxis = InputController.Instance.MovementAxis;
    //    Vector3 desiredEulers = new Vector3(inputAxis.y * 90 * focusedManeuversMultiplier, 0, -inputAxis.x * 90 * focusedManeuversMultiplier);
    //    //
    //    Vector3 currentEulers = bodyTransform.localEulerAngles;
    //    if (currentEulers.x > 180) currentEulers.x -= 360;
    //    if (currentEulers.z > 180) currentEulers.z -= 360;
    //    ////
    //    //float offsetX = desiredEulers.x - currentEulers.x;
    //    //float offsetZ = desiredEulers.z - currentEulers.z;
    //    ////
    //    //offsetX = Mathf.Clamp(offsetX, -bodyRotationSpeed * dt, bodyRotationSpeed * dt);
    //    //offsetZ = Mathf.Clamp(offsetZ, -bodyRotationSpeed * dt, bodyRotationSpeed * dt);
    //    ////
    //    //currentEulers.x += offsetX;
    //    //currentEulers.z += offsetZ;
    //    //
    //    bodyTransform.localEulerAngles = Vector3.Lerp(currentEulers, desiredEulers, dt);
    //    // bodyTransform.localEulerAngles = currentEulers;
    //}

    void UpdateBodyRotationManually(float dt)
    {
        //
        Vector2 inputAxis = InputController.Instance.MovementAxis;
        UpdateBodyRotation(inputAxis, dt);
    }

    void UpdateBodyRotationAuto(float dt)
    {
        //
        Vector3 currentObjectiveOffset = CameraControl.Instance.LockedObjective.position - transform.position;
        //Vector3 rotationOffest = currentObjectiveOffset - transform.forward;
        Vector3 horizontalOffset = Vector3.ProjectOnPlane(currentObjectiveOffset, transform.up);
        Vector3 verticalOffset = Vector3.ProjectOnPlane(currentObjectiveOffset, transform.right);
        //
        float horizontalAngle = Vector3.SignedAngle(horizontalOffset, transform.forward, transform.up);
        float verticalAngle = Vector3.SignedAngle(verticalOffset, rotationReplicator.forward, rotationReplicator.right);

        // Si el objetivo se va demasiado, que desenganche y vuevla a control manual
        float loseObjectiveAngle = 30;
        if(Mathf.Abs(horizontalAngle) > loseObjectiveAngle || Mathf.Abs(verticalAngle) > loseObjectiveAngle)
        {
            CameraControl.Instance.UnLockObjective();
        }
        else
        {
            //
            float maxForcingAngle = 5;
            horizontalAngle = (Mathf.Abs(horizontalAngle) >= maxForcingAngle) ? Mathf.Sign(horizontalAngle) : horizontalAngle / maxForcingAngle;
            verticalAngle = (Mathf.Abs(verticalAngle) >= maxForcingAngle) ? Mathf.Sign(verticalAngle) : verticalAngle / maxForcingAngle;
            //
            UpdateBodyRotation(new Vector2(-horizontalAngle, -verticalAngle), dt);
        }
    }

    void UpdateBodyRotation(Vector2 axisInput, float dt)
    {
        //
        float focusedManeuversMultiplier = focusedManeuvers ? 0.5f : 1;
        //
        Vector3 desiredEulers = new Vector3(axisInput.y * 90 * focusedManeuversMultiplier, 0, -axisInput.x * 90 * focusedManeuversMultiplier);
        //
        Vector3 currentEulers = bodyTransform.localEulerAngles;
        if (currentEulers.x > 180) currentEulers.x -= 360;
        if (currentEulers.z > 180) currentEulers.z -= 360;
        //
        bodyTransform.localEulerAngles = Vector3.Lerp(currentEulers, desiredEulers, 0.01f);
        //
        rotationReplicator.localEulerAngles = new Vector3(bodyTransform.localEulerAngles.x, 0, 0);
    }

    void UpdateMainRotation(float dt)
    {
        //
        float rotationToUse = bodyTransform.localEulerAngles.z > 180 ? bodyTransform.localEulerAngles.z - 360 : bodyTransform.localEulerAngles.z;
        //float rotationToUse = bodyTransform.localEulerAngles.z;
        // Debug.Log(bodyTransform.localEulerAngles.z + " - " + rotationToUse);
        //
        transform.Rotate(Vector3.up, -rotationToUse / 90 * mainRotationSpeed * dt);
    }

    void UpdateMovement(float dt)
    {
        Vector3 upSpeed = bodyTransform.up * currentVerticalSpeed * dt;
        Vector3 forwardSpeed = bodyTransform.forward * currentSpeed * dt;
        transform.position += forwardSpeed + upSpeed;
        // rb.velocity = bodyTransform.forward * currentSpeed * dt;
    }

    void UpdateCameraFov(float dt)
    {
        //
        float objectiveFov = focusedManeuvers ? cameraFovs.x : cameraFovs.y;
        //
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, objectiveFov, 0.1f);
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

        //
        if (InputController.Instance.LeftShoulderPressed)
        {
            braking = true;
        }
        else if (InputController.Instance.LeftShoulderReleased)
        {
            braking = false;
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
