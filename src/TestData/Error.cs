﻿using System;
using System.Linq;

namespace TestData
{
    internal static class Error
    {
        internal static void Raise<TException>(Func<TException> getException, params ErrorCode[] errorCodes) where TException : Exception => Raise(getException(), errorCodes);
        internal static void Raise<TException>(TException exception, params ErrorCode[] errorCodes) where TException : Exception
        {
            exception = Format(exception, errorCodes);
            throw exception;
        }

        internal static TException Format<TException>(Func<TException> getException, params ErrorCode[] errorCodes) where TException : Exception => Format(getException(), errorCodes);
        internal static TException Format<TException>(TException exception, params ErrorCode[] errorCodes) where TException : Exception
        {
            Assert.IsNotNull(exception, nameof(exception));

            var errorsList = string.Join(", ", errorCodes.Select(e => e.Code));
            if (exception.Data.Contains(Errors.ErrorCodeExceptionDataKey))
                exception.Data[Errors.ErrorCodeExceptionDataKey] = errorsList;
            else
                exception.Data.Add(Errors.ErrorCodeExceptionDataKey, errorsList);

            return exception;
        }
    }
}