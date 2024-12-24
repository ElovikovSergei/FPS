using UnityEngine;

namespace FPS.Common
{
    public interface IControllerService<TController> where TController : MonoBehaviour
    {
        public void SetController(TController controller);
    }
}