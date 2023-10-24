using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public float baseMoveSpeed = 30.0f; // Movement speed
    public float mouseSensitivity = 4.0f; // Mouse sensitivity
    public float maxYAngle = 120.0f; // Clamping the vertical angle

    private float pitch = 0.0f; // Pitch rotation
    private float yaw = 0.0f; // Yaw rotation

    void Update()
    {
        float moveSpeed = baseMoveSpeed;
        if (Time.timeScale > 1)
        {
            moveSpeed = baseMoveSpeed / Time.timeScale;
        }
 
        // Getting user inputs
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculating the yaw and pitch rotation based on mouse input
        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -maxYAngle, maxYAngle);
        
        // Apply the rotations to the camera
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        // Calculating the forward and side movements
        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;

        // Translating the movement in the direction camera is facing
        transform.Translate(movement);
    }
}