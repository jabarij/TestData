namespace TestData
{
    public static class Errors
    {
        public const string ErrorCodeExceptionDataKey = "ErrorCode";

        public static readonly ErrorCode ArgumentIsNull = new ErrorCode("E1001");
        public static readonly ErrorCode ArgumentIsNullOrEmpty = new ErrorCode("E1002");
        public static readonly ErrorCode ArgumentIsNullOrWhiteSpace = new ErrorCode("E1003");
        public static readonly ErrorCode ArgumentIsOfUnexpectedType = new ErrorCode("E1004");

        public static readonly ErrorCode ConstructorNotFound = new ErrorCode("E2101");
        public static readonly ErrorCode OnlyMemberAccessExpressionAreAllowed = new ErrorCode("E2201");
    }
}
