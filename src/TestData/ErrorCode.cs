using System;

namespace TestData
{
    public struct ErrorCode
    {
        public ErrorCode(string code)
        {
            Assert.IsNotNullOrWhiteSpace(code, nameof(code));
            Code = code;
        }

        public string Code { get; }
    }
}