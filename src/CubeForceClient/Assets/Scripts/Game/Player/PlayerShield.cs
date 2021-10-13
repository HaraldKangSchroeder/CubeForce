using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public GameObject shield;

    // shows or hides the shield gameobject depending on its activation state
    void Update()
    {
        // if(GetComponent<PlayerAttributes>().id == Client.id){
        //     if(Input.GetKeyDown(KeyCode.LeftAlt)){
                
        //     }
        //     if(Input.GetKeyUp(KeyCode.LeftAlt)){
                
        //     }
        // }

        if(transform.GetComponent<PlayerAttributes>().shield_active){
            shield.SetActive(true);
        }
        else{
            shield.SetActive(false);
        }
    }
}
