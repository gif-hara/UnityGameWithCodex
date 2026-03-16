using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

namespace UnityGameWithCodex
{
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

            Assert.IsNotNull(targetCamera, "Player requires a target camera.");

            if (targetCamera == null)
            {
                enabled = false;
                return;
            }

            Vector3 currentEulerAngles = targetCamera.transform.localEulerAngles;
            pitch = currentEulerAngles.x.NormalizeAngle();
            yaw = currentEulerAngles.y.NormalizeAngle();
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
    }
}
