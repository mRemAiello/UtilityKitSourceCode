﻿using UnityEngine;

namespace UtilityKit
{
    public class KeepBool : StateMachineBehaviour
    {
        public string boolName;
        public bool status;
        public bool resetOnExit = true;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(boolName, status);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (resetOnExit)
                animator.SetBool(boolName, !status);
        }
    }
}