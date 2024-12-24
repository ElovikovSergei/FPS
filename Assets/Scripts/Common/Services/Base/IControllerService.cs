using UnityEngine;

namespace FPS.Common
{
    public interface IControllerService<TController> : IInitializable where TController : MonoBehaviour
    {
        public void SetController(TController controller);
    }
}