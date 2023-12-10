using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    //
    public enum PlaneState
    {
        Patrolling,
        Engaged,
        Dead
    }

    //
    [Header("Components")]
    public GameObject[] fires;
    public GameObject explosion;
    public GameObject trail;
    public GameObject smokeTrail;
    public Transform[] shootPoints;
    public GameObject bulletPrefab;
    [Header("Parameters")]
    public int maxHealth = 3;
    public float movementSpeed;
    public float rotationSpeed;
    public float fireRate = 0.5f;

    //
    private Objective objective;
    private Vector3 nextPointToGo;
    private PlaneState currentState = PlaneState.Engaged;
    private Formation currentFormation;
    private FlyController flyController;
    private bool playerOnSight = false;
    private int currentHealh;

    //
    public Formation CurrentFormation
    {
        get { return currentFormation; }
        set { currentFormation = value; }
    }

    public PlaneState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        objective = GetComponent<Objective>();
        currentHealh = maxHealth;
        nextPointToGo = new Vector3(Random.Range(-500, 500), Random.Range(-500, 500), Random.Range(-500, 500));
        StartCoroutine(AttackCycle());
    }

    // Update is called once per frame
    void Update()
    {
        //
        float dt = Time.deltaTime;
        //
        UpdateBehaviour(dt);
        //UpdateMovement(dt);
        CheckCheckPoint();
    }

    void CheckCheckPoint()
    {
        if((transform.position - nextPointToGo).sqrMagnitude < Mathf.Pow(50, 2))
        {
            nextPointToGo = new Vector3(Random.Range(-500, 500), Random.Range(-500, 500), Random.Range(-500, 500));
        }
    }

    void UpdateBehaviour(float dt)
    {
        // Para ver como movemos
        if (currentState == PlaneState.Dead)
        {
            UpdateDeadMovement(dt);
        }
        else if(currentFormation && this == currentFormation.PlaneControllers[0])
        {
            switch (currentState)
            {
                case PlaneState.Patrolling:
                    UpdateFreeMovement(dt);
                    break;
                case PlaneState.Engaged:
                    UpdateChaseMovement(dt);
                    break;
            }
        }
        else if (currentFormation)
        {
            UpdateInFormationMovement(dt);
        }

        // Para ver si atacamos
        if(currentState == PlaneState.Engaged)
        {
            UpdateMachineGunAttack(dt);
        }
    }

    #region Movement Methods

    void UpdateDeadMovement(float dt)
    {
        //
        Vector3 nextPointDirection = Vector3.down;
        float rotationSpeedMultiplier = 4;
        //
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, nextPointDirection, rotationSpeed * rotationSpeedMultiplier * dt * Mathf.Deg2Rad, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        //
        transform.position += transform.forward * movementSpeed * dt;
    }
        

    void UpdateFreeMovement(float dt)
    {
        //
        Vector3 nextPointDirection;
        float rotationSpeedMultiplier = 1;
        nextPointDirection = nextPointToGo - transform.position;
            
        //
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, nextPointDirection, rotationSpeed * rotationSpeedMultiplier * dt * Mathf.Deg2Rad, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        //
        transform.position += transform.forward * movementSpeed * dt;
    }

    void UpdateChaseMovement(float dt)
    {
        //
        if (!flyController)
            flyController = FindObjectOfType<FlyController>();
        //
        Vector3 nextPointDirection;
        Vector3 positionToLook = flyController.transform.position;
        float rotationSpeedMultiplier = 1;

        //
        float travelTime = GeneralFunctions.EstimateTimeBetweenTwoPoints(shootPoints[0].position, flyController.transform.position, 250);
        positionToLook = GeneralFunctions.EstimateFuturePosition(positionToLook,
                    flyController.transform.forward * flyController.CurrentSpeed, travelTime);

        //
        nextPointDirection = positionToLook - transform.position;

        //
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, nextPointDirection, rotationSpeed * rotationSpeedMultiplier * dt * Mathf.Deg2Rad, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        //
        transform.position += transform.forward * movementSpeed * dt;
    }

    void UpdateInFormationMovement(float dt)
    {
        //
        Vector3 nextPointDirection;
        float rotationSpeedMultiplier = 1;
        nextPointDirection = currentFormation.FormationPosition(currentFormation.PlaneControllers.IndexOf(this)) - transform.position;

        //
        float dot = Vector3.Dot(transform.forward, nextPointDirection);

        //
        float speedMultiplier = 1;
        if (nextPointDirection.sqrMagnitude > Mathf.Pow(5, 2))
            speedMultiplier = 1.2f;

        //
        if(dot == -1 && nextPointDirection.sqrMagnitude < Mathf.Pow(5, 2))
        {
            speedMultiplier = 0.8f;            
        }
        else
        {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, nextPointDirection, rotationSpeed * rotationSpeedMultiplier * dt * Mathf.Deg2Rad, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        } 

        //
        transform.position += transform.forward * movementSpeed * speedMultiplier * dt;
    }

    #endregion

    //
    void UpdateMachineGunAttack(float dt)
    {
        //
        if(currentState == PlaneState.Engaged)
        {
            //
            if (!flyController)
                flyController = FindObjectOfType<FlyController>();
            //
            Vector3 playerDirection;
            playerDirection = flyController.transform.position - transform.position;

            //
            Vector3 horizontalOffset = Vector3.ProjectOnPlane(playerDirection, transform.up);
            Vector3 verticalOffset = Vector3.ProjectOnPlane(playerDirection, transform.right);
            //
            float horizontalAngle = Vector3.SignedAngle(horizontalOffset, transform.forward, transform.up);
            float verticalAngle = Vector3.SignedAngle(verticalOffset, transform.forward, transform.right);

            //
            float minAngleToAttack = 5f;
            if(Mathf.Abs(horizontalAngle) <= minAngleToAttack && Mathf.Abs(verticalAngle) <= minAngleToAttack)
            {
                Debug.Log("Ready for attack");
                playerOnSight = true;
            }
            else
            {
                playerOnSight = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    public void ReceiveDamage()
    {
        currentHealh--;
        //
        if (currentHealh < 3) fires[0].SetActive(true);
        if (currentHealh < 2) fires[1].SetActive(true);
        if (currentHealh < 1) fires[2].SetActive(true);
        //
        if (currentHealh <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //
        if (currentFormation)
        {
            currentFormation.EngageInCombat();
            currentFormation.RemoveMember(this);
            currentFormation = null;
        }
        //
        objective.targeteable = false;
        trail.SetActive(false);
        explosion.SetActive(true);
        smokeTrail.SetActive(true);
        currentState = PlaneState.Dead;
        Destroy(gameObject, 20f);
        CameraControl.Instance.UnLockObjective();
    }

    IEnumerator AttackCycle()
    {
        while(currentState != PlaneState.Dead)
        {
            yield return new WaitForSeconds(fireRate);
            if (playerOnSight)
            {
                for(int i = 0; i < shootPoints.Length; i++)
                {
                    Debug.Log("Shooting bullet - " + i);
                    GameObject newBullet = Instantiate(bulletPrefab, shootPoints[i].position, shootPoints[i].rotation);                    
                }
            }
        }
    }
}
