using System;
using System.Collections.Generic;
using Server.Controls;
using System.Threading.Tasks;
using Server.Entities;
using Shared;

namespace Server.Boundaries
{
	public enum UserAccountStatus
	{ active, active_no_host, active_limited, inactive_under_review, blacklisted }

	public record UserShard(Guid Id, string PhoneNumber, string Email, string Name, DateTimeOffset DateOfBirth,
		bool IsPhoneConfirmed, bool IsEmailConfirmed,
		string SecurityStamp, DateTimeOffset? LockoutDate, int AccessTries, UserAccountStatus AccountStatus,
		DateTimeOffset JoinDate, int Reputation, int NumberOfFollowers, Character Character);
	public record UserProfile(Guid Id, string Name, int Reputation, int NumberOfFollowers);
	public record UserSilhouette(Guid Id, string Name);

	public record Character(int Extraversion, int Athleticism, int Chaoticness,
		int Competitiveness, int Industriousness, int NightOwl, int Openness);

	public record UserReport(Guid Id, Guid ReportingUserId, Guid ReportedUserId, DateTimeOffset ReportTime,
		UserReportType ReportType, string ReportDetails);

	public interface IAccountDatabase
	{
        public static IAccountDatabase AccountDatabaseAccess;

		UserShard FindUserById(Guid id);
        UserShard FindUserByPhoneNumber(string phoneNumber);
		UserShard FindUserByEmail(string normalisedEmail);
        bool CreateUser(string phoneNumber, string email, string name,
			DateTimeOffset dateOfBirth, Character character);
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
		bool UpdateAccountStatus(Guid id, UserAccountStatus accountStatus);
		bool UpdateReputation(Guid id, int newReputation);
		bool UpdateUserCharacter(Guid id, int extraversion, int athleticism, int chaoticness,
			int competitiveness, int industriousness, int nightOwl, int openness);

		(double Latitude, double Longitude, double Radius) GetRecentUserLocation(Guid id);
		bool UpdateRecentLocation(Guid id, double latitude, double longitude, double radius);

		(double Latitude, double Longitude, double Radius, int Stability) GetUserHaunt(Guid id);
		bool UpdateHaunt(Guid id, double latitude, double longitude, double radius, int stability);

		List<UserSilhouette> GetFriends(Guid id);
		List<UserSilhouette> GetFollowedUsers(Guid id);
		List<UserSilhouette> GetBlockedUsers(Guid id);

		bool FollowUser(Guid selfId, Guid targetId);
		bool UnfollowUser(Guid selfId, Guid targetId);
		bool BlockUser(Guid selfId, Guid targetId);
		bool UnblockUser(Guid selfId, Guid targetId);

		bool RateUser(Guid selfId, Guid targetId, UserRating rating);
		bool RemoveUserRating(Guid selfId, Guid targetId);
		(int Positive, int Negative) GetUserRatings(Guid id);

		(List<UserReport>, List<EventReport>) GetReportsAboutUser(Guid id);
		(List<UserReport>, List<EventReport>) GetReportsByUser(Guid id);
		bool ReportUser(Guid selfId, Guid eventId, Guid targetId, UserReportType reportType, string reportDetails);
	}

	public interface IAccountOperations
	{
		static IAccountOperations AccountManager
			=> new AccountManager(IAccountDatabase.AccountDatabaseAccess, IEventDatabase.EventDatabaseAccess);

		Task<UserShard> GetUserAsync(Guid userID);
		Task<UserShard> GetUserAsync(string phoneNumber);
		Task<UserProfile> GetUserProfileAsync(Guid userID, Guid targetID);

		Task<List<EventShard>> GetUserActivityAsync(Guid userID, Guid targetID);
		Task<Dictionary<UserSilhouette, List<EventShard>>> GetFriendActivityAsync(Guid userID);

		Task CreateUserAsync(string phoneNumber, string email, string name, DateTimeOffset dateOfBirth);
		Task EditUserAsync(Guid userID,
			string phoneNumber = null, string email = null, string name = null,
			bool? isPhoneNumberConfirmed = null, bool? isEmailConfirmed = null,
			string securityStamp = null, DateTimeOffset? lockoutDate = null, int? accessTries = null);
		Task DeleteUserAsync(Guid userID);

		Task UpdateUserLocationAsync(Guid userID, double latitude, double longitude);

		Task<List<UserSilhouette>> GetFollowedUsersAsync(Guid userID);
		Task<List<UserSilhouette>> GetBlockedUsersAsync(Guid userID);

		Task FollowUserAsync(Guid userID, Guid targetID);
		Task UnfollowUserAsync(Guid userID, Guid targetID);
		Task BlockUserAsync(Guid userID, Guid targetID);
		Task UnblockUserAsync(Guid userID, Guid targetID);

		Task RateUserAsync(Guid userID, Guid targetID, UserRating rating);

		Task ReportUserAsync(Guid userID, Guid targetID, UserReportType reportType, string reportDetails);
	}
}
