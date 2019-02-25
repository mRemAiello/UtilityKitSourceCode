using UnityEngine;

namespace MCFramework
{
    public class ActionCameraController : MonoBehaviour
    {
        public Transform target;
        public float moveSpeed = 2f;

        public float angleMin = -50.0f;
        public float angleMax = 50.0f;

        private Vector2 m_CurrentPosition;

        void Update()
        {
            m_CurrentPosition.x += Input.GetAxis("Mouse X");
            m_CurrentPosition.y += Input.GetAxis("Mouse Y");
            m_CurrentPosition.y = Mathf.Clamp(m_CurrentPosition.y, angleMin, angleMax);

            Quaternion rotation = Quaternion.Euler(m_CurrentPosition.y, m_CurrentPosition.x, 0);
            transform.rotation = rotation;
        }

        void LateUpdate()
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }
}

