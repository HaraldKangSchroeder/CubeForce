using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDeleteAnimation : MonoBehaviour
{
    public float explosion_strength_max;
    public float explosion_strength_min;

    public void beginDestroyAnimation(){
        Vector3 projectile_position = transform.position;
        Vector3 projectile_rotation = transform.rotation.eulerAngles;
        Material projectile_material = GetComponent<MeshRenderer>().material;
        GameObject death_object = Instantiate(GetComponent<ProjectileAttributes>().explosion_cube,
                            new Vector3(projectile_position.x,projectile_position.y,projectile_position.z), 
                            Quaternion.Euler(projectile_rotation.x,projectile_rotation.y,projectile_rotation.z));

        foreach(Transform child in death_object.transform)
        {
            child.gameObject.transform.GetComponent<MeshRenderer>().material = projectile_material;
            
            child.gameObject.transform.GetComponent<Rigidbody>().AddForce(new Vector3((2.5f* Random.Range(-0.5f,0.5f)) * Random.Range(explosion_strength_min,explosion_strength_max),
                                                                                        0.25f * Random.Range(explosion_strength_min,explosion_strength_max),
                                                                                        (2.5f * Random.Range(-0.5f,0.5f)) * Random.Range(explosion_strength_min,explosion_strength_max)));
                                                                                                        
            
        }
    }   

    public IEnumerator removeProjectileRemains(){
        Debug.Log("test");
        Vector3 projectile_position = transform.position;
        Vector3 projectile_rotation = transform.rotation.eulerAngles;
        Material projectile_material = GetComponent<MeshRenderer>().material;
        GameObject death_object = Instantiate(GetComponent<ProjectileAttributes>().explosion_cube,
                            new Vector3(projectile_position.x,projectile_position.y,projectile_position.z), 
                            Quaternion.Euler(projectile_rotation.x,projectile_rotation.y,projectile_rotation.z));

        foreach(Transform child in death_object.transform)
        {
            child.gameObject.transform.GetComponent<MeshRenderer>().material = projectile_material;
            
            child.gameObject.transform.GetComponent<Rigidbody>().AddForce(new Vector3((2.5f* Random.Range(-0.5f,0.5f)) * Random.Range(explosion_strength_min,explosion_strength_max),
                                                                                        0.25f * Random.Range(explosion_strength_min,explosion_strength_max),
                                                                                        (2.5f * Random.Range(-0.5f,0.5f)) * Random.Range(explosion_strength_min,explosion_strength_max)));
                                                                                                        
            
        }
        yield return new WaitForSeconds(5);
        Destroy(death_object);
        Destroy(gameObject);
        yield return null;
    }

}
