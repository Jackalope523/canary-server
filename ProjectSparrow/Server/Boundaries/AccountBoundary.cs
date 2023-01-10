using System;
using System.Collections.Generic;

namespace Server.Boundaries
{
	public record ThinUser(Guid Id, string PhoneNumber, string Name, DateTime DateOfBirth, int Reputation, int NumberOfFollowers);
	public record ThinnerUser(Guid Id, string Name);
	public record ThinProfile(Guid Id, string Name, int Reputation, int NumberOfFollowers);

	public interface IAccountDatabase
	{
		ThinUser FindUser(Guid id);
        ThinUser FindUser(string phoneNumber);
        bool CreateUser(string phoneNumber, string passkey, string name, DateTime dateOfBirth);
        bool DeleteUser(Guid Id);
        bool UpdatePhoneNumber(Guid id, string newNumber);
        bool UpdatePasskey(Guid id, string newPasskey);
        bool UpdateName(Guid id, string newName);
        bool UpdateReputation(Guid id, int newReputation);
		
		List<ThinnerUser> GetFollowedUsers(Guid id);
		List<ThinnerUser> GetBlockedUsers(Guid id);

		bool FollowUser(Guid selfId, Guid targetId);
		bool UnfollowUser(Guid selfId, Guid targetId);
		bool BlockUser(Guid selfId, Guid targetId);
		bool UnblockUser(Guid selfId, Guid targetId);
	}

	public interface IAccountOperations
	{
		ThinProfile GetUserProfile(string identification, string targetIdentification);

		string TryLogin(string identification, string passkey);
		void CreateUser(string identification, string passkey, string name, DateTime dateOfBirth, string profilePhoto);
		void EditUser(string identification, string newName, DateTime newDateOfBirth, string newPhoto); // TODO Add EditAccount to update identification and/or passkey
		void DeleteUser(string identification);

		List<ThinnerUser> GetFollowedUsers(string identification);
		List<ThinnerUser> GetBlockedUsers(string identification);

		void FollowUser(string identification, string targetIdentification);
		void UnfollowUser(string identification, string targetIdentification);
		void BlockUser(string identification, string targetIdentification);
		void UnblockUser(string identification, string targetIdentification);
	}
}
