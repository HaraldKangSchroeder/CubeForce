using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeFootController : MonoBehaviour
{
    public float shift_size_factor;
    public GameObject HomeFootLeftBack;
    public GameObject HomeFootLeftFront;
    public GameObject HomeFootRightBack;
    public GameObject HomeFootRightFront;

    public GameObject PlayerObject;

    private Vector3 foot_left_back_home_default_position;
    private Vector3 foot_left_front_home_default_position;
    private Vector3 foot_right_back_home_default_position;
    private Vector3 foot_right_front_home_default_position;


    // Start is called before the first frame update
    void Start()
    {
        foot_left_back_home_default_position = HomeFootLeftBack.transform.localPosition;
        foot_left_front_home_default_position = HomeFootLeftFront.transform.localPosition;
        foot_right_back_home_default_position = HomeFootRightBack.transform.localPosition;
        foot_right_front_home_default_position = HomeFootRightFront.transform.localPosition;
    }
    
    // Update is called once per frame
    void Update()
    {
        Vector3 position_current = PlayerObject.transform.position;

        Vector3 dir = new Vector3();// = (position_current - position_previous).normalized;
        
        if(PlayerObject.GetComponent<PlayerAttributes>().key_move_up){
            dir += new Vector3(0,0,1);
            //Debug.Log("w pressed");
        }
        if(PlayerObject.GetComponent<PlayerAttributes>().key_move_left){
            dir += new Vector3(-1,0,0);
            //Debug.Log("a pressed");
        }
        if(PlayerObject.GetComponent<PlayerAttributes>().key_move_down){
            dir += new Vector3(0,0,-1);
            //Debug.Log("s pressed");
        }
        if(PlayerObject.GetComponent<PlayerAttributes>().key_move_right){
            dir += new Vector3(1,0,0);
            //Debug.Log("d pressed");
        }


        Vector3 dir_normalized = dir.normalized;

        Vector3 foot_left_back_home_world = PlayerObject.transform.TransformPoint(foot_left_back_home_default_position);
        Vector3 foot_left_front_home_world = PlayerObject.transform.TransformPoint(foot_left_front_home_default_position);
        Vector3 foot_right_back_home_world = PlayerObject.transform.TransformPoint(foot_right_back_home_default_position);
        Vector3 foot_right_front_home_world = PlayerObject.transform.TransformPoint(foot_right_front_home_default_position);

        foot_left_back_home_world += dir_normalized * shift_size_factor;
        foot_left_back_home_world = new Vector3(foot_left_back_home_world.x,0,foot_left_back_home_world.z); 
        
        foot_left_front_home_world += dir_normalized * shift_size_factor;
        foot_left_front_home_world = new Vector3(foot_left_front_home_world.x,0,foot_left_front_home_world.z); 

        foot_right_back_home_world += dir_normalized * shift_size_factor;
        foot_right_back_home_world = new Vector3(foot_right_back_home_world.x,0,foot_right_back_home_world.z); 

        foot_right_front_home_world += dir_normalized * shift_size_factor;
        foot_right_front_home_world = new Vector3(foot_right_front_home_world.x,0,foot_right_front_home_world.z); 

        HomeFootLeftBack.transform.localPosition = PlayerObject.transform.InverseTransformPoint(foot_left_back_home_world);
        HomeFootLeftFront.transform.localPosition = PlayerObject.transform.InverseTransformPoint(foot_left_front_home_world);
        HomeFootRightBack.transform.localPosition = PlayerObject.transform.InverseTransformPoint(foot_right_back_home_world);
        HomeFootRightFront.transform.localPosition = PlayerObject.transform.InverseTransformPoint(foot_right_front_home_world);

    }
}
