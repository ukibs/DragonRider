using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    //
    [Header("Fireball Components")]
    public GameObject fireBallPrefab;
    public Transform shootPoint;
    [Header("Machinegun Components")]
    public Transform[] shootPoints;
    public GameObject bulletPrefab;
    [Header("Camera Components")]
    public CameraControl cameraControl;
    [Header("Parameters")]
    public float mgFireRate = 0.5f;

    //
    private Rigidbody rb;
    private bool machineGunShooting = false;
    private Coroutine machineGunShootingCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //
        if (InputController.Instance.BPressed)
        {
            SpawnFireball();
        }
        else if (InputController.Instance.BReleased)
        {
            
        }

        //
        if (InputController.Instance.XPressed)
        {
            SpawnBullets();
            //machineGunShooting = true;
            if (!machineGunShooting)
            {
                machineGunShooting = true;
                machineGunShootingCoroutine = StartCoroutine(MgAttackCycle());
            }            
        }
        else if (InputController.Instance.XReleased)
        {
            machineGunShooting = false;
        }

        //
        if (InputController.Instance.YPressed)
        {
            cameraControl.LockOnObjective();
        }
        else if (InputController.Instance.YReleased)
        {

        }
    }

    void SpawnFireball()
    {
        GameObject fireBall = Instantiate(fireBallPrefab, shootPoint.position, shootPoint.rotation);
        FireBallController fireBallController = fireBall.GetComponent<FireBallController>();
        //
        if (cameraControl.CurrentObjective)
        {
            //
            Vector3 positionToLook = cameraControl.CurrentObjective.position;
            //
            PlaneController planeController = cameraControl.CurrentObjective.GetComponent<PlaneController>();
            if (planeController)
            {
                float travelTime = GeneralFunctions.EstimateTimeBetweenTwoPoints(shootPoint.position, positionToLook, fireBallController.movementSpeed);
                positionToLook = GeneralFunctions.EstimateFuturePosition(positionToLook,
                    cameraControl.CurrentObjective.forward * planeController.movementSpeed, travelTime);
            }
            //
            fireBall.transform.LookAt(positionToLook);
        }
    }

    void SpawnBullets()
    {
        //
        for(int i = 0; i < shootPoints.Length; i++)
        {
            SpawnBullet(i);
        }        
    }

    void SpawnBullet(int index)
    {
        //
        GameObject bullet = Instantiate(bulletPrefab, shootPoints[index].position, shootPoints[index].rotation);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        //
        if (cameraControl.CurrentObjective)
        {
            //
            Vector3 positionToLook = cameraControl.CurrentObjective.position;
            //
            PlaneController planeController = cameraControl.CurrentObjective.GetComponent<PlaneController>();
            if (planeController)
            {
                float travelTime = GeneralFunctions.EstimateTimeBetweenTwoPoints(shootPoint.position, positionToLook, bulletController.movementSpeed);
                positionToLook = GeneralFunctions.EstimateFuturePosition(positionToLook,
                    cameraControl.CurrentObjective.forward * planeController.movementSpeed, travelTime);
            }
            //
            bullet.transform.LookAt(positionToLook);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger with " + other.name);

        PlaneController planeController = other.GetComponentInParent<PlaneController>();
        if (planeController)
        {
            planeController.Die();
            // TODO: Sufrir daño con esto
            // TODO: Cuando metamos animación de embestida, en ese caso no
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with " + collision.collider.name);

        PlaneController planeController = collision.collider.GetComponentInParent<PlaneController>();
        if (planeController)
        {
            planeController.Die();
            rb.velocity = Vector3.zero;
            // TODO: Sufrir daño con esto
            // TODO: Cuando metamos animación de embestida, en ese caso no
        }
    }

    IEnumerator MgAttackCycle()
    {
        while (machineGunShooting)
        {
            yield return new WaitForSeconds(mgFireRate);
            SpawnBullets();
        }
    }
}
