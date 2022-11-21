using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //
    public RectTransform enemyMarkerRT;
    public GameObject lockOnImage;

    //
    private static CameraControl instance;
    private Camera cam;
    private Transform currentObjective;
    private Vector2 currentObjectiveScreenCoordinates;
    private Transform lockedObjective;

    //
    public static CameraControl Instance { get { return instance; } }
    public Transform CurrentObjective { get { return currentObjective; } }
    public Transform LockedObjective { get { return lockedObjective; } }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotation();
        //
        if (lockedObjective != null)
        {
            currentObjective = lockedObjective;
        }
        else
        {
            //
            currentObjective = GetNearestObjectiveToScreenCenter();
        }
        //
        if (currentObjective)
        {
            currentObjectiveScreenCoordinates = cam.WorldToScreenPoint(currentObjective.position);
            enemyMarkerRT.anchoredPosition = currentObjectiveScreenCoordinates;
            enemyMarkerRT.gameObject.SetActive(true);
        }
        else
        {
            enemyMarkerRT.gameObject.SetActive(false);
        }  
    }

    void UpdateRotation()
    {
        //
        Vector2 inputAxis = InputController.Instance.CameraAxis;
        Vector3 desiredEulers = new Vector3(inputAxis.y * 90, inputAxis.x * 180, 0);
        //
        Vector3 currentEulers = transform.localEulerAngles;
        if (currentEulers.x > 180) currentEulers.x -= 360;
        if (currentEulers.y > 180) currentEulers.y -= 360;
        //
        transform.localEulerAngles = Vector3.Lerp(currentEulers, desiredEulers, 0.05f);
    }

    public void LockOnObjective()
    {
        if (currentObjective != null && lockedObjective == null)
        {
            lockedObjective = currentObjective;
            lockOnImage.SetActive(true);
        }
        else
        {
            lockedObjective = null;
            lockOnImage.SetActive(false);
        }
            
    }

    public void UnLockObjective()
    {
        lockedObjective = null;
        lockOnImage.SetActive(false);
    }

    public Transform GetNearestObjectiveToScreenCenter()
    {
        //
        Transform selectedObjective = null;
        //Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        float nearestToCenter = Mathf.Infinity;
        //
        Objective[] possibleObjectives = FindObjectsOfType<Objective>();
        for (int i = 0; i < possibleObjectives.Length; i++)
        {
            //
            if (possibleObjectives[i].targeteable)
            {
                // Distancia al centro de pantalla
                Vector3 posInScreen = cam.WorldToViewportPoint(possibleObjectives[i].transform.position);
                float distanceToCenter = Mathf.Pow(posInScreen.x - 0.5f, 2) + Mathf.Pow(posInScreen.y - 0.5f, 2);
                // TODO: Peso extra según la etuiqueta
                bool inScreen = posInScreen.x >= 0 && posInScreen.x <= 1 &&
                    posInScreen.y >= 0 && posInScreen.y <= 1 &&
                    posInScreen.z > 0;
                //
                if (inScreen && distanceToCenter < nearestToCenter)
                {
                    nearestToCenter = distanceToCenter;
                    selectedObjective = possibleObjectives[i].transform;
                }
            }            
        }

        //
        return selectedObjective;
    }
}
