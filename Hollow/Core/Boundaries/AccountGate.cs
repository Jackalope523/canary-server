using System;
using System.Collections.Generic;
using Core.Controls;
using System.Threading.Tasks;
using Core.Entities;
using Shared;

namespace Core.Boundaries
{
	public enum UserAccountStatus
	{ active, active_no_host, active_limited, inactive_under_review, blacklisted }

	public record UserShard(ulong Id, string PhoneNumber, string Email, string Name,
		DateTimeOffset DateOfBirth, bool IsPhoneConfirmed, bool IsEmailConfirmed,
		string SecurityStamp, DateTimeOffset? LockoutDate, int AccessTries, UserAccountStatus AccountStatus,
		DateTimeOffset JoinDate, int Reputation, int NumberOfFollowers, Character Character);

	public record Character(int Extraversion, int Athleticism, int Chaoticness,
		int Competitiveness, int Industriousness, int NightOwl, int Openness);

	public interface IAccountDatabase
	{
		UserShard FindUserById(ulong userId);
        UserShard FindUserByPhoneNumber(string phoneNumber);
		UserShard FindUserByEmail(string normalisedEmail);
        bool CreateUser(string phoneNumber, string email, string normalisedEmail,
			string name, DateTimeOffset dateOfBirth, Character character);
		bool UpdateUser(ulong userId, List<(string Property, object Value)> edits);
        bool DeleteUser(ulong userId);

		(double Latitude, double Longitude, double Radius) GetRecentUserLocation(ulong userId);
		bool UpdateRecentLocation(ulong userId, double latitude, double longitude, double radius);

		(double Latitude, double Longitude, double Radius, int Stability) GetUserHaunt(ulong userId);
		bool UpdateHaunt(ulong userId, double latitude, double longitude, double radius, int stability);
	}

	public interface IAccountOperations
	{
		Task<UserShard> GetUserAsync(ulong userId);
		Task<UserShard> GetUserAsync(string phoneNumber);

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
}
