using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField] public CharacterController controller;
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    float angle;
    Vector3 lastDirection;
    Vector3 direction;

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, -2.5f, vertical).normalized;

        if(!horizontal.Equals(0) || !vertical.Equals(0))
        {
            float targetAngle = Mathf.Atan2(lastDirection.x, lastDirection.z) * Mathf.Rad2Deg;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        }

        if (direction.magnitude >= 0.5f)
        {
            lastDirection = direction;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            controller.Move(direction * speed * Time.deltaTime);
        }
        else if (lastDirection.magnitude >= 0.5f)
        {
            
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }
}