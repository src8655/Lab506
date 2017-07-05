using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cammove : MonoBehaviour {

    // Use this for initialization

	// Update is called once per frame
	void Update () {
             if (main_script.predicted == 1 && main_script.L_Hand != null && main_script.R_Hand == null)
             {
                 transform.position = new Vector3(0.02664f, 9.0f, -10.0f);

             }

        if (main_script.predicted == 0 && main_script.L_Hand != null && main_script.R_Hand != null)
        {
            transform.position = new Vector3(0.02664f, 7.0f, -8.0f);
        }

        if (main_script.predicted == 1 && main_script.L_Hand != null && main_script.R_Hand != null)
        {
            transform.position = new Vector3(0.02664f, 14.0f, -13.0f);
        }
   
    }
}
