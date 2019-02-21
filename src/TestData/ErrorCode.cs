using System;

namespace TestData
{
    public struct ErrorCode
    {
        public ErrorCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentNullException(nameof(code));
            Code = code;
        }

        public string Code { get; }
    }
}