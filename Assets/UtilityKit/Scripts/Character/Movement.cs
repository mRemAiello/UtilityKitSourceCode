using UnityEngine;

namespace UtilityKit
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        public float floorOffsetY;
        public float moveSpeed = 6f;
        public float rotateSpeed = 10f;

        [Header("Raycast")]
        public float raycastWidthOffset = 0.25f;
        public float raycastHeightOffset = 0.5f;
        public float raycastLength = 1.6f;

        [Header("Animation")]
        public string walkAnimationFloatName;

        private Rigidbody m_Rigibody;
        private Animator m_Animator;
        private Vector3 m_MoveDirection;
        private float m_InputAmount;
        private Vector3 m_FloorMovement;
        private Vector3 m_Gravity;

        public float Horizontal
        {
            get;
            set;
        }

        public float Vertical
        {
            get;
            set;
        }

        void Start()
        {
            m_Rigibody = GetComponent<Rigidbody>();
            m_Animator = GetComponentInChildren<Animator>();

            Horizontal = 0;
            Vertical = 0;
        }

        void Update()
        {
            Vector3 combinedInput = (Horizontal * Camera.main.transform.right) + (Vertical * Camera.main.transform.forward);

            m_MoveDirection = Vector3.zero;
            m_MoveDirection = new Vector3(combinedInput.normalized.x, 0, combinedInput.normalized.z);

            float inputMagnitude = Mathf.Abs(Horizontal) + Mathf.Abs(Vertical);
            m_InputAmount = Mathf.Clamp01(inputMagnitude);

            if (m_MoveDirection != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(m_MoveDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * m_InputAmount * rotateSpeed);
                transform.rotation = targetRotation;
            }

            // handle animation blendtree for walking
            if (m_Animator != null && !string.IsNullOrEmpty(walkAnimationFloatName))
                m_Animator.SetFloat(walkAnimationFloatName, m_InputAmount, 0.2f, Time.deltaTime);
        }

        void FixedUpdate()
        {
            // if not grounded , increase down force
            if (FloorRaycasts(new Vector3(0, raycastHeightOffset, 0), raycastLength) == Vector3.zero)
            {
                m_Gravity += Vector3.up * Physics.gravity.y * Time.fixedDeltaTime;
            }

            // actual movement of the rigidbody + extra down force
            m_Rigibody.velocity = (m_MoveDirection * moveSpeed * m_InputAmount) + m_Gravity;

            // find the Y position via raycasts
            m_FloorMovement = new Vector3(m_Rigibody.position.x, FindFloor().y + floorOffsetY, m_Rigibody.position.z);

            // only stick to floor when grounded
            if (FloorRaycasts(new Vector3(0, raycastHeightOffset, 0), raycastLength) != Vector3.zero && m_FloorMovement != m_Rigibody.position)
            {
                // move the rigidbody to the floor
                m_Rigibody.MovePosition(m_FloorMovement);
                m_Gravity.y = 0;
            }
        }

        private Vector3 FindFloor()
        {
            float halfRaycastWidthOffset = raycastWidthOffset / 1.5f;

            Vector3[] floorAverage = new Vector3[9];
            floorAverage[0] = FloorRaycasts(new Vector3(0, raycastHeightOffset, 0), raycastLength);
            floorAverage[1] = FloorRaycasts(new Vector3(raycastWidthOffset, raycastHeightOffset, 0), raycastLength);
            floorAverage[2] = FloorRaycasts(new Vector3(-raycastWidthOffset, raycastHeightOffset, 0), raycastLength);
            floorAverage[3] = FloorRaycasts(new Vector3(0, raycastHeightOffset, raycastWidthOffset), raycastLength);
            floorAverage[4] = FloorRaycasts(new Vector3(0, raycastHeightOffset, -raycastWidthOffset), raycastLength);
            floorAverage[5] = FloorRaycasts(new Vector3(halfRaycastWidthOffset, raycastHeightOffset, halfRaycastWidthOffset), raycastLength);
            floorAverage[6] = FloorRaycasts(new Vector3(-halfRaycastWidthOffset, raycastHeightOffset, -halfRaycastWidthOffset), raycastLength);
            floorAverage[7] = FloorRaycasts(new Vector3(halfRaycastWidthOffset, raycastHeightOffset, -halfRaycastWidthOffset), raycastLength);
            floorAverage[8] = FloorRaycasts(new Vector3(-halfRaycastWidthOffset, raycastHeightOffset, halfRaycastWidthOffset), raycastLength);

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < floorAverage.Length; i++)
            {
                if (floorAverage[i] != Vector3.zero)
                {
                    sum += floorAverage[i];
                }
            }

            return sum / floorAverage.Length;
        }

        private Vector3 FloorRaycasts(Vector3 offset, float raycastLength)
        {
            Vector3 raycastFloorPos = transform.TransformPoint(offset);
            
            RaycastHit hit;
            if (Physics.Raycast(raycastFloorPos, -Vector3.up, out hit, raycastLength))
            {
                return hit.point;
            }               
            else
            {
                return Vector3.zero;
            }               
        }

        public void OnDrawGizmosSelected()
        {
            float halfRaycastWidthOffset = raycastWidthOffset / 1.5f;
            Vector3[] startPoints = new Vector3[9];
            startPoints[0] = transform.TransformPoint(0, raycastHeightOffset, 0);
            startPoints[1] = transform.TransformPoint(raycastWidthOffset, raycastHeightOffset, 0);
            startPoints[2] = transform.TransformPoint(-raycastWidthOffset, raycastHeightOffset, 0);
            startPoints[3] = transform.TransformPoint(0, raycastHeightOffset, raycastWidthOffset);
            startPoints[4] = transform.TransformPoint(0, raycastHeightOffset, -raycastWidthOffset);
            startPoints[5] = transform.TransformPoint(halfRaycastWidthOffset, raycastHeightOffset, halfRaycastWidthOffset);
            startPoints[6] = transform.TransformPoint(-halfRaycastWidthOffset, raycastHeightOffset, halfRaycastWidthOffset);
            startPoints[7] = transform.TransformPoint(-halfRaycastWidthOffset, raycastHeightOffset, -halfRaycastWidthOffset);
            startPoints[8] = transform.TransformPoint(halfRaycastWidthOffset, raycastHeightOffset, -halfRaycastWidthOffset);
            Vector3 endPoint = Vector3.down * raycastLength;

            for (int i = 0; i < startPoints.Length; i++)
            {
                Debug.DrawLine(startPoints[i], startPoints[i] + endPoint, Color.magenta);
            }
        }
    }
}

