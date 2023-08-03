using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{
    //
    public GameObject explosionPrefab;
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
        Debug.Log("Trigger with " + other.name);

        //
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);

        //
        //PlaneController planeController = other.GetComponent<PlaneController>();
        //if (planeController != null) planeController.Die();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with " + collision.collider.name);

        //
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);

        //
        PlaneController planeController = collision.collider.GetComponentInParent<PlaneController>();
        if (planeController != null) planeController.Die();
    }
}
