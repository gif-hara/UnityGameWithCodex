using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace UnityGameWithCodex
{
    public class Player : MonoBehaviour
    {
        private static readonly Vector3 ViewportCenter = new(0.5f, 0.5f, 0f);
        private const float MaxShotDistance = 1000f;

        [SerializeField] private Camera targetCamera;
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float bulletSpeed = 40f;
        [SerializeField] private float bulletLifetime = 3f;

        private float pitch;
        private float yaw;
        private ObjectPool<Bullet> bulletPool;

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

            bulletPool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet);
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

            Ray centerRay = targetCamera.ViewportPointToRay(ViewportCenter);
            Vector3 targetPoint = centerRay.origin + (centerRay.direction * MaxShotDistance);

            if (Physics.Raycast(centerRay, out RaycastHit hitInfo, MaxShotDistance))
            {
                targetPoint = hitInfo.point;
            }

            Vector3 direction = (targetPoint - muzzleTransform.position).normalized;
            Bullet bullet = bulletPool.Get();
            bullet.transform.SetPositionAndRotation(muzzleTransform.position, Quaternion.LookRotation(direction));
            bullet.Launch(direction, bulletSpeed, bulletLifetime);
        }

        private Bullet CreateBullet()
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.SetPool(bulletPool);
            bullet.gameObject.SetActive(false);
            return bullet;
        }

        private static void OnGetBullet(Bullet bullet)
        {
            bullet.gameObject.SetActive(true);
        }

        private static void OnReleaseBullet(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }

        private static void OnDestroyBullet(Bullet bullet)
        {
            Destroy(bullet.gameObject);
        }

        private void OnDestroy()
        {
            bulletPool?.Clear();
        }
    }
}
