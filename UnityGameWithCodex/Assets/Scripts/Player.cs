using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

namespace UnityGameWithCodex
{
    public class Player : MonoBehaviour
    {
        private static readonly Vector3 ViewportCenter = new(0.5f, 0.5f, 0f);
        private const float MaxShotDistance = 1000f;

        [SerializeField] private Camera targetCamera;
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private BulletPool bulletPool;

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

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            Assert.IsNotNull(muzzleTransform, "Player requires a muzzle transform.");
            Assert.IsNotNull(bulletPool, "Player requires a bullet pool.");

            if (muzzleTransform == null || bulletPool == null)
            {
                return;
            }

            Ray centerRay = targetCamera.ViewportPointToRay(ViewportCenter);
            Vector3 targetPoint = centerRay.origin + (centerRay.direction * MaxShotDistance);

            if (Physics.Raycast(centerRay, out RaycastHit hitInfo, MaxShotDistance))
            {
                targetPoint = hitInfo.point;
            }

            Vector3 direction = (targetPoint - muzzleTransform.position).normalized;
            bulletPool.Spawn(muzzleTransform.position, Quaternion.LookRotation(direction));
        }
    }
}
