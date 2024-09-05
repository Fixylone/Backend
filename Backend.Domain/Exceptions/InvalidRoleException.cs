using System.Globalization;

namespace Backend.Domain.Exceptions
{ /// <summary>
  /// Exception that should be thrown when the email is already taken.
  /// </summary>
  /// <seealso cref="Exception" />
    public class InvalidRoleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRoleException"/> class.
        /// </summary>
        public InvalidRoleException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRoleException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidRoleException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRoleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public InvalidRoleException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}