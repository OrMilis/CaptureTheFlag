using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {

    private GunScript gun;
    public Transform gunHolder;
    public LayerMask floorMask;
    public Rigidbody playerRigidbody;
    public Transform AimObj;
    public Vector3 lookPoint;
    private Health myHealth;
    public float camRayLength = 100f;

    // Use this for initialization
    void Start () {
        gun = GetComponentInChildren<GunScript>();
        myHealth = GetComponent<Health>();
	}
	
	// Update is called once per frame
	void Update () {
        // Create a ray from the mouse cursor on screen in the direction of the camera.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit variable to store information about what was hit by the ray.
        RaycastHit floorHit;

        // Perform the raycast and if it hits something on the floor layer...
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 playerToMouse = floorHit.point - transform.position;

            // Ensure the vector is entirely along the floor plane.
            playerToMouse.y = 0f;

            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

            // Set the player's rotation to this new rotation.
            transform.LookAt(new Vector3(AimObj.position.x, transform.position.y, AimObj.position.z));
            //playerRigidbody.MoveRotation(newRotation);
            gunHolder.transform.rotation.SetLookRotation(playerToMouse);

            AimObj.transform.position = new Vector3(floorHit.point.x, 1, floorHit.point.z);
            lookPoint = floorHit.point;
        }

        if (Input.GetMouseButtonDown(0) && !myHealth.isDead())
        {
            gun.shootCommand(true);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            gun.shootCommand(false);
        }
    }
}
