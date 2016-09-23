using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OpenQA.Selenium
{
    /// <summary>
    /// The exception that is thrown when a reference to an element is no longer valid.
    /// </summary>
    [Serializable]
    public class InvalidElementStateException : WebDriverException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidElementStateException"/> class.
        /// </summary>
        public InvalidElementStateException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidElementStateException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidElementStateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidElementStateException"/> class with
        /// a specified error message and a reference to the inner exception that is the
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or <see langword="null"/> if no inner exception is specified.</param>
        public InvalidElementStateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidElementStateException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        protected InvalidElementStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}