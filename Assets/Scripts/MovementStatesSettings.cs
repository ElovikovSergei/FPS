using UnityEngine;
using System;

namespace FPS
{
    [CreateAssetMenu(fileName = "MovementStatesSettings", menuName = "Settings/Movement states settings", order = 0)]
    public sealed class MovementStatesSettings : ScriptableObject
    {
        [field: SerializeField] public LayerMask GroundMask { get; private set; }

        [field: SerializeField] public MovementStateSettings Idle { get; private set; }
        [field: SerializeField] public MovementStateSettings Prone { get; private set; }
        [field: SerializeField] public MovementStateSettings Crouching { get; private set; }
        [field: SerializeField] public MovementStateSettings Walking { get; private set; }
        [field: SerializeField] public MovementStateSettings Sprinting { get; private set; }
        [field: SerializeField] public AnimationCurve AccelerationCurve { get; private set; } = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        [field: SerializeField] [field: Range(0f, 1f)] public float CrouchRatio { get; private set; } = 0.5f;
     
        [field: SerializeField] [field: Min(0f)] public float ProneTransitionDuration { get; private set; } = 0f;

        [field: SerializeField] public float JumpHeight { get; private set; } = 9f;
        [field: SerializeField] [field: Range(0f, 1f)] public float AirFriction { get; private set; } = 0f;
        [field: SerializeField] public float AirVelocity { get; private set; } = 0f;
        [field: SerializeField] public float MaxFallVelocity { get; private set; } = 0f;
        [field: SerializeField] public float Gravity { get; private set; } = 9.81f;

        [field: Header("Sliding")]
        [field: SerializeField] public AnimationCurve SlideCurve { get; private set; } = AnimationCurve.Constant(0f, 1f, 0f);
        [field: SerializeField] [field: Min(0f)] public float SlideSpeed { get; private set; } = 1f;
        [field: SerializeField] public float SlideDirectionSmoothing { get; private set; } = 0f;
    }


    [Serializable]
    public struct MovementStateSettings
    {
        // Player movement velocity.
        [field: SerializeField] public float Velocity { get; private set; }
        // Velocity vector interpolation speed.
        [field: SerializeField] public float VelocitySmoothing { get; set; }
    }
}