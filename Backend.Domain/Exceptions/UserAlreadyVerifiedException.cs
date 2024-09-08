using System.Globalization;

namespace Backend.Domain.Exceptions
{
    public class UserAlreadyVerifiedException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyVerifiedException"/> class.
        /// </summary>
        public UserAlreadyVerifiedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyVerifiedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UserAlreadyVerifiedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAlreadyVerifiedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public UserAlreadyVerifiedException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}