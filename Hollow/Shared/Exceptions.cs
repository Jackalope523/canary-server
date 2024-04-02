using System;


namespace Shared
{
    #region Hollow

    [Serializable]
    public abstract class HollowException : Exception
    {
        public HollowException()
        { }

        public HollowException(string message)
            : base(message) { }

        public HollowException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public abstract class HollowFailureException : HollowException
    {
        public HollowFailureException()
        { }

        public HollowFailureException(string message)
            : base(message) { }

        public HollowFailureException(string message, Exception inner)
            : base(message, inner) { }
    }

    [Serializable]
    public abstract class UserErrorException : HollowException
	{
		public UserErrorException()
		{ }

		public UserErrorException(string message)
			: base(message) { }

		public UserErrorException(string message, Exception inner)
			: base(message, inner) { }
	}

	[Serializable]
	public class UnexpectedFailureException : HollowFailureException
	{
		public UnexpectedFailureException()
			: base() { }
		public UnexpectedFailureException(string message)
			: base(message) { }
		public UnexpectedFailureException(string message, Exception innerException)
			: base(message, innerException) { }
	}

    [Serializable]
    public class UndefinedBehaviourException : HollowException
    {
        public UndefinedBehaviourException()
        { }

        public UndefinedBehaviourException(string message)
            : base(message) { }

        public UndefinedBehaviourException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    #endregion


    #region Repository

    [Serializable]
    public class UnsupportedHarborFlagException : HollowFailureException
    {
        public UnsupportedHarborFlagException()
        {
        }

        public UnsupportedHarborFlagException(string message)
            : base(message)
        {
        }

        public UnsupportedHarborFlagException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    [Serializable]
    public class InsufficientRadiusException : HollowException
    {
        public InsufficientRadiusException()
        {
        }

        public InsufficientRadiusException(string message)
            : base(message)
        {
        }

        public InsufficientRadiusException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    [Serializable]
    public class ExcessiveRadiusException : HollowException
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
    [Serializable]
    public class UserNotFoundException : HollowFailureException
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
    [Serializable]
    public class InvalidInputException : HollowFailureException
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

    [Serializable]
    public class DatabaseWriteException : HollowFailureException
    {
        private static readonly string defaultMessage = "An unexpected error occured while writing to the database.";

        public DatabaseWriteException()
        {
        }
        public DatabaseWriteException(string message)
            : base(message)
        {
        }
        public DatabaseWriteException(Exception inner)
           : base(defaultMessage, inner)
        {
        }
        public DatabaseWriteException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    [Serializable]
    public class DatabaseReadException : HollowFailureException
    {
        private static readonly string defaultMessage = "An unexpected error occured while reading from the database.";
        public DatabaseReadException()
        {
        }

        public DatabaseReadException(string message)
            : base(message)
        {
        }
        public DatabaseReadException(Exception inner)
            : base(defaultMessage, inner)
        {
        }
        public DatabaseReadException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    [Serializable]
    public class BlobIOException : HollowFailureException
    {
        private static readonly string defaultMessage = "An unexpected error occured while communicating with the blob storage.";
        public BlobIOException()
        {
        }

        public BlobIOException(string message)
            : base(message)
        {
        }
        public BlobIOException(Exception inner)
            : base(defaultMessage, inner)
        {
        }
        public BlobIOException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    #endregion


    #region Core

    [Serializable]
	public class InvalidUserException : UserErrorException
	{
		public InvalidUserException()
            : base() { }
		public InvalidUserException(string message)
            : base(message) { }
        public InvalidUserException(string message, Exception innerException)
			: base(message, innerException) { }
    }

    [Serializable]
    public class InvalidInformationException : UserErrorException
    {
        public InvalidInformationException()
            : base() { }
        public InvalidInformationException(string message)
            : base(message) { }
        public InvalidInformationException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public class InsufficientPermissionsException : UserErrorException
    {
        public InsufficientPermissionsException()
            : base() { }
        public InsufficientPermissionsException(string message)
            : base(message) { }
        public InsufficientPermissionsException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public class InvalidEventException : UserErrorException
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
