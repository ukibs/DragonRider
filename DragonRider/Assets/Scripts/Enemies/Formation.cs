using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{
    //
    [Header("Compoments")]
    public GameObject objectPrefab;
    [Header("Parameters")]
    public Vector3[] formationPositions;

    //
    private List<PlaneController> planeControllers;

    //
    public List<PlaneController> PlaneControllers { get { return planeControllers; } }

    // Start is called before the first frame update
    void Start()
    {
        SpawnFormationMembers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnFormationMembers()
    {
        planeControllers = new List<PlaneController>(formationPositions.Length);
        for (int i = 0; i < formationPositions.Length; i++)
        {
            PlaneController planeController = Instantiate(objectPrefab, transform.position + formationPositions[i], Quaternion.identity).GetComponent<PlaneController>();
            planeController.CurrentFormation = this;
            planeControllers.Add(planeController);
        }
    }

    public Vector3 FormationPosition(int index)
    {
        return planeControllers[0].transform.TransformPoint(formationPositions[index]);
    }

    public void RemoveMember(PlaneController deadPlane)
    {
        planeControllers.Remove(deadPlane);
    }

    public void EngageInCombat()
    {
        for(int i = 0; i < planeControllers.Count; i++)
        {
            planeControllers[i].CurrentState = PlaneController.PlaneState.Engaged;
        }
    }
}
