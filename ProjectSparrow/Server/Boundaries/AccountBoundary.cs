using Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public record ThinAccount(string AccountID, string Identification);
	public record ThinUser(string AccountID, string Identification, string Name, DateTime DateOfBirth, string ProfilePhoto, int Reputation, int NumberOfFollowers)
		: ThinAccount(AccountID, Identification);

	public record ThinListUser(string AccountID, string Name, string ProfilePhoto);

	public record ThinProfile(string AccountID, string Name, string ProfilePhoto, int Reputation, int NumberOfFollowers);

	public interface IAccountDatabase
	{
		ThinAccount GetAccount(string accountID);
		ThinAccount FindAccount(string identification);

		void UpdateAccount(string accountID, string newIdentification, string newPasskey);
		void DeleteAccount(string accountID);

		ThinUser GetUser(string accountID);
		ThinUser CreateUser(string identification, string passkey, string name, DateTime dateOfBirth, string profilePhoto);
		void UpdateUser(string accountID, string newName, DateTime newDateOfBirth, string newPhoto);

		List<ThinListUser> GetFollowedUsers(string accountID);
		List<ThinListUser> GetBlockedUsers(string accountID);

		void FollowUser(string accountID, string targetAccountID);
		void UnfollowUser(string accountID, string targetAccountID);
		void BlockUser(string accountID, string targetAccountID);
		void UnblockUser(string accountID, string targetAccountID);
	}

	public interface IAccountOperations
	{
		ThinProfile GetUserProfile(string identification, string targetIdentification);

		string TryLogin(string identification, string passkey);
		void CreateUser(string identification, string passkey, string name, DateTime dateOfBirth, string profilePhoto);
		void EditUser(string identification, string newName, DateTime newDateOfBirth, string newPhoto); // TODO Add EditAccount to update identification and/or passkey
		void DeleteUser(string identification);

		List<ThinListUser> GetFollowedUsers(string identification);
		List<ThinListUser> GetBlockedUsers(string identification);

		void FollowUser(string identification, string targetIdentification);
		void UnfollowUser(string identification, string targetIdentification);
		void BlockUser(string identification, string targetIdentification);
		void UnblockUser(string identification, string targetIdentification);
	}
}
