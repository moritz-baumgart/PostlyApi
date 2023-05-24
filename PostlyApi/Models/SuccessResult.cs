namespace PostlyApi.Models
{
    public class SuccessResult<T, E>
    {
        /// <summary>
        /// Indicates whether the action was successful or not.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// If successful there might be a result set.
        /// </summary>
        public T? Result { get; set; }

        /// <summary>
        /// If not successful there might be an error set.
        /// </summary>
        public E? Error { get; set; }

        public SuccessResult(bool success, T result)
        {
            Success = success;
            Result = result;
        }

        public SuccessResult(bool success, E error)
        {
            Success = success;
            Error = error;
        }
    }
}
