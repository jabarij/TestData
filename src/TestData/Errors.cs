namespace TestData
{
    public static class Errors
    {
        public const string ErrorCodeExceptionDataKey = "ErrorCode";

        public static readonly ErrorCode ObjectIsNull = new ErrorCode("E1001");
        public static readonly ErrorCode ObjectIsNullOrEmptyString = new ErrorCode("E1002");
        public static readonly ErrorCode ObjectIsNullOrWhiteSpace = new ErrorCode("E1003");
        public static readonly ErrorCode ObjectIsOfUnexpectedType = new ErrorCode("E1004");
        public static readonly ErrorCode ObjectIsOutOfRange = new ErrorCode("E1005");
        public static readonly ErrorCode ObjectIsNullOrEmptyCollection = new ErrorCode("E1006");

        public static readonly ErrorCode ConstructorNotFound = new ErrorCode("E2101");
        public static readonly ErrorCode ObjectIsDisposed = new ErrorCode("E2102");

        public static readonly ErrorCode OnlyMemberAccessExpressionAreAllowed = new ErrorCode("E2201");

        public static readonly ErrorCode EnumValueIsNotHandled = new ErrorCode("E2301");
    }
}
