using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputInterface : MonoBehaviour
{
    [SerializeField]
    Joystick attitudeJoystick;

    [SerializeField]
    Joystick altitudeJoystick;

    FlightController flightController;
    [SerializeField]
    float altitudeTarget = 0f;

    [SerializeField]
    Vector2 attitudeTarget = new Vector3(0, 0);
    [SerializeField]
    float joystickSensitivity = 0.1f;
    [SerializeField]
    float attitudeJoystickSensitivity = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        flightController = FindObjectOfType<FlightController>();   
    }

    // Update is called once per frame
    void Update()
    {
        

        if (altitudeJoystick.Vertical != 0) { 
            altitudeTarget += joystickSensitivity * altitudeJoystick.Vertical * Time.deltaTime;
            flightController.setAltitudeTarget(altitudeTarget);
            Debug.Log(altitudeTarget);
        }

        
            attitudeTarget.x = attitudeJoystick.Vertical * attitudeJoystickSensitivity;
            attitudeTarget.y = -attitudeJoystick.Horizontal * attitudeJoystickSensitivity;
            flightController.setAttitudeTarget(attitudeTarget);
            Debug.Log(altitudeTarget);
        
    }
}
