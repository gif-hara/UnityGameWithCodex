using UnityEngine;

namespace UnityGameWithCodex
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 40f;
        [SerializeField] private float lifetime = 3f;

        private Vector3 direction = Vector3.forward;
        private float elapsedTime;

        public void Initialize(Vector3 moveDirection, float moveSpeed, float lifeSeconds)
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
                Destroy(gameObject);
            }
        }
    }
}
