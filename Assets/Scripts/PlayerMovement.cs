using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float movementSpeed;

    private Vector3 movementInput;
    private Vector3 movementVelocity;

    private Rigidbody rb3d;

    public Animator m_Animator;

    private PlayerShooting aim;
    private Health myHealth;

    private void Start()
    {
        rb3d = GetComponent<Rigidbody>();
        aim = GetComponent<PlayerShooting>();
        myHealth = GetComponent<Health>();
    }

    void Update()
    {
        if (!myHealth.isDead())
        {
            float verticalDir = Input.GetAxis("Vertical");
            float horizontalDir = -Input.GetAxis("Horizontal");
            movementInput = new Vector3(verticalDir,
                0,
                horizontalDir);
            movementVelocity = movementInput * movementSpeed;

            updateAnimator(verticalDir, horizontalDir);
        }
    }

    private void FixedUpdate()
    {
        if (!myHealth.isDead())
            rb3d.MovePosition(transform.position + movementVelocity * Time.deltaTime);
        else
            rb3d.MovePosition(transform.position);
        //rb3d.velocity = movementVelocity;
    }

    void updateAnimator(float verticalDir, float horizontalDir)
    {
        float forwardBackwardsMagnitude = 0;
        float rightLeftMagnitude = 0;
        if (movementInput.magnitude > 0)
        {
            Vector3 normalizedLookingAt = aim.lookPoint - transform.position;
            normalizedLookingAt.Normalize();
            forwardBackwardsMagnitude = Mathf.Clamp(
                    Vector3.Dot(movementInput, normalizedLookingAt), -1, 1
            );

            Vector3 perpendicularLookingAt = new Vector3(
                   normalizedLookingAt.z, 0, -normalizedLookingAt.x
            );
            rightLeftMagnitude = Mathf.Clamp(
                   Vector3.Dot(movementInput, perpendicularLookingAt), -1, 1
           );
        }

        // update the animator parameters
        m_Animator.SetFloat("Forward", forwardBackwardsMagnitude);
        m_Animator.SetFloat("Turn", rightLeftMagnitude);
    }

    public void dead()
    {
        m_Animator.SetTrigger("IsDead");
    }
}