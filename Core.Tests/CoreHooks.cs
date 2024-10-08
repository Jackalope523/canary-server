using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Core.Boundaries;
using Core.Entities;
using Xunit;
using Xunit.Abstractions;

using NetTopologySuite.Utilities;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

namespace Core.Tests
{
	public class UserHook : IAccountDatabase
	{
		private IAccountDatabase accounts;
		private ConcurrentBag<ulong> generatedUserIds;

		public UserHook(IAccountDatabase accountDatabase, ConcurrentBag<ulong> userIdList)
		{
			accounts = accountDatabase;
			generatedUserIds = userIdList;
		}

        public async Task<CoreUser> CreateUserAsync(string phoneNumber, string email, string normalisedEmail, string name, DateTimeOffset dateOfBirth, DateTimeOffset joinDate, CharacterShard character, Guid notificationId)
        {
			ContentValidation.TryNormalisePhoneNumber(phoneNumber, out phoneNumber);
            // Ensure no duplicate user exists
            CoreUser userCheck = null;
			try
			{
				userCheck = await accounts.FindUserByPhoneNumberAsync(phoneNumber);
				Console.Error.WriteLine($"Tried to create user with phone number {phoneNumber} but it already exists.");
			}
			catch { }

			if (userCheck != null)
			{
				// Check that user is properly inside id bag
				if (!generatedUserIds.Contains(userCheck.Id))
				{
					Console.Error.WriteLine("User did not exist in id bag. Adding...");
					generatedUserIds.Add(userCheck.Id);
				}

				throw new UnexpectedFailureException();
			}

			await accounts.CreateUserAsync(phoneNumber, email, normalisedEmail, name, dateOfBirth, joinDate, character, notificationId);

			CoreUser createdUser = await accounts.FindUserByPhoneNumberAsync(phoneNumber);
			generatedUserIds.Add(createdUser.Id);

			return createdUser;
        }

        public async Task DeleteUserAsync(ulong userId)
        {
			await accounts.DeleteUserAsync(userId);
        }

        public async Task<CoreUser> FindUserByEmailAsync(string normalisedEmail)
        {
			return await accounts.FindUserByEmailAsync(normalisedEmail);
        }

        public async Task<CoreUser> FindUserByIdAsync(ulong userId)
        {
			return await accounts.FindUserByIdAsync(userId);
        }

        public async Task<CoreUser> FindUserByPhoneNumberAsync(string phoneNumber)
        {
            ContentValidation.TryNormalisePhoneNumber(phoneNumber, out phoneNumber);
            return await accounts.FindUserByPhoneNumberAsync(phoneNumber);
        }

        public async Task<LocationShard> GetRecentLocationAsync(ulong userId)
        {
			return await accounts.GetRecentLocationAsync(userId);
        }

        public async Task<HauntShard> GetUserHauntAsync(ulong userId)
        {
			return await accounts.GetUserHauntAsync(userId);
        }

        public async Task UpdateHauntAsync(ulong userId, double latitude, double longitude, double radius, int stability)
        {
			await accounts.UpdateHauntAsync(userId, latitude, longitude, radius, stability);
        }

        public async Task UpdateRecentLocationAsync(ulong userId, double latitude, double longitude, double radius)
        {
			await accounts.UpdateRecentLocationAsync(userId, latitude, longitude, radius);
        }

        public async Task UpdateUserAsync(ulong userId, List<(string Property, object Value)> edits)
        {
			await accounts.UpdateUserAsync(userId, edits);
        }
    }

	public class NotificationServiceStub : INotificationService
	{
		public class NotificationStub
		{
			public string Title { get; init; }
			public string Message { get; init; }
		}

		public static ConcurrentDictionary<string, ConcurrentBag<NotificationStub>> messages = new();

		public Task PushNotification(string notificationId, NotificationGroup notificationGroup, string title, string message, string collapseId = "")
		{
			ConcurrentBag<NotificationStub> userBag;
			var exists = messages.TryGetValue(notificationId, out userBag);

			if (!exists)
			{ 
				userBag = new();
				messages.TryAdd(notificationId, userBag);
			}

			userBag.Add(new NotificationStub() { Title = title, Message = message });
			return Task.FromResult(0);
		}
	}
}
