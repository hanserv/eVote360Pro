namespace eVote360Pro.Core.Application
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, "");
        public static Result Failure(string error) => new Result(false, error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; set; }
        
        protected Result(T? value, bool isSuccess, string error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, "");
        public new static Result<T> Failure(string error) => new Result<T>(default, false, error);
    }
}
