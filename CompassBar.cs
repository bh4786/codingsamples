using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassBar : MonoBehaviour
{
    [SerializeField] private GameObject compassBar = null;

    private float yRot;
    void Update()
    {
        if (yRot < 0) //y rotation is usually between -180 and 180 -- this clamps it to a normal 0-360 value
        {
            yRot += 360; 
        }
        else
        {
            yRot = transform.rotation.eulerAngles.y;
        }

        compassBar.transform.localPosition = new Vector3(yRot * -20/9 + 800, compassBar.transform.localPosition.y, compassBar.transform.localPosition.z); //set compass bar x value using linear equation with the players current y rotation
    }
}
