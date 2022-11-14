using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    //
    public GameObject fireBallPrefab;
    public Transform shootPoint;

    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    void SpawnFireball()
    {
        GameObject fireBall = Instantiate(fireBallPrefab, shootPoint.position, shootPoint.rotation);
    }
}
