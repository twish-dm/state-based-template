
using System;

namespace StateEngine
{
    public interface IInitialize : IDisposable
    {
        bool IsInitialized { get; }
        void Initialize();
    }
}