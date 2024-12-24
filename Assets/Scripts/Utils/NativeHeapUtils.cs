using System.Runtime.InteropServices;

namespace FPS.Utils
{
    public static class NativeHeapUtils
    {
        [DllImport("__Internal")]
        private static extern int GC_expand_hp(int bytes);

        /// <summary>
        /// Allocates the spcified amount of memory in the heap for managed objects.
        /// The garbage collecor will not free this memory.
        /// </summary>
        /// <param name="bytes">Amount of memory to reserve</param>
        public static void ReserveMemory(int bytes)
        {
#if !UNITY_EDITOR && ENABLE_IL2CPP
		    GC_expand_hp(bytes);
#endif
        }

        /// <inheritdoc cref="ReserveMemory(int)"/>
        public static void ReserveMegabytes(int megabytes)
        {
            ReserveMemory(megabytes * 1024 * 1024);
        }
    }
}