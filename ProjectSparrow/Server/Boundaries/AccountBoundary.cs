using System;
using System.Collections.Generic;

namespace Server.Boundaries
{
	public record ThinUser(Guid AccountId, string Identification, string Name, DateTime DateOfBirth, int Reputation, int NumberOfFollowers);
	public record ThinnerUser(Guid AccountId, string Name);
	public record ThinProfile(Guid AccountId, string Name, int Reputation, int NumberOfFollowers);

	public interface IAccountDatabase
	{
		ThinUser FindUser(Guid id);
        ThinUser FindUser(string phoneNumber);
        bool CreateUser(string phoneNumber, string passkey, string name, DateTime dateOfBirth);
        bool DeleteUser(Guid accountId);
        bool UpdatePhoneNumber(Guid id, string newNumber);
        bool UpdatePasskey(Guid id, string newPasskey);
        bool UpdateName(Guid id, string newName);
        bool UpdateReputation(Guid id, int newReputation);
		
		List<ThinnerUser> GetFollowedUsers(Guid id);
		List<ThinnerUser> GetBlockedUsers(Guid id);

		void FollowUser(Guid selfId, Guid targetId);
		void UnfollowUser(Guid selfId, Guid targetId);
		void BlockUser(Guid selfId, Guid targetId);
		void UnblockUser(Guid selfId, Guid targetId);
	}

	public interface IAccountOperations
	{
		ThinProfile GetUserProfile(Guid userID, Guid targetID);

		string TryLogin(string phoneNumber, string passkey);
		void CreateUser(string phoneNumber, string passkey, string name, DateTime dateOfBirth);
		void EditUser(Guid userID, string newName); // TODO Add EditAccount to update identification and/or passkey
		void DeleteUser(Guid userID);

		List<ThinnerUser> GetFollowedUsers(Guid userID);
		List<ThinnerUser> GetBlockedUsers(Guid userID);

		void FollowUser(Guid userID, Guid targetID);
		void UnfollowUser(Guid userID, Guid targetID);
		void BlockUser(Guid userID, Guid targetID);
		void UnblockUser(Guid userID, Guid targetID);
	}
}
