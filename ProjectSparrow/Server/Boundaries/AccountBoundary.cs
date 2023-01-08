using Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public record ThinUser(int AccountId, string Identification, string Name, DateTime DateOfBirth, string ProfilePhoto, int Reputation, int NumberOfFollowers);
	public record ThinnerUser(string AccountId, string Name, string ProfilePhoto);
	public record ThinProfile(string AccountId, string Name, string ProfilePhoto, int Reputation, int NumberOfFollowers);

	public interface IAccountDatabase
	{
		ThinUser FindUser(Guid id);
        ThinUser FindUser(string phoneNumber);
        bool CreateUser(string phoneNumber, string passkey, string name, DateTime dateOfBirth);
        bool DeleteUser(Guid accountId);
        bool UpdatePhoneNumber(Guid id, string newNumber);
        bool UpdatePasskey(Guid id, string Passkey);
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
