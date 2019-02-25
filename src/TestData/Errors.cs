namespace TestData
{
    public static class Errors
    {
        public const string ErrorCodeExceptionDataKey = "ErrorCode";

        public static readonly ErrorCode ConstructorNotFound = new ErrorCode("E2101");
        public static readonly ErrorCode OnlyMemberAccessExpressionAreAllowed = new ErrorCode("E2201");
    }
}
