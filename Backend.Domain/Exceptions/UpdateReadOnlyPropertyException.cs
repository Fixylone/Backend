using System.Globalization;

namespace Backend.Domain.Exceptions
{
    public class UpdateReadOnlyPropertyException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateReadOnlyPropertyException"/> class.
        /// </summary>
        public UpdateReadOnlyPropertyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateReadOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UpdateReadOnlyPropertyException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateReadOnlyPropertyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public UpdateReadOnlyPropertyException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}