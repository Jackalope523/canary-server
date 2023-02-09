using System;
using System.Collections.Generic;
using Server.Controls;
using System.Threading.Tasks;
using Server.Entities;
using Shared;

namespace Server.Boundaries
{
	public enum UserAccountStatus
	{ active, active_no_host, active_under_review, inactive_under_review, blacklisted }

	public record ThinUser(Guid Id, string PhoneNumber, string Email, string Name, DateTimeOffset DateOfBirth,
		bool IsPhoneConfirmed, bool IsEmailConfirmed,
		string SecurityStamp, DateTimeOffset? LockoutDate, int AccessTries, UserAccountStatus AccountStatus,
		DateTimeOffset JoinDate, int Reputation, int NumberOfFollowers);
	public record ThinnerUser(Guid Id, string Name);
	public record ThinProfile(Guid Id, string Name, int Reputation, int NumberOfFollowers);

	public record UserReport(Guid Id, Guid ReportingUserId, Guid ReportedUserId, DateTimeOffset ReportTime,
		UserReportType ReportType, string ReportDetails);

	public interface IAccountDatabase
	{
        public static IAccountDatabase AccountDatabaseAccess;
		ThinUser FindUser(Guid id);
        ThinUser FindUser(string phoneNumber);
		ThinUser FindUserByEmail(string normalisedEmail);
        bool CreateUser(string phoneNumber, string email, string name, DateTimeOffset dateOfBirth);
        bool DeleteUser(Guid id);
        bool UpdatePhoneNumber(Guid id, string newNumber);
		bool UpdateEmail(Guid id, string newEmail);
		bool UpdateNormalisedEmail(Guid id, string normalisedEmail);
        bool UpdateName(Guid id, string newName);
		bool UpdatePhoneConfirmation(Guid id, bool isConfirmed);
		bool UpdateEmailConfirmation(Guid id, bool isConfirmed);
		bool UpdateSecurityStamp(Guid id, string newSecurityStamp);
		bool UpdateLockoutDate(Guid id, DateTimeOffset? newLockoutDate);
		bool UpdateAccessTries(Guid id, int newAccessTries);
        bool UpdateReputation(Guid id, int newReputation);

		List<ThinnerUser> GetFriends(Guid id);
		List<ThinnerUser> GetFollowedUsers(Guid id);
		List<ThinnerUser> GetBlockedUsers(Guid id);

		bool FollowUser(Guid selfId, Guid targetId);
		bool UnfollowUser(Guid selfId, Guid targetId);
		bool BlockUser(Guid selfId, Guid targetId);
		bool UnblockUser(Guid selfId, Guid targetId);

		(List<UserReport>, List<EventReport>) GetReports(Guid id);
		(List<UserReport>, List<EventReport>) GetReportsByUser(Guid id);
		bool ReportUser(Guid selfId, Guid targetId, UserReportType reportType, string reportDetails);
	}

	public interface IAccountOperations
	{
		static IAccountOperations AccountManager => new AccountManager(IAccountDatabase.AccountDatabaseAccess, IEventDatabase.EventDatabaseAccess);

		Task<ThinUser> GetUserAsync(Guid userID);
		Task<ThinUser> GetUserAsync(string phoneNumber);
		Task<ThinProfile> GetUserProfileAsync(Guid userID, Guid targetID);

		Task<List<ThinEvent>> GetUserActivityAsync(Guid userID, Guid targetID);
		Task<Dictionary<ThinnerUser, List<ThinEvent>>> GetFriendActivityAsync(Guid userID);

		Task CreateUserAsync(string phoneNumber, string email, string name, DateTimeOffset dateOfBirth);
		Task EditUserAsync(Guid userID,
			string phoneNumber = null, string email = null, string name = null,
			bool? isPhoneNumberConfirmed = null, bool? isEmailConfirmed = null,
			string securityStamp = null, DateTimeOffset? lockoutDate = null, int? accessTries = null);
		Task DeleteUserAsync(Guid userID);

		Task<List<ThinnerUser>> GetFollowedUsersAsync(Guid userID);
		Task<List<ThinnerUser>> GetBlockedUsersAsync(Guid userID);

		Task FollowUserAsync(Guid userID, Guid targetID);
		Task UnfollowUserAsync(Guid userID, Guid targetID);
		Task BlockUserAsync(Guid userID, Guid targetID);
		Task UnblockUserAsync(Guid userID, Guid targetID);

		Task ReportUserAsync(Guid userID, Guid targetID, UserReportType reportType, string reportDetails);
	}
}
