using UnityEngine;

namespace UtilityKit
{
    public class IKFoot : MonoBehaviour
    {
        public Vector3 leftFootOffset;
        public Vector3 rightFootOffset;

        private Animator animator;

        private Vector3 leftFootPos;
        private Vector3 rightFootPos;

        private Quaternion leftFootRot;
        private Quaternion rightFootRot;

        private float leftFootWeight;
        private float rightFootWeight;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            rightFootWeight = animator.GetFloat("RightFoot");
            Vector3 rightFootPos = animator.GetIKPosition(AvatarIKGoal.RightFoot) + (Vector3.up * 0.3f);
            Debug.DrawRay(rightFootPos, -Vector3.up, Color.green);
            RaycastHit hit;
            if (Physics.Raycast(rightFootPos, -Vector3.up, out hit, 3))
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
                animator.SetIKPosition(AvatarIKGoal.RightFoot, hit.point + rightFootOffset);
                
                if (rightFootWeight > 0f)
                {
                    Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hit.normal), hit.normal);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, footRotation);
                }
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
            }

            leftFootWeight = animator.GetFloat("LeftFoot");
            Vector3 leftFootPos = animator.GetIKPosition(AvatarIKGoal.LeftFoot) + (Vector3.up * 0.3f);
            Debug.DrawRay(leftFootPos, -Vector3.up, Color.green);
            if (Physics.Raycast(leftFootPos, -Vector3.up, out hit, 3))
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point + leftFootOffset);

                if (leftFootWeight > 0f)
                {
                    Quaternion footRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hit.normal), hit.normal);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, footRotation);
                }
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
            }
        }
    }
}

