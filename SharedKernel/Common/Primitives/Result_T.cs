using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common.Primitives
{
    public class Result<T> : Result
    {
        public T Value { get; }

        protected Result(T value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, string.Empty);
        public new static Result<T> Failure(string error) => new Result<T>(default!, false, error);
    }
}
