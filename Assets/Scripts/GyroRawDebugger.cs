using UnityEngine;

/// <summary>
/// Very small runtime sensor debugger for Android devices.
/// Prints gyro attitude, angular velocity (rotationRateUnbiased) and accelerometer to Logcat every frame.
/// Attach to a GameObject and Build & Run to device. Check Android Logcat in Unity for output.
/// </summary>
public class GyroRawDebugger : MonoBehaviour
{
    void Start()
    {
        bool hasGyro = SystemInfo.supportsGyroscope;
        Debug.Log($"GyroRawDebugger: supportsGyroscope = {hasGyro}");

        if (hasGyro)
        {
            // enable the gyro so attitude/rotationRate data will start updating
            Input.gyro.enabled = true;
            Debug.Log("GyroRawDebugger: Input.gyro.enabled = true");
        }
    }

    void Update()
    {
        // Print every frame — you can reduce frequency if Logcat gets too noisy
        if (SystemInfo.supportsGyroscope)
        {
            Quaternion att = Input.gyro.attitude;                     // quaternion (may be (0,0,0,0) on some combos)
            Vector3 rotRate = Input.gyro.rotationRateUnbiased;        // rad/s (angular velocity)
            Vector3 acc = Input.acceleration;                         // g units

            Debug.Log($"GyroRawDebugger: attitude={att}  rotRate={rotRate}  acc={acc}");
        }
        else
        {
            // If no gyro, still print accelerometer to help debugging
            Vector3 acc = Input.acceleration;
            Debug.Log($"GyroRawDebugger: NO GYRO - acc={acc}");
        }
    }
}