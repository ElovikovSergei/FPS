namespace FPS.Common
{
    public sealed class InputService
    {
        public readonly GameplayInputController GameplayInput;

        public InputService(GameplayInputController gameplayInput)
        {
            GameplayInput = gameplayInput;
        }
    }
}