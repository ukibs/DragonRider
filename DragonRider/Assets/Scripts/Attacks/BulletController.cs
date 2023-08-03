using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    //
    public float movementSpeed = 100;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 20);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * movementSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet trigger with " + other.name);

        //
        Destroy(gameObject);

        //
        //PlaneController planeController = other.GetComponent<PlaneController>();
        //if (planeController != null) planeController.Die();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet collision with " + collision.collider.name);

        //
        Destroy(gameObject);

        //
        PlaneController planeController = collision.collider.GetComponentInParent<PlaneController>();
        if (planeController != null) planeController.ReceiveDamage();

        //
        HealthController healthController = collision.collider.GetComponentInParent<HealthController>();
        if (healthController) healthController.ReceiveDamage();
    }
}
