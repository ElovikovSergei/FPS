using MiniContainer;
using FPS.Settings;
using UnityEngine;

namespace FPS.DI
{
    public sealed class SettingsRootContainer : RootContainer
    {
        [SerializeField] private RegistrableSettings[] _settings;

        protected override void Register(IBaseDIService builder)
        {
            for (int i = 0; i < _settings.Length; ++i)
            {
                builder.RegisterInstanceAsSelf(_settings[i]);
            }
        }
    }
}