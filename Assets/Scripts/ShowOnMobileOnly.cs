using UnityEngine;

public class ShowOnMobileOnly : MonoBehaviour
{
    [Header("UI Elements to Hide on Mobile")]
    public GameObject jumpButton;
    public GameObject sprintButton;
    public GameObject lookJoystick;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Enable joystick canvas on Android
        gameObject.SetActive(true);

        // Hide unnecessary UI elements
        if (jumpButton) jumpButton.SetActive(false);
        if (sprintButton) sprintButton.SetActive(false);
        if (lookJoystick) lookJoystick.SetActive(false);

#else
        // Hide joystick canvas entirely on PC / Editor
        gameObject.SetActive(false);
#endif
    }
}
