using System;

namespace Coalballcat.Services
{
    /// <summary>
    /// Mark a <see cref="MonoSingleton{T}"/> type with this to prevent
    /// <see cref="MonoSingleton{T}.Instance"/> from auto-creating a GameObject when no
    /// instance exists in the scene. Accessing <c>Instance</c> will return <c>null</c> instead.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NonAutoCreateSingletonAttribute : Attribute { }
}
