using UnityEngine;

namespace FPS.Settings
{
    [CreateAssetMenu(fileName = "RecoilPatternSettings", menuName = "Settings/Recoil pattern settings")]
    public sealed class RecoilPatternSettings : ScriptableObject
    {
        [field: SerializeField] public Vector2 HorizontalRecoil { get; private set; }
        [field: SerializeField] public Vector2 VerticalRecoil { get; private set; }

        [field: SerializeField, Min(0f)] public float HorizontalSmoothing { get; private set; }
        [field: SerializeField, Min(0f)] public float VerticalSmoothing { get; private set; }

        [field: SerializeField, Min(0f)] public float Damping { get; private set; } = 0f;

        [field: Space]
        [field: SerializeField] public string DeltaLookInputProperty { get; private set; }
    }
}