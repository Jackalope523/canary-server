using System;
using System.Collections.Generic;
using Server.Controls;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public record ThinUser(Guid Id, string PhoneNumber, string Email, string Name, DateTime DateOfBirth,
		bool IsPhoneConfirmed, bool IsEmailConfirmed,
		string SecurityStamp, DateTimeOffset? LockoutDate, int AccessTries,
		DateTimeOffset JoinDate, int Reputation, int NumberOfFollowers);
	public record ThinnerUser(Guid Id, string Name);
	public record ThinProfile(Guid Id, string Name, int Reputation, int NumberOfFollowers);

	public interface IAccountDatabase
	{
        public static IAccountDatabase AccountDatabaseAccess;
        ThinUser FindUser(Guid id);
        ThinUser FindUser(string phoneNumber);
        bool CreateUser(string phoneNumber, string email, string name, DateTime dateOfBirth);
        bool DeleteUser(Guid id);
        bool UpdatePhoneNumber(Guid id, string newNumber);
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
		static IAccountOperations AccountManager => new AccountManager(IAccountDatabase.AccountDatabaseAccess);

		Task<ThinUser> GetUserAsync(Guid userID);
		Task<ThinUser> GetUserAsync(string phoneNumber);
		Task<ThinProfile> GetUserProfileAsync(Guid userID, Guid targetID);

		Task CreateUserAsync(string phoneNumber, string email, string name, DateTime dateOfBirth);
		Task EditUserAsync(Guid userID,
			string phoneNumber = "", string email = "", string name = "",
			bool? isPhoneNumberConfirmed = null, bool? isEmailConfirmed = null,
			string securityStamp = "", DateTimeOffset? lockoutDate = null, int? accessTries = null);
		Task DeleteUserAsync(Guid userID);

		Task<List<ThinnerUser>> GetFollowedUsersAsync(Guid userID);
		Task<List<ThinnerUser>> GetBlockedUsersAsync(Guid userID);

		Task FollowUserAsync(Guid userID, Guid targetID);
		Task UnfollowUserAsync(Guid userID, Guid targetID);
		Task BlockUserAsync(Guid userID, Guid targetID);
		Task UnblockUserAsync(Guid userID, Guid targetID);
	}
}
