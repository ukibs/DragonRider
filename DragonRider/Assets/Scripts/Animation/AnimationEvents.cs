using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    //
    private FlyController flyController;

    // Start is called before the first frame update
    void Start()
    {
        flyController = GetComponentInParent<FlyController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlapForce()
    {
        flyController.AddVerticalSpeed();
    }

    public void OtherFunction()
    {

    }
}
