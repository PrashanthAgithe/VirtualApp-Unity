using UnityEngine;
using StarterAssets;

public class ARCameraLink : MonoBehaviour
{
    [Header("References")]
    public Transform arCamera;                // Assign XR Origin → Main Camera
    public StarterAssetsInputs starterInputs; // Auto-fetched if left null
    public FirstPersonController controller;  // Auto-fetched if left null

    void Awake()
    {
        // Auto-fetch references if not set manually
        if (starterInputs == null) starterInputs = GetComponent<StarterAssetsInputs>();
        if (controller == null) controller = GetComponent<FirstPersonController>();
    }

    void LateUpdate()
    {
        if (arCamera == null || controller == null) return;

        // Sync the FirstPersonController's CinemachineCameraTarget with AR Camera rotation
        if (controller.CinemachineCameraTarget != null)
        {
            controller.CinemachineCameraTarget.transform.rotation = arCamera.rotation;
        }
    }
}


