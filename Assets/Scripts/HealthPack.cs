using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour {

    public int healthAmount = 40;
    public int timeToRespawn = 10;

    MeshRenderer myRenderer;
    BoxCollider myCollider;
    Light myLight;

    private void Start()
    {

        myRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<BoxCollider>();
        myLight = GetComponent<Light>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.gameObject;
        var health = obj.GetComponent<Health>();
        if (health != null)
        {
            if (health.takeHealthPack(healthAmount))
            {
                myRenderer.enabled = false;
                myCollider.enabled = false;
                myLight.enabled = false;
                StartCoroutine(healthPackRespawn());
            }
        }
    }

    IEnumerator healthPackRespawn()
    {
        yield return new WaitForSeconds(timeToRespawn);
        myLight.enabled = true;
        myRenderer.enabled = true;
        myCollider.enabled = true;
    }

}
