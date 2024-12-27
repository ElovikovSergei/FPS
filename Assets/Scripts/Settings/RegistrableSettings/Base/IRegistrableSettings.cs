using MiniContainer;
using UnityEngine;

namespace FPS.Settings
{
    public interface IRegistrableSettings : IRegistrable
    {

    }

    
    public abstract class RegistrableSettings : ScriptableObject, IRegistrableSettings
    {

    }
}