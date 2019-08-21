using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameLightComponent : MonoBehaviour
{
    private Light myLight;
    
    void Start ()
    {
        myLight = GetComponent<Light>();
    }
    
    void Update ()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            myLight.enabled = true;
        }
        else
        {
            myLight.enabled = false;
        }
    }
}