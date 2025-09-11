using UnityEngine;
using StarterAssets;

public class GyroIntegrator : MonoBehaviour
{
    public StarterAssetsInputs starterAssetsInputs; // assign PlayerCapsule
    public float sensitivity = 1.0f;
    public float deadzoneDeg = 1f;

    private bool gyroOn;
    private Quaternion integrated = Quaternion.identity;

    void Start()
    {
        gyroOn = SystemInfo.supportsGyroscope;
        if (gyroOn) Input.gyro.enabled = true;
        integrated = Quaternion.identity;
    }

    void Update()
    {
        if (!gyroOn || starterAssetsInputs == null) return;

        // read angular velocity (rad/sec). Convert to degrees
        Vector3 angVel = Input.gyro.rotationRateUnbiased; // rad/s
        // convert rad->deg
        Vector3 angDegPerSec = angVel * Mathf.Rad2Deg;

        // integrate to get delta rotation for this frame
        Vector3 deltaDeg = angDegPerSec * Time.deltaTime;
        Quaternion deltaQ = Quaternion.Euler(deltaDeg);

        integrated = deltaQ * integrated;

        // compute euler and map to look input
        Vector3 euler = integrated.eulerAngles;
        // normalize to -180..180
        euler.x = (euler.x > 180 ? euler.x - 360f : euler.x);
        euler.y = (euler.y > 180 ? euler.y - 360f : euler.y);

        if (Mathf.Abs(euler.y) < deadzoneDeg) euler.y = 0;
        if (Mathf.Abs(euler.x) < deadzoneDeg) euler.x = 0;

        Vector2 look = new Vector2(euler.y, -euler.x) * (sensitivity * Time.deltaTime);

        starterAssetsInputs.LookInput(look);
    }
}
