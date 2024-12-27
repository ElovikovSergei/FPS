using System.Collections.Generic;
using FPS.Settings;
using UnityEngine;

namespace FPS.Common.Core
{
    public sealed class InputPropertiesService
    {
        private (int, float, float)[] _floatsToInterpolate;
        private Dictionary<string, int> _inputPropertyMap;
        private List<object> _inputProperties;

#if UNITY_EDITOR
        private List<(string, object)> _propertyNames;
#endif

        public InputPropertiesService(InputPropertiesSettings settings)
        {
            Initialize(settings);
        }

        public int GetPropertyIndex(string propertyName)
        {
            if (_inputPropertyMap.TryGetValue(propertyName, out int index))
            {
                return index;
            }

            return -1;
        }

        public T GetValue<T>(string propertyName)
        {
            return GetValue<T>(GetPropertyIndex(propertyName));
        }

        public T GetValue<T>(int propertyIndex)
        {
            if (propertyIndex < 0 || propertyIndex > _inputProperties.Count - 1)
            {
                return default;
            }

            return (T)_inputProperties[propertyIndex];
        }

        public void SetValue(string propertyName, object value)
        {
            SetValue(GetPropertyIndex(propertyName), value);
        }

        public void SetValue(int propertyIndex, object value)
        {
            if (propertyIndex < 0 || propertyIndex > _inputProperties.Count - 1)
            {
                return;
            }

            if (_floatsToInterpolate != null)
            {
                int floatToInterpolateIndex = -1;

                for (int i = 0; i < _floatsToInterpolate.Length; ++i)
                {
                    if (_floatsToInterpolate[i].Item1 == propertyIndex)
                    {
                        floatToInterpolateIndex = i;
                    }
                }

                if (floatToInterpolateIndex != -1)
                {
                    var tuple = _floatsToInterpolate[floatToInterpolateIndex];

                    tuple.Item3 = (float)value;
                    _floatsToInterpolate[floatToInterpolateIndex] = tuple;

                    return;
                }
            }

            _inputProperties[propertyIndex] = value;
        }

#if UNITY_EDITOR
        public (string, object)[] GetPropertyBindings()
        {
            if (_propertyNames == null)
            {
                return null;
            }

            int count = _propertyNames.Count;

            for (int i = 0; i < count; i++)
            {
                var item = _propertyNames[i];
                item.Item2 = _inputProperties[i];
                _propertyNames[i] = item;
            }

            return _propertyNames.ToArray();
        }
#endif

        private void Initialize(InputPropertiesSettings settings)
        {
#if UNITY_EDITOR
            _propertyNames = new List<(string, object)>();
#endif
            _inputProperties = new List<object>();
            _inputPropertyMap = new Dictionary<string, int>();

            List<(int, float, float)> floatsToInterpolate = new List<(int, float, float)>();

            int index = 0;

            foreach (var property in settings.BoolProperties)
            {
                _inputProperties.Add(property.DefaultValue);
                _inputPropertyMap.TryAdd(property.Name, index);

                ++index;
#if UNITY_EDITOR
                _propertyNames.Add((property.Name, null));
#endif
            }

            foreach (var property in settings.IntProperties)
            {
                _inputProperties.Add(property.DefaultValue);
                _inputPropertyMap.TryAdd(property.Name, index);

                ++index;
#if UNITY_EDITOR
                _propertyNames.Add((property.Name, null));
#endif
            }

            foreach (var property in settings.FloatProperties)
            {
                _inputProperties.Add(property.DefaultValue);
                _inputPropertyMap.TryAdd(property.Name, index);

                if (!Mathf.Approximately(property.InterpolationSpeed, 0f))
                {
                    floatsToInterpolate.Add((index, property.InterpolationSpeed, property.DefaultValue));
                }

                ++index;
#if UNITY_EDITOR
                _propertyNames.Add((property.Name, null));
#endif
            }

            if (floatsToInterpolate.Count > 0)
            {
                _floatsToInterpolate = floatsToInterpolate.ToArray();
            }

            foreach (var property in settings.VectorProperties)
            {
                _inputProperties.Add(property.DefaultValue);
                _inputPropertyMap.TryAdd(property.Name, index);

                ++index;
#if UNITY_EDITOR
                _propertyNames.Add((property.Name, null));
#endif
            }
        }
    }
}
