using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OpenQA.Selenium
{
    /// <summary>
    /// The exception that is thrown when an unhandled alert is present.
    /// </summary>
    [Serializable]
    public class UnhandledAlertException : WebDriverException
    {
        private IAlert alert;
        private string alertText;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledAlertException"/> class.
        /// </summary>
        public UnhandledAlertException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledAlertException"/> class with
        /// a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnhandledAlertException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledAlertException"/> class with
        /// a specified error message and alert text.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="alertText">The text of the unhandled alert.</param>
        public UnhandledAlertException(string message, string alertText)
            : base(message)
        {
            this.alertText = alertText;
            this.alert = new UnhandledAlert(alertText);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledAlertException"/> class with
        /// a specified error message and a reference to the inner exception that is the
        /// cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or <see langword="null"/> if no inner exception is specified.</param>
        public UnhandledAlertException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledAlertException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        protected UnhandledAlertException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the <see cref="IAlert"/> that has not been handled.
        /// </summary>
        [Obsolete("Use the AlertText property to get the alert of an already dismised alert, or use SwitchTo().Alert() to handle an unhandled alert.")]
        public IAlert Alert
        {
            get { return this.alert; }
        }

        /// <summary>
        /// Gets the text of the unhandled alert.
        /// </summary>
        public string AlertText
        {
            get { return this.alertText; }
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        private class UnhandledAlert : IAlert
        {
            private string alertText;

            public UnhandledAlert(string alertText)
            {
                this.alertText = alertText;
            }

            public string Text
            {
                get { return this.alertText; }
            }

            public void Dismiss()
            {
                ThrowAlreadyDismissed();
            }

            public void Accept()
            {
                ThrowAlreadyDismissed();
            }

            public void SendKeys(string keysToSend)
            {
                ThrowAlreadyDismissed();
            }

            public void SetAuthenticationCredentials(string userName, string password)
            {
                ThrowAlreadyDismissed();
            }

            private static void ThrowAlreadyDismissed()
            {
                throw new InvalidOperationException("Alert was already dismissed");
            }
        }
    }
}