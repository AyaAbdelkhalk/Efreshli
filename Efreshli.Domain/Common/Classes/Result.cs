using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Domain.Common.Classes
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T Data { get; private set; }
        public List<string>? Errors { get; private set; } = [];

        public static Result<T> Success(T value)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = value
            };
        }

        public static Result<T> Failure(List<string> errors)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Errors = errors
            };
        }
    }
    public class Result
    {
        public bool IsSuccess { get; }
        public List<string> Errors { get; }

        public Result(bool isSuccess, List<string>? errors)
        {
            if (isSuccess && errors is not null && errors.Count is not 0) throw new InvalidOperationException();
            if (!isSuccess && (errors is null || errors.Count == 0)) throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Errors = errors ?? [];
        }

        public static Result Success() => new(true, null);
        public static Result Failure(List<string> errors) => new(false, errors);
        public static Result Failure(params string[] errors) => new(false, [.. errors]);
    }
}
