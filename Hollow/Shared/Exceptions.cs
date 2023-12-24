using System;


namespace Shared
{
    #region Repository
    [Serializable]
    public class InsuifficientRadiusException : Exception
    {
        public InsuifficientRadiusException()
        {
        }

        public InsuifficientRadiusException(string message)
            : base(message)
        {
        }

        public InsuifficientRadiusException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class ExcessiveRadiusException : Exception
    {
        public ExcessiveRadiusException()
        {
        }

        public ExcessiveRadiusException(string message)
            : base(message)
        {
        }

        public ExcessiveRadiusException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string message)
            : base(message)
        {
        }

        public UserNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class InvalidInputException : Exception
    {
        public InvalidInputException()
        {
        }

        public InvalidInputException(string message)
            : base(message)
        {
        }

        public InvalidInputException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    #endregion

    #region Accounts

    [Serializable]
	public class InvalidUserException : Exception
	{
		public InvalidUserException()
            : base() { }
		public InvalidUserException(string message)
            : base(message) { }
        public InvalidUserException(string message, Exception innerException)
			: base(message, innerException) { }
    }

    [Serializable]
    public class InvalidInformationException : Exception
    {
        public InvalidInformationException()
            : base() { }
        public InvalidInformationException(string message)
            : base(message) { }
        public InvalidInformationException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public class InsufficientPermissionsException : Exception
    {
        public InsufficientPermissionsException()
            : base() { }
        public InsufficientPermissionsException(string message)
            : base(message) { }
        public InsufficientPermissionsException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public class UnexpectedFailureException : Exception
    {
        public UnexpectedFailureException()
            : base() { }
        public UnexpectedFailureException(string message)
            : base(message) { }
        public UnexpectedFailureException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    #endregion

    #region Events

    [Serializable]
    public class InvalidEventException : Exception
    {
        public InvalidEventException()
            : base() { }
        public InvalidEventException(string message)
            : base(message) { }
        public InvalidEventException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    #endregion
}
