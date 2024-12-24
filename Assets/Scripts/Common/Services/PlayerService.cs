namespace FPS.Common
{
    public sealed class PlayerService : IControllerService<PlayerController>
    {
        private PlayerController _controller;

        private readonly InputService _input;

        public PlayerService(InputService input)
        {
            _input = input;
        }

        public void SetController(PlayerController controller)
        {
            _controller = controller;
        }
    }
}