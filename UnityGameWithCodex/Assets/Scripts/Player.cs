using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

namespace UnityGameWithCodex
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float bulletSpeed = 40f;
        [SerializeField] private float bulletLifetime = 3f;

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
            Assert.IsNotNull(bulletPrefab, "Player requires a bullet prefab.");

            if (muzzleTransform == null || bulletPrefab == null)
            {
                return;
            }

            Ray centerRay = targetCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 targetPoint = centerRay.origin + centerRay.direction * 1000f;

            if (Physics.Raycast(centerRay, out RaycastHit hitInfo, 1000f))
            {
                targetPoint = hitInfo.point;
            }

            Vector3 direction = (targetPoint - muzzleTransform.position).normalized;
            Bullet bullet = Instantiate(bulletPrefab, muzzleTransform.position, Quaternion.LookRotation(direction));
            bullet.Initialize(direction, bulletSpeed, bulletLifetime);
        }
    }
}
