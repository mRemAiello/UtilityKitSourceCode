using UnityEngine;

namespace MCFramework
{
    public class IKHead : MonoBehaviour
    {
        public Transform lookAtThis;

        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (lookAtThis != null)
            {
                float distanceFaceObject = Vector3.Distance(animator.GetBoneTransform(HumanBodyBones.Head).position, lookAtThis.position);

                animator.SetLookAtPosition(lookAtThis.position);
                animator.SetLookAtWeight(Mathf.Clamp01(2 - distanceFaceObject), Mathf.Clamp01(1 - distanceFaceObject));
            }
        }
    }
}

