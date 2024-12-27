using System.Collections.Generic;
using UnityEngine;
using System;

namespace FPS.Settings
{
    [CreateAssetMenu(fileName = "InputPropertiesSettings", menuName = "Settings/Input properties settings")]
    public class InputPropertiesSettings : RegistrableSettings
    {
        [field: SerializeField] public List<IntProperty> IntProperties { get; private set; }
        [field: SerializeField] public List<FloatProperty> FloatProperties { get; private set; }
        [field: SerializeField] public List<BoolProperty> BoolProperties { get; private set; }
        [field: SerializeField] public List<VectorProperty> VectorProperties { get; private set; }
    }


    [Serializable]
    public class VectorProperty : Property<Vector4> { }


    [Serializable]
    public sealed class BoolProperty : Property<bool> { }


    [Serializable]
    public sealed class IntProperty : Property<int> { }


    [Serializable]
    public sealed class FloatProperty : Property<float>
    {
        [field: SerializeField] public float InterpolationSpeed { get; private set; }
    }


    [Serializable]
    public abstract class Property<TType>
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public TType DefaultValue { get; private set; }
    }
}