using UnityEngine;

namespace FPS
{
    public static class AnimatorParameters
    {
        public static readonly int InAir = Animator.StringToHash("InAir");
        public static readonly int MoveX = Animator.StringToHash("MoveX");
        public static readonly int MoveY = Animator.StringToHash("MoveY");
        public static readonly int Velocity = Animator.StringToHash("Velocity");
        public static readonly int Moving = Animator.StringToHash("Moving");
        public static readonly int Crouching = Animator.StringToHash("Crouching");
        public static readonly int Sprinting = Animator.StringToHash("Sprinting");
        public static readonly int Proning = Animator.StringToHash("Proning");

        public static readonly int FullBodyWeight = Animator.StringToHash("FullBodyWeight");
        public static readonly int ProneWeight = Animator.StringToHash("ProneWeight");
        public static readonly int InspectStart = Animator.StringToHash("InspectStart");
        public static readonly int InspectEnd = Animator.StringToHash("InspectEnd");
        public static readonly int Slide = Animator.StringToHash("Sliding");
    }
}