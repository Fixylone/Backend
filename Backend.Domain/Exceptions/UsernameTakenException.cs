using System.Globalization;

namespace Backend.Domain.Exceptions
{
    public class UsernameTakenException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsernameTakenException"/> class.
        /// </summary>
        public UsernameTakenException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsernameTakenException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UsernameTakenException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsernameTakenException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public UsernameTakenException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}