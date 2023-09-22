using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mainSpeed = 100.0f; // regular speed
    public float shiftAdd = 250.0f; // multiplied by how long shift is held (running)
    public float maxShift = 1000.0f; // Maximum speed when holding Shift
    public float camSens = 0.25f; // Mouse sensitivity
    public float scrollSens = 20.0f;
    private Vector3 lastMouse = new Vector3(255, 255, 255); // Middle of the screen
    private float totalRun = 1.0f;
    private Vector3 baseInput;

    void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {
        lastMouse = Input.mousePosition - lastMouse;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
        transform.eulerAngles = lastMouse;
        lastMouse = Input.mousePosition;

        // Keyboard commands
        baseInput = GetBaseInput();

        // Add scroll wheel input for Y-axis movement
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        baseInput += new Vector3(0, scrollInput * camSens * scrollSens, 0);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalRun += Time.deltaTime;
            baseInput = baseInput * totalRun * shiftAdd;
            baseInput.x = Mathf.Clamp(baseInput.x, -maxShift, maxShift);
            baseInput.y = Mathf.Clamp(baseInput.y, -maxShift, maxShift);
            baseInput.z = Mathf.Clamp(baseInput.z, -maxShift, maxShift);
        }
        else
        {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            baseInput = baseInput * mainSpeed;
        }

        baseInput = baseInput * Time.deltaTime;
        Vector3 newPosition = transform.position;

        if (Input.GetKey(KeyCode.Space)) // If player wants to move on X and Z axis only
        {
            transform.Translate(baseInput);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        else
        {
            transform.Translate(baseInput);
        }
    }

    private Vector3 GetBaseInput()
    {
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
    
}
