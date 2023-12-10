using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeppelinController : MonoBehaviour
{

    [Header("Parameters")]
    public int maxHealth = 3;
    public float movementSpeed;
    public float rotationSpeed;
    public float fireRate = 0.5f;

    //
    private Objective objective;
    private Vector3 nextPointToGo;
    //private PlaneState currentState = PlaneState.Engaged;
    private Formation currentFormation;
    private FlyController flyController;
    private bool playerOnSight = false;
    private int currentHealh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveDamage()
    {
        currentHealh--;
        //
        //if (currentHealh < 3) fires[0].SetActive(true);
        //if (currentHealh < 2) fires[1].SetActive(true);
        //if (currentHealh < 1) fires[2].SetActive(true);
        //
        if (currentHealh <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //
        objective.targeteable = false;
        //trail.SetActive(false);
        //explosion.SetActive(true);
        //smokeTrail.SetActive(true);
        //currentState = PlaneState.Dead;
        Destroy(gameObject, 20f);
        CameraControl.Instance.UnLockObjective();
    }

}
