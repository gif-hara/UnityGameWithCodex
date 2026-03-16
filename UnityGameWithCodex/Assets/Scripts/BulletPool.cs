using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace UnityGameWithCodex
{
    public class BulletPool : MonoBehaviour
    {
        private readonly Dictionary<Bullet, ObjectPool<Bullet>> pools = new();

        public Bullet Spawn(Bullet bulletPrefab, Vector3 position, Quaternion rotation)
        {
            Assert.IsNotNull(bulletPrefab, "BulletPool requires a bullet prefab.");

            if (bulletPrefab == null)
            {
                return null;
            }

            ObjectPool<Bullet> pool = GetOrCreatePool(bulletPrefab);
            Bullet bullet = pool.Get();
            bullet.transform.SetPositionAndRotation(position, rotation);
            bullet.OnSpawned();
            return bullet;
        }

        private ObjectPool<Bullet> GetOrCreatePool(Bullet bulletPrefab)
        {
            if (pools.TryGetValue(bulletPrefab, out ObjectPool<Bullet> pool))
            {
                return pool;
            }

            pool = new ObjectPool<Bullet>(
                () => CreateBullet(bulletPrefab),
                OnGetBullet,
                OnReleaseBullet,
                OnDestroyBullet);
            pools.Add(bulletPrefab, pool);
            return pool;
        }

        private Bullet CreateBullet(Bullet bulletPrefab)
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.SetPool(GetOrCreatePool(bulletPrefab));
            bullet.gameObject.SetActive(false);
            return bullet;
        }

        private void OnGetBullet(Bullet bullet)
        {
            if (bullet == null)
            {
                return;
            }

            bullet.gameObject.SetActive(true);
        }

        private void OnReleaseBullet(Bullet bullet)
        {
            if (bullet == null)
            {
                return;
            }

            bullet.gameObject.SetActive(false);
        }

        private void OnDestroyBullet(Bullet bullet)
        {
            if (bullet == null)
            {
                return;
            }

            Destroy(bullet.gameObject);
        }

        private void OnDestroy()
        {
            foreach (ObjectPool<Bullet> pool in pools.Values)
            {
                pool.Clear();
            }

            pools.Clear();
        }
    }
}
