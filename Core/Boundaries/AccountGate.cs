using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Boundaries
{
	#region Schemas

	public enum UserAccountStatus
	{ Active, Impotent, Limited, Suspended, Blacklisted }

	public record CoreUser(ulong Id, string PhoneNumber, string Email, string Name,
		DateTimeOffset DateOfBirth, bool IsPhoneConfirmed, bool IsEmailConfirmed, bool IsPendingDeletion,
		string SecurityStamp, DateTimeOffset? LockoutDate, int AccessTries, UserAccountStatus AccountStatus,
		DateTimeOffset JoinDate, int Reputation, int NumberOfAppreciateers, Character Character)
		: CoreOnlyData();

    public record UserShard(ulong Id, string PhoneNumber, string Email, string Name,
        DateTimeOffset DateOfBirth, int Reputation, int NumberOfAppreciateers);

    public record Character(int Extraversion, int Athleticism, int Chaoticness,
		int Competitiveness, int Industriousness, int NightOwl, int Openness);

    public record RecentLocation(double Latitude, double Longitude, double Radius);
    public record Haunt(double Latitude, double Longitude, double Radius, int Stability);
	
    #endregion

    #region Gates

    public interface IAccountDatabase
	{
		Task<CoreUser> FindUserByIdAsync(ulong userId);
        Task<CoreUser> FindUserByPhoneNumberAsync(string phoneNumber);
		Task<CoreUser> FindUserByEmailAsync(string normalisedEmail);
		Task CreateUserAsync(string phoneNumber, string email, string normalisedEmail,
			string name, DateTimeOffset dateOfBirth, DateTimeOffset joinDate, Character character);
		Task UpdateUserAsync(ulong userId, List<(string Property, object Value)> edits);
		Task DeleteUserAsync(ulong userId);

		Task<RecentLocation> GetRecentUserLocationAsync(ulong userId);
		Task UpdateRecentLocationAsync(ulong userId, double latitude, double longitude, double radius);

		Task<Haunt> GetUserHauntAsync(ulong userId);
		Task UpdateHauntAsync(ulong userId, double latitude, double longitude, double radius, int stability);
	}

	public interface IAccountOperations
	{
		Task<CoreUser> GetCoreUserAsync(ulong userId);
		Task<CoreUser> GetCoreUserAsync(string phoneNumber);
		Task<UserShard> GetUserShardAsync(ulong userId);

		Task CreateUserAsync(string phoneNumber, string email, string name, DateTimeOffset dateOfBirth);
		Task EditUserAsync(ulong userId,
			string phoneNumber = null, string email = null, string name = null,
			bool? isPhoneNumberConfirmed = null, bool? isEmailConfirmed = null,
			string securityStamp = null, DateTimeOffset? lockoutDate = null, int? accessTries = null);
		Task DeleteUserAsync(ulong userId);

		Task UpdateUserLocationAsync(ulong userId, double latitude, double longitude);
	}

	public interface IEmailService
	{
		Task SendEmailAsync(string email, string subject, string body);
	}

	public interface ISMSService
	{
		Task SendSMSAsync(string phoneNumber, string message);
	}

	#endregion
}
