using UnityEngine;
using UnityEngine.Pool;

namespace UnityGameWithCodex
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 40f;
        [SerializeField] private float lifetime = 3f;

        private Vector3 direction = Vector3.forward;
        private float elapsedTime;
        private IObjectPool<Bullet> pool;

        public void SetPool(IObjectPool<Bullet> ownerPool)
        {
            pool = ownerPool;
        }

        public void Launch(Vector3 moveDirection, float moveSpeed, float lifeSeconds)
        {
            direction = moveDirection.normalized;
            speed = moveSpeed;
            lifetime = lifeSeconds;
            elapsedTime = 0f;
        }

        private void Update()
        {
            transform.position += direction * (speed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            if (elapsedTime >= lifetime)
            {
                if (pool != null)
                {
                    pool.Release(this);
                    return;
                }

                Destroy(gameObject);
            }
        }
    }
}
