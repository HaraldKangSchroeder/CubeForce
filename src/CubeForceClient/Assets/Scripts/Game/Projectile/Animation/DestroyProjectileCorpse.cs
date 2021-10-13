using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyProjectileCorpse : MonoBehaviour
{
    public float delay;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(destroyAfterDelay(delay));
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.transform.localScale = child.gameObject.transform.localScale * 0.995f;
        }
    }


    private IEnumerator destroyAfterDelay(float delay){
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
