using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Core.Boundaries;
using Core.Entities;
using Repository;
using Xunit;
using Xunit.Abstractions;
using Shared;
using NetTopologySuite.Utilities;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Linq;

namespace Core.Tests.Entities
{
	public class TestEnvironment : IDisposable
	{
		public CoreTerminal Terminal;

		private static int testUserIncrement = 0;
		private string testUserPhoneNumberFormat = "000-000-{0}";
		private string testUserEmailFormat = "email_{0}@test.com";
		private string testUserName = "name";
		private DateTimeOffset subjectDateOfBirth = new(new DateTime(0));
		private DateTimeOffset testJoinDate = new(new DateTime(0));

		private string testEventName = "event1";
		private string testEventDescription = "The first of many.";
		private DateTimeOffset testEventStartTime = new(new DateTime(0));
		private GeoLocation testEventLocation = new() { Latitude = 10, Longitude = 10 };
		private int testEventGroupMinimum = 0;
		private int testEventGroupMaximum = 10;
		private Distance testEventRadius = new() { Kilometres = 1 };
		private bool testEventIsDynamic = false;

		private DateTimeOffset testEtchingTime = new(new DateTime(0));
		private string testEtchingImageURL = "https://cdn.sparrow.com/101";

		public TestEnvironment()
		{
			// Arrange Core
			Terminal = CoreTerminal.CreateTerminal(QueryStore.AccountDatabaseAccess, QueryStore.EventDatabaseAccess,
				QueryStore.EtchingDatabaseAccess, QueryStore.ProfileDatabaseAccess,
				QueryStore.ReportDatabaseAccess, QueryStore.NotificationDatabaseAccess,
				new NotificationServiceStub());
		}

		internal User CreateUser(User user)
		{
			var userIncrement = testUserIncrement++;

			User userStub = new(user.ToUserShard())
				{ Id = (ulong) userIncrement };

			return userStub;
		}

		internal User CreateTestUser()
		{
			var userIncrement = testUserIncrement++;

			User userStub = new()
			{
				PhoneNumber = string.Format(testUserPhoneNumberFormat, userIncrement.ToString("D4")),
				Email = string.Format(testUserEmailFormat, userIncrement.ToString("D4")),
				Name = testUserName,
				DateOfBirth = subjectDateOfBirth,
				JoinDate = testJoinDate
			};

			return userStub;
		}

		internal async Task<User> GenerateTestUserAsync()
		{
			var userStub = CreateTestUser();

			return await GenerateUserUnsafeAsync(userStub);
		}

		internal async Task<User> GenerateUserUnsafeAsync(User userStub)
		{
			bool success = await Terminal.AccountDatabase.CreateUserAsync(userStub.PhoneNumber, userStub.Email, userStub.Email,
				userStub.Name, userStub.DateOfBirth, CharacterVector.Default.ToCharacter());

			if (!success)
			{ throw new UnexpectedFailureException("User creation failed."); }

			var user = await Terminal.AccountDatabase.FindUserByPhoneNumberAsync(userStub.PhoneNumber);

			return new(user);
		}

		internal async Task ForceFriendshipAsync(params User[] users)
		{
			foreach (var user in users)
			{
				foreach (var otherUser in users)
				{
					if (user.Equals(otherUser))
					{ continue; }

					await Terminal.ProfileDatabase.FollowUserAsync(user.Id, otherUser.Id);
				}
			}
		}

		internal async Task ForceEnemiesAsync(params User[] users)
		{
			foreach (var user in users)
			{
				foreach (var otherUser in users)
				{
					if (user.Equals(otherUser))
					{ continue; }

					await Terminal.ProfileDatabase.BlockUserAsync(user.Id, otherUser.Id);
				}
			}
		}

		internal Event CreateTestEvent(User host)
		{
			Event eventStub = new()
			{
				Name = testEventName,
				Description = testEventDescription,
				Host = host,
				StartTime = testEventStartTime,
				Location = testEventLocation,
				GroupMinimum = testEventGroupMinimum,
				GroupMaximum = testEventGroupMaximum,
				Radius = testEventRadius,
				IsDynamic = testEventIsDynamic
			};

			return eventStub;
		}

		internal async Task<Event> GenerateTestEventAsync(User host)
		{
			var eventStub = CreateTestEvent(host);

			return await GenerateEventUnsafeAsync(eventStub, host);
		}

		internal async Task<Event> GenerateEventUnsafeAsync(Event eventStub, User host)
		{
			return new(await Terminal.EventDatabase.CreateEventAsync(host.Id, eventStub.Name, eventStub.Description,
				eventStub.StartTime, eventStub.Location.Latitude, eventStub.Location.Longitude,
				eventStub.GroupMinimum, eventStub.GroupMaximum, host.Character.ToCharacter(),
				eventStub.Radius.Kilometres, eventStub.IsDynamic));
		}

		internal async Task<Event> GeneratePopulatedEventAsync(User host, params User[] guests)
		{
			var eventStub = await GenerateTestEventAsync(host);

			foreach (var guest in guests)
			{
				await Terminal.EventDatabase.SetUserStateAsync(guest.Id, eventStub.Id, EventUserState.Guest);
			}

			return eventStub;
		}

		internal async Task<Etching> GenerateTestEtchingAsync(Event etchedEvent, User etcher)
		{
			return await GenerateEtchingUnsafeAsync(etchedEvent, etcher, testEtchingTime, testEtchingImageURL);
		}

		internal async Task<Etching> GenerateEtchingUnsafeAsync(Event etchedEvent, User etcher, Etching etching)
		{
			return await Terminal.EtchingDatabase.AddEtchingAsync(etchedEvent.Id, etcher.Id, etching.TimeEtched, etching.ImageURL);
		}

		internal async Task<Etching> GenerateEtchingUnsafeAsync(Event etchedEvent, User etcher, DateTimeOffset timeEtched, string imageURL)
		{
			return await Terminal.EtchingDatabase.AddEtchingAsync(etchedEvent.Id, etcher.Id, timeEtched, imageURL);
		}

		public async void Dispose()
		{
			GC.SuppressFinalize(this);

			for (int id = 0; id <= testUserIncrement; id++)
			{
				_ = Terminal.AccountDatabase.DeleteUserAsync((ulong) id);
			}
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

		public Task PushNotification(DeviceType deviceType, string deviceToken, string title, string message)
		{
			ConcurrentBag<NotificationStub> userBag;
			var exists = messages.TryGetValue(deviceToken, out userBag);

			if (!exists)
			{ 
				userBag = new();
				messages.TryAdd(deviceToken, userBag);
			}

			userBag.Add(new NotificationStub() { Title = title, Message = message });
			return Task.FromResult(0);
		}
	}
}
