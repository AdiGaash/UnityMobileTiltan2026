using UnityEngine;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope; 

public class SensorController : MonoBehaviour
{

    void Start()
    {
        // 1. Check and enable basic sensors
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
        }

        if (Gyroscope.current != null)
        {
            InputSystem.EnableDevice(Gyroscope.current);
        }

        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
        }
    }

    void Update()
    {
        // Example: Reading Gyroscope values directly
        if (Gyroscope.current != null && Gyroscope.current.enabled)
        {
            Vector3 rotationVelocity = Gyroscope.current.angularVelocity.ReadValue();
            Debug.Log($"Gyroscope - Angular Velocity: x={rotationVelocity.x}, y={rotationVelocity.y}, z={rotationVelocity.z}");
        }

        // Reading Accelerometer values
        if (Accelerometer.current != null && Accelerometer.current.enabled)
        {
            Vector3 acceleration = Accelerometer.current.acceleration.ReadValue();
            Debug.Log($"Accelerometer - Acceleration: x={acceleration.x}, y={acceleration.y}, z={acceleration.z}");
        }

        // Reading Gravity values
        if (GravitySensor.current != null && GravitySensor.current.enabled)
        {
            Vector3 gravity = GravitySensor.current.gravity.ReadValue();
            Debug.Log($"Gravity Sensor - Gravity: x={gravity.x}, y={gravity.y}, z={gravity.z}");
        }

        // Reading Attitude Sensor values
        if (AttitudeSensor.current != null && AttitudeSensor.current.enabled)
        {
            Quaternion attitude = AttitudeSensor.current.attitude.ReadValue();
            Debug.Log($"Attitude Sensor - Attitude: x={attitude.x}, y={attitude.y}, z={attitude.z}, w={attitude.w}");
        }

        // Reading Linear Acceleration values
        if (LinearAccelerationSensor.current != null && LinearAccelerationSensor.current.enabled)
        {
            Vector3 linearAcceleration = LinearAccelerationSensor.current.acceleration.ReadValue();
            Debug.Log($"Linear Acceleration Sensor - Acceleration: x={linearAcceleration.x}, y={linearAcceleration.y}, z={linearAcceleration.z}");
        }
    }
}