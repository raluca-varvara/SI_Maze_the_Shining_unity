using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] public Transform character;
    [SerializeField] public CinemachineFreeLook freeLookCamera;
    public float rotationSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // // Get the forward direction of the character
        // Vector3 characterForward = character.forward;
        // characterForward.y = 0;  // Ignore the y-axis for horizontal alignment

        // // Calculate the desired rotation angle around the y-axis
        // float targetYaw = Quaternion.LookRotation(characterForward).eulerAngles.y;

        // // Smoothly interpolate the camera's current yaw to the target yaw
        // freeLookCamera.m_XAxis.Value = Mathf.LerpAngle(
        //     freeLookCamera.m_XAxis.Value,
        //     targetYaw,
        //     rotationSpeed * Time.deltaTime
        // );
    }
}
