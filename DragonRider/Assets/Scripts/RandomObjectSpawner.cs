using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    //
    public GameObject prefabToSpawn;
    public int amountToSpawn;
    public Vector3 maxSpaceToSpawn = new Vector3(500, 500, 500);

    // Start is called before the first frame update
    void Start()
    {
        SpawnObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObjects()
    {
        for(int i = 0; i < amountToSpawn; i++)
        {
            //
            GameObject newObject = Instantiate(prefabToSpawn,
                new Vector3(Random.Range(-maxSpaceToSpawn.x, maxSpaceToSpawn.x), Random.Range(-maxSpaceToSpawn.x, maxSpaceToSpawn.x), Random.Range(-maxSpaceToSpawn.x, maxSpaceToSpawn.x)),
                Quaternion.identity);
            //
            newObject.transform.eulerAngles = new Vector3 (Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            //
            float newScale = Random.Range(2, 50);
            newObject.transform.localScale = Vector3.one * newScale;
        }
    }
}
