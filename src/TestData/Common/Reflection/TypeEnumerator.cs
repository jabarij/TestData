using System;
using System.Collections;
using System.Collections.Generic;

namespace TestData.Common.Reflection
{
    class TypeEnumerator : IEnumerator<Type>, IExtendedDisposable
    {
        public TypeEnumerator(Type initialType, Func<Type, Type> getNextType)
        {
            _initialType = Assert.IsNotNull(initialType, nameof(initialType));
            _getNextType = Assert.IsNotNull(getNextType, nameof(getNextType));
            Reset();
        }

        private Func<Type, Type> _getNextType;
        private Type GetNextType(Type currentType) { ThrowIfDisposed(); return _getNextType(currentType); }

        private Type _initialType;
        private Type InitialType { get { ThrowIfDisposed(); return _initialType; } }

        #region IEnumerator

        object IEnumerator.Current => Current;
        public Type Current { get; private set; }
        public bool MoveNext() => (Current = GetNextType(Current)) != null;
        public void Reset() => Current = InitialType;

        #endregion

        #region IDisposable

        private bool _isDisposed;
        bool IExtendedDisposable.IsDisposed => _isDisposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _getNextType = null;
                _initialType = null;
                _isDisposed = true;
            }
        }
        protected void ThrowIfDisposed() { Assert.IsNotDisposed(this, e => e._isDisposed); if (_isDisposed) Error.Raise(new ObjectDisposedException(GetType().FullName), Errors.ObjectIsDisposed); }

        #endregion
    }
}