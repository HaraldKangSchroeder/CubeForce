using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MunitionUI : MonoBehaviour
{
    public GameObject munition_image_prefab;
    public float step_size;

    public void updateMunitionUI(int munition, Material material)
    {
        if (transform.childCount != munition)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < munition; i++)
            {
                GameObject munition_image = Instantiate(munition_image_prefab, new Vector3(transform.position.x - step_size * i, transform.position.y, 0), Quaternion.identity);
                munition_image.transform.SetParent(transform);
                munition_image.GetComponent<Image>().material = material;
            }
            //
        }
    }
}
