﻿using System.Globalization;

namespace Backend.Domain.Exceptions
{
    public class InvalidRoleException : AppException
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