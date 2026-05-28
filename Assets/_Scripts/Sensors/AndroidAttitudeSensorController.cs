using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;

public class AndroidAttitudeSensorExample
{
    public AttitudeSensor attitudeSensor;

    public void Start()
    {
        attitudeSensor = InputSystem.GetDevice<AndroidGameRotationVector>();

        if (attitudeSensor == null)
        {
            attitudeSensor = InputSystem.GetDevice<AndroidRotationVector>();
        }

        if (attitudeSensor != null)
        {
            InputSystem.EnableDevice(attitudeSensor);
        }
    }

    public void Update()
    {
        if (attitudeSensor != null && attitudeSensor.enabled)
        {
            Quaternion deviceOrientation = attitudeSensor.attitude.ReadValue();
        }
    }
}
