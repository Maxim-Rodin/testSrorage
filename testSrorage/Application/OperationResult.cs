namespace testSrorage.Application
{
    // Результат операции с признаком успеха и сообщением для UI.
    public sealed class OperationResult
    {
        private OperationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; private set; }
        public string Message { get; private set; }

        // Создаёт успешный результат.
        public static OperationResult Ok(string message)
        {
            return new OperationResult(true, message);
        }

        // Создаёт неуспешный результат.
        public static OperationResult Fail(string message)
        {
            return new OperationResult(false, message);
        }
    }
}
