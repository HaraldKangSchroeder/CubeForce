using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public Quaternion target_rotation;
    private int update_step = 0;
    private float direction_factor = 1;


    // id of the layer belonging to the helper plane (exists in all arenas) in order to be able to rotate the player correctly so that he always points perfectly to the mouse
    // (considering a perspective view which is the case here)
    private int layerMask = 1 << 8;

    void Start(){
        StartCoroutine(send(0.033f));
    }
    
    // updates the angle that indicates how much the player should rotate in order to point to the mouse
    void Update()
    {
        if(transform.GetComponent<PlayerAttributes>().id == Client.id){
            RaycastHit hit;
            Ray ray = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity,layerMask)) {
                Transform objectHit = hit.transform;
                
                Vector3 dir_3d = (hit.point - transform.position).normalized;
                Vector3 front_3d = new Vector3(1,0,0);

                Vector3 dir_2d = new Vector2(dir_3d.x,dir_3d.z);
                Vector3 front_2d  = new Vector2(front_3d.x,front_3d.z);

                float angle = -Vector2.SignedAngle(front_2d,dir_2d);
                transform.GetComponent<PlayerAttributes>().angle_to_mouse = angle;
            }
        }

        if(update_step > 0){
            update_step--;
            transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, direction_factor);
        }
        // due to floating point calculation errors?
        else{
            transform.rotation = target_rotation;
        }
    }

    private IEnumerator send(float delay){
        Packet packet = PacketManufacturer.playerMouseRotationAngle(Client.id);
        GameObject.Find("Client").GetComponent<Client>().sendMessage(packet);
        yield return new WaitForSeconds(delay);
        StartCoroutine(send(delay));
    }




    public void updateRotation(Quaternion rotation){
        transform.rotation = target_rotation;
        target_rotation = rotation;

        update_step = GameObject.Find("Game").GetComponent<UpdateFrequencyTracker>().getAverageUpdateSteps();
        direction_factor = update_step != 0 ? 1f/update_step : 1f;
        //Debug.Log(direction_factor);
    }


}
