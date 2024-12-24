using KINEMATION.FPSAnimationFramework.Runtime.Layers.IkMotionLayer;
using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.KAnimationCore.Runtime.Attributes;
using KINEMATION.KAnimationCore.Runtime.Rig;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    [CreateAssetMenu(fileName = "PlayerControllerSettings", menuName = "Settings/Player controller settings")]
    public sealed class PlayerControllerSettings : ScriptableObject, IRigUser
    {
        [SerializeField] private KRig _kRig;

        [field: Tab("Animation")]


        [field: Header("Unarmed State")]
        [field: SerializeField] public RuntimeAnimatorController UnarmedController { get; private set; }

        [field: Header("IK Motions")]
        [field: SerializeField] public IkMotionLayerSettings AimingMotion { get; private set; }
        [field: SerializeField] public IkMotionLayerSettings CrouchingMotion { get; private set; }
        [field: SerializeField] public IkMotionLayerSettings JumpingMotion { get; private set; }
        [field: SerializeField] public IkMotionLayerSettings StopMotion { get; private set; }
        [field: SerializeField] public IkMotionLayerSettings LeanMotion { get; private set; }


        [field: Tab("Controller")]


        [field: Header("General")]
        [field: SerializeField] [field: Min(0f)] public float TimeScale { get; private set; } = 1f;
        [field: SerializeField] [field: Min(0f)] public float EquipDelay { get; private set; }
        [field: SerializeField] [field: Range(0f, 90f)] public float LeanAngle { get; private set; } = 25f;

        [field: Header("Camera")]
        [field: SerializeField] [field:Min(0f)] public float Sensitivity { get; private set; } = 1f;


        [field: Tab("Weapon")]


        [field: SerializeField] public KRigElement WeaponBone { get; private set; } = new KRigElement(-1, FPSANames.IkWeaponBone);
        [field: SerializeField] public List<GameObject> WeaponPrefabs { get; private set; }

        public KRig GetRigAsset()
        {
            return _kRig;
        }
    }
}