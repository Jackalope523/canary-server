using System;

namespace Core.Exceptions
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


    #region Core

    [Serializable]
	public class InvalidActionException : UserErrorException
	{
		public InvalidActionException()
            : base() { }
		public InvalidActionException(string message)
            : base(message) { }
        public InvalidActionException(string message, Exception innerException)
			: base(message, innerException) { }
    }

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
    public class InvalidGatheringException : UserErrorException
    {
        public InvalidGatheringException()
            : base() { }
        public InvalidGatheringException(string message)
            : base(message) { }
        public InvalidGatheringException(string message, Exception innerException)
            : base(message, innerException) { }
    }

	#endregion
}
