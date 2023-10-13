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

	public record UserShard(Guid Id, string PhoneNumber, string Email, string Name,
		DateTimeOffset DateOfBirth, bool IsPhoneConfirmed, bool IsEmailConfirmed,
		string SecurityStamp, DateTimeOffset? LockoutDate, int AccessTries, UserAccountStatus AccountStatus,
		DateTimeOffset JoinDate, int Reputation, int NumberOfFollowers, Character Character);

	public record Character(int Extraversion, int Athleticism, int Chaoticness,
		int Competitiveness, int Industriousness, int NightOwl, int Openness);

	public interface IAccountDatabase
	{
		UserShard FindUserById(Guid id);
        UserShard FindUserByPhoneNumber(string phoneNumber);
		UserShard FindUserByEmail(string normalisedEmail);
        bool CreateUser(string phoneNumber, string email, string normalisedEmail,
			string name, DateTimeOffset dateOfBirth, Character character);
		bool UpdateUser(Guid id, List<(string Property, object Value)> edits);
        bool DeleteUser(Guid id);

		(double Latitude, double Longitude, double Radius) GetRecentUserLocation(Guid id);
		bool UpdateRecentLocation(Guid id, double latitude, double longitude, double radius);

		(double Latitude, double Longitude, double Radius, int Stability) GetUserHaunt(Guid id);
		bool UpdateHaunt(Guid id, double latitude, double longitude, double radius, int stability);
	}

	public interface IAccountOperations
	{
		Task<UserShard> GetUserAsync(Guid userID);
		Task<UserShard> GetUserAsync(string phoneNumber);

		Task CreateUserAsync(string phoneNumber, string email, string name, DateTimeOffset dateOfBirth);
		Task EditUserAsync(Guid userID,
			string phoneNumber = null, string email = null, string name = null,
			bool? isPhoneNumberConfirmed = null, bool? isEmailConfirmed = null,
			string securityStamp = null, DateTimeOffset? lockoutDate = null, int? accessTries = null);
		Task DeleteUserAsync(Guid userID);

		Task UpdateUserLocationAsync(Guid userID, double latitude, double longitude);
	}
}
