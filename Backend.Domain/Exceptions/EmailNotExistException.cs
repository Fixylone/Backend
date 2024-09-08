using System.Globalization;

namespace Backend.Domain.Exceptions
{
    public class EmailNotExistException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailNotExistException"/> class.
        /// </summary>
        public EmailNotExistException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailNotExistException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EmailNotExistException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailNotExistException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public EmailNotExistException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}