using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    //
    public GameObject[] prefabsToSpawn;
    public int amountToSpawn;
    public Vector3 maxSpaceToSpawn = new Vector3(500, 500, 500);
    public Vector2 possibleScales = new Vector2(1,1);
    public bool randomRotation = false;

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
            int prefabIndexToSpawn = Random.Range(0, prefabsToSpawn.Length);
            //
            GameObject newObject = Instantiate(prefabsToSpawn[prefabIndexToSpawn],
                new Vector3(Random.Range(-maxSpaceToSpawn.x, maxSpaceToSpawn.x), Random.Range(-maxSpaceToSpawn.x, maxSpaceToSpawn.x), Random.Range(-maxSpaceToSpawn.x, maxSpaceToSpawn.x)),
                Quaternion.identity);
            //
            if(randomRotation)
                newObject.transform.eulerAngles = new Vector3 (Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            //
            newObject.transform.parent = transform;
            //
            float newScale = Random.Range(possibleScales.x, possibleScales.y);
            newObject.transform.localScale = Vector3.one * newScale;
        }
    }
}
