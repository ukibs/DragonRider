using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    //
    [Header("Components")]
    public GameObject fire;
    public GameObject trail;
    public GameObject smokeTrail;
    [Header("Parameters")]
    public float movementSpeed;
    public float rotationSpeed;

    //
    private Objective objective;
    private Vector3 nextPointToGo;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        objective = GetComponent<Objective>();
        nextPointToGo = new Vector3(Random.Range(-500, 500), Random.Range(-500, 500), Random.Range(-500, 500));
    }

    // Update is called once per frame
    void Update()
    {
        //
        float dt = Time.deltaTime;
        //
        UpdateMovement(dt);
        CheckCheckPoint();
    }

    void CheckCheckPoint()
    {
        if((transform.position - nextPointToGo).sqrMagnitude < Mathf.Pow(50, 2))
        {
            nextPointToGo = new Vector3(Random.Range(-500, 500), Random.Range(-500, 500), Random.Range(-500, 500));
        }
    }

    void UpdateMovement(float dt)
    {
        //
        Vector3 nextPointDirection;
        float rotationSpeedMultiplier = isDead ? 4 : 1;
        if (!isDead)
        {
            nextPointDirection = nextPointToGo - transform.position;
        }
        else
        {
            nextPointDirection = Vector3.down;
        }
            
        //
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, nextPointDirection, rotationSpeed * rotationSpeedMultiplier * dt * Mathf.Deg2Rad, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        //
        transform.position += transform.forward * movementSpeed * dt;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    public void Die()
    {
        objective.targeteable = false;
        trail.SetActive(false);
        fire.SetActive(true);
        smokeTrail.SetActive(true);
        isDead = true;
        Destroy(gameObject, 20f);
        CameraControl.Instance.UnLockObjective();
    }
}
