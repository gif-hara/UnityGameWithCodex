using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace UnityGameWithCodex
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField] private Bullet bulletPrefab;

        private ObjectPool<Bullet> pool;

        private void Awake()
        {
            Assert.IsNotNull(bulletPrefab, "BulletPool requires a bullet prefab.");

            if (bulletPrefab == null)
            {
                enabled = false;
                return;
            }

            pool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet);
        }

        public Bullet Spawn(Vector3 position, Quaternion rotation)
        {
            Assert.IsNotNull(pool, "BulletPool is not initialized.");

            Bullet bullet = pool.Get();
            bullet.transform.SetPositionAndRotation(position, rotation);
            return bullet;
        }

        private Bullet CreateBullet()
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.SetPool(pool);
            bullet.gameObject.SetActive(false);
            return bullet;
        }

        private static void OnGetBullet(Bullet bullet)
        {
            if (bullet == null)
            {
                return;
            }

            bullet.gameObject.SetActive(true);
        }

        private static void OnReleaseBullet(Bullet bullet)
        {
            if (bullet == null)
            {
                return;
            }

            bullet.gameObject.SetActive(false);
        }

        private static void OnDestroyBullet(Bullet bullet)
        {
            if (bullet == null)
            {
                return;
            }

            Destroy(bullet.gameObject);
        }

        private void OnDestroy()
        {
            pool?.Clear();
            pool = null;
        }
    }
}
