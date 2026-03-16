using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UnityGameWithCodex
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private Transform handTransform;
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private float bulletSpeed = 40f;
        [SerializeField] private float bulletLifetime = 3f;
        [SerializeField] private Vector3 handOffset = new(0.35f, -0.35f, 0.8f);

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

            EnsureCrosshair();
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

            UpdateHandPose();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Shoot();
            }
        }

        private void UpdateHandPose()
        {
            if (handTransform == null)
            {
                return;
            }

            Transform cameraTransform = targetCamera.transform;
            handTransform.position = cameraTransform.TransformPoint(handOffset);
            handTransform.rotation = cameraTransform.rotation;
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

        private void EnsureCrosshair()
        {
            if (GameObject.Find("CrosshairCanvas") != null)
            {
                return;
            }

            GameObject canvasObject = new("CrosshairCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            CreateCrosshairBar(canvasObject.transform, "CrosshairVertical", new Vector2(4f, 24f));
            CreateCrosshairBar(canvasObject.transform, "CrosshairHorizontal", new Vector2(24f, 4f));
        }

        private static void CreateCrosshairBar(Transform parent, string objectName, Vector2 size)
        {
            GameObject barObject = new(objectName, typeof(RectTransform), typeof(Image));
            barObject.transform.SetParent(parent, false);

            RectTransform rectTransform = barObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = size;

            Image image = barObject.GetComponent<Image>();
            image.color = Color.red;
        }
    }
}
