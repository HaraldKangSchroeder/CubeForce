using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;


public class PlayerDeathAnimation : MonoBehaviour
{

    public float explosion_strength_min;
    public float explosion_strength_max;

    private bool was_killed = false;

    void Update(){
        PlayerAttributes player_attributes = GetComponent<PlayerAttributes>();

        if(!player_attributes.alive && !was_killed){
            // GameObject death_object = Instantiate(transform.GetComponent<PlayerAttributes>().explosion_cube,
            //                             new Vector3(player_attributes.death_position.x,player_attributes.death_position.y,player_attributes.death_position.z), 
            //                             Quaternion.Euler(player_attributes.death_rotation.x,player_attributes.death_rotation.y,player_attributes.death_rotation.z));

            GameObject death_object = Instantiate(transform.GetComponent<PlayerAttributes>().explosion_cube,
                                        transform.position, 
                                        transform.rotation);

            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
            // Play death sound
            FindObjectOfType<AudioManager>().Play("DeathScream");

            foreach(Transform child in death_object.transform)
            {
                child.gameObject.transform.GetComponent<MeshRenderer>().material = transform.GetChild(0).transform.GetChild(0).GetComponent<MeshRenderer>().material;
                // child.gameObject.transform.GetComponent<Rigidbody>().AddForce(new Vector3((player_attributes.death_direction.x + Random.Range(-0.5f,0.5f)) * Random.Range(explosion_strength_min,explosion_strength_max),
                //                                                                             0.1f * Random.Range(explosion_strength_min,explosion_strength_max),
                //                                                                             (player_attributes.death_direction.z + Random.Range(-0.5f,0.5f)) * Random.Range(explosion_strength_min,explosion_strength_max)));

                child.gameObject.transform.GetComponent<Rigidbody>().AddForce(new Vector3((2.5f* Random.Range(-0.5f,0.5f)) * Random.Range(explosion_strength_min,explosion_strength_max),
                                                                                            0.25f * Random.Range(explosion_strength_min,explosion_strength_max),
                                                                                            (2.5f * Random.Range(-0.5f,0.5f)) * Random.Range(explosion_strength_min,explosion_strength_max)));
                                                                                                            
                
            }
        }
        was_killed = !player_attributes.alive;
    }

}