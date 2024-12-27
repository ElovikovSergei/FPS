using Demo.Scripts.Runtime.Item;
using MiniContainer;

namespace FPS
{
    public sealed class WeaponFactoryService
    {
        private readonly IContainer _container;

        public WeaponFactoryService(IContainer container)
        {
            _container = container;
        }

        public TService GetPopupService<TService>(params object[] args) where TService : IWeaponService
        {
            var service = _container.Resolve<TService>();

            if (service != null)
            {
                var viewType = service.ViewType;
                var view = (FPSItem)_container.Resolve(viewType);

                //service.Initialize(view, args); // to do
            }

            return service;
        }
    }
}