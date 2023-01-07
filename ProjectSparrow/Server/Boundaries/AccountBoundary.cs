using Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public record ThinAccount(string AccountID, string Identification);
	public record ThinUser(string AccountID, string Identification, string Name, DateTime DateOfBirth, string ProfilePhoto) : ThinAccount(AccountID, Identification);

	public record ThinListUser(string UserID, string Name, string ProfilePhoto);

	public interface IAccountDatabase
	{
		ThinAccount GetAccount(string accountID);
		ThinAccount FindAccount(string identification);

		void UpdateAccount(string accountID, string newIdentification, string newHashedPasskey);
		void DeleteAccount(string accountID);

		ThinUser GetUser(string accountID);
		ThinUser CreateUser(string identification, string hashedPasskey, string name, DateTime dateOfBirth, string profilePhoto);
		void UpdateUser(string userID, string newName, DateTime newDateOfBirth, string newPhoto);
	}

	public interface IAccountOperations
	{
		ThinUser GetUserProfile(string identification, string targetIdentification);

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
