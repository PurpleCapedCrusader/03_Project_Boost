using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cuteRocketExplode : MonoBehaviour
{
    public GameObject Explosion;

    private void OnTriggerEnter(Collider other)
    {

        GameObject explosion = Instantiate(Explosion, transform.position, Quaternion.identity);
        //this.GetComponent<Renderer>().enabled = false;
        // Destroy(this.gameObject, 0.25f);

        // Destroy(explosion, 2f);
    }
}
