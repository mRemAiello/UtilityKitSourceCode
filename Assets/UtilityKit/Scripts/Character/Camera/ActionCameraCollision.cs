using UnityEngine;

namespace UtilityKit
{
    public class ActionCameraCollision : MonoBehaviour
    {
        public float minDistance = 1.0f;
        public float maxDistance = 4.0f;
        public float smooth = 10.0f;

        private Vector3 m_CameraDir;
        private float m_Distance;

        void Start()
        {
            m_CameraDir = transform.localPosition.normalized;
            m_Distance = transform.localPosition.magnitude;
        }

        void LateUpdate()
        {
            Vector3 desiredCameraPosition = transform.parent.TransformPoint(m_CameraDir * maxDistance);

            RaycastHit hit;
            if (Physics.Linecast(transform.parent.position, desiredCameraPosition, out hit))
            {
                m_Distance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            }
            else
            {
                m_Distance = maxDistance;
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, m_CameraDir * m_Distance, Time.deltaTime * smooth);
        }
    }
}

