using Demo.Scripts.Runtime.Item;
using System;

namespace FPS
{
    public abstract class WeaponService<TController> : IWeaponService where TController : FPSItem
    {
        public Type ViewType => throw new NotImplementedException();

        protected TController _controller;
    }
}