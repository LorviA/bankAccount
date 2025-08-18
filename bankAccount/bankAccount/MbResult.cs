namespace bankAccount
{
    /// <summary>
    /// Результат операции, содержащий либо значение, либо ошибку
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого значения</typeparam>
    public class MbResult<T>
    {
        /// <summary>
        /// Успешность выполнения операции
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Результат операции (только при успехе)
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Информация об ошибке (только при неудаче)
        /// </summary>
        public MbError? Error { get; }

        private MbResult(T? value, bool isSuccess, MbError? error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// Создание успешного результата
        /// </summary>
        public static MbResult<T> Success(T value) => new(value, true, null);

        /// <summary>
        /// Создание результата с ошибкой
        /// </summary>
        public static MbResult<T> Failure(MbError error) => new(default, false, error);

        /// <summary>
        /// Создание результата с ошибкой валидации
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public static MbResult<T> ValidationFailure(IDictionary<string, string[]> errors)
            => new(default, false, MbError.ValidationError(errors));
    }

    /// <summary>
    /// Информация об ошибке
    /// </summary>
    public class MbError
    {
        /// <summary>
        /// HTTP статус код ошибки
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Детали ошибок валидации (ключ - имя поля)
        /// </summary>
        public IDictionary<string, string[]>? ValidationErrors { get; }

        private MbError(int statusCode, string message, IDictionary<string, string[]>? validationErrors = null)
        {
            StatusCode = statusCode;
            Message = message;
            ValidationErrors = validationErrors;
        }

        /// <summary>
        /// Ошибка валидации
        /// </summary>
        public static MbError ValidationError(IDictionary<string, string[]> errors)
            => new(400, "Validation error", errors);

        /// <summary>
        /// Ошибка валидации (простое сообщение)
        /// </summary>
        public static MbError Validation(string message)
            => new(400, message);

        /// <summary>
        /// Конфликт (HTTP 409)
        /// </summary>
        public static MbError Conflict(string message)
            => new(409, message);

        /// <summary>
        /// Ресурс не найден
        /// </summary>
        public static MbError NotFound(string message)
            => new(404, message);

        /// <summary>
        /// Ошибка аутентификации
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public static MbError Unauthorized(string message = "Unauthorized")
            => new(401, message);

        /// <summary>
        /// Внутренняя ошибка сервера
        /// </summary>
        public static MbError Internal(string message = "Internal server error")
            => new(500, message);
    }
}