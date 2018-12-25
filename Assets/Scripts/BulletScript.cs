using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public Team teamBullet;
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        var obj = collision.gameObject;
        var health = obj.GetComponent<Health>();
        if(health != null)
        {
            Team collisionTeam = obj.GetComponent<FlagCaptureScript>().playerTeam;
            if (teamBullet != collisionTeam)
            {
                health.takeDamage(damage);
            }
        }
        Destroy(this.gameObject);
    }
}
