using Demo.Scripts.Runtime.Item;
using MiniContainer;
using UnityEngine;

namespace FPS
{
    public sealed class WeaponRootContainer : RootContainer
    {
        [SerializeField] private FPSItem[] _weapons;

        protected override void Register(IBaseDIService builder)
        {
            builder.RegisterInstance(new WeaponFactoryService(DIContainer));

            for (int i = 0; i < _weapons.Length; ++i)
            {
                var weapon = _weapons[i];

                builder.RegisterComponentInNewPrefab(weapon);
                //builder.Register(weapon.ServiceType, ServiceLifeTime.Transient); // to do
            }
        }
    }
}