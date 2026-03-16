using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float sensitivity = 0.1f;

    private float pitch;
    private float yaw;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponentInChildren<Camera>();
        }

        if (targetCamera != null)
        {
            Vector3 currentEulerAngles = targetCamera.transform.localEulerAngles;
            pitch = NormalizeAngle(currentEulerAngles.x);
            yaw = NormalizeAngle(currentEulerAngles.y);
        }
    }

    private void Update()
    {
        if (targetCamera == null || Mouse.current == null)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        yaw += mouseDelta.x * sensitivity;
        pitch -= mouseDelta.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        targetCamera.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private static float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }
}
