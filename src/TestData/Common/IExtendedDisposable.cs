using System;

namespace TestData.Common
{
    interface IExtendedDisposable : IDisposable
    {
        bool IsDisposed { get; }
    }
}
