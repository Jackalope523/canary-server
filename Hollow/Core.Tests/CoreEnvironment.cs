using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Core.Boundaries;
using Core.Entities;
using Xunit;
using Xunit.Abstractions;
using Shared;
using NetTopologySuite.Utilities;
using System.ComponentModel;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using PhoneNumbers;

namespace Core.Tests
{
	public class CoreEnvironment
	{
		public CoreTerminal Terminal;

		private int instance;

		private static ConcurrentBag<ulong> generatedUserIds = new();
		private ulong uniqueUserIncrement = 0;
		private string testUserPhoneNumberFormat = "000-{0}-{1}";
		private string testUserEmailFormat = "email_{0}_{1}@test.com";
		private string testUserName = "name";
		private DateTimeOffset subjectDateOfBirth = new(new DateTime(0));
		private DateTimeOffset testJoinDate = new(new DateTime(0));

		private static double uniqueEventDegree = -89;
		private string testEventName = "event1";
		private string testEventDescription = "The first of many.";
		private DateTimeOffset testEventStartTime = new(DateTime.UtcNow + TimeSpan.FromDays(1));
		private GeoLocation testEventLocation = new() { Latitude = 0, Longitude = 0 };
		private int testEventGroupMinimum = 0;
		private int testEventGroupMaximum = 10;
		private Distance testEventRadius = new() { Kilometres = 1 };
		private bool testEventIsDynamic = false;

		private DateTimeOffset testEtchingTime = new(DateTime.UtcNow);
		private string testEtchingImageURL = "https://cdn.sparrow.com/101";

		/////
		// Set-up
		///////////

		public CoreEnvironment(int instanceNumber)
		{
			instance = instanceNumber;

            // Arrange Core
			Repository.Harbor harbor = new(Repository.Harbor.Flag.Development);

            Terminal = CoreTerminal.CreateTerminal(
                new UserHook(harbor.AccountDatabaseAccess, generatedUserIds),
                harbor.EventDatabaseAccess,
                harbor.EtchingDatabaseAccess,
                harbor.ProfileDatabaseAccess,
                harbor.ReportDatabaseAccess,
                harbor.NotificationDatabaseAccess,
				harbor.AdminDatabaseAccess,
				new NotificationServiceStub());
		}

		///////
		// User Helpers
		/////////////////

		internal User CreateTestUser()
		{
			var userIncrement = Interlocked.Increment(ref uniqueUserIncrement);

			User userStub = new()
			{
				PhoneNumber = string.Format(testUserPhoneNumberFormat, instance.ToString("D3"), userIncrement.ToString("D4")),
				Email = string.Format(testUserEmailFormat, instance.ToString("D3"), userIncrement.ToString("D4")),
				Name = testUserName,
				DateOfBirth = subjectDateOfBirth,
				JoinDate = testJoinDate
			};

			return userStub;
		}

		internal async Task<User> GenerateUserUnsafeAsync(User userStub)
        {
            await Terminal.AccountDatabase.CreateUserAsync(userStub.PhoneNumber, userStub.Email, userStub.Email,
				userStub.Name, userStub.DateOfBirth, Psijic.Time, CharacterVector.Default.ToCharacter());

			var user = await Terminal.AccountDatabase.FindUserByPhoneNumberAsync(userStub.PhoneNumber);

			return new(user);
		}

		internal async Task<User> GenerateUniqueUserAsync()
		{
			var userStub = CreateTestUser();

			return await GenerateUserUnsafeAsync(userStub);
		}

		internal async Task UpdateUser(User user, string property, object value)
		{
			await Terminal.AccountDatabase.UpdateUserAsync(user.Id, new() { (property, value) });
		}

		internal async Task UpdateUserLocationAsync(User user, double latitude, double longitude, double radius = 1)
		{
			await Terminal.AccountDatabase.UpdateRecentLocationAsync(user.Id, latitude, longitude, radius);
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

		////////
		// Event Helpers
		//////////////////

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

		internal async Task<Event> GenerateEventUnsafeAsync(Event eventStub, User host)
		{
			return new(await Terminal.EventDatabase.CreateEventAsync(host.Id, eventStub.Name, eventStub.Description,
				eventStub.StartTime, eventStub.Location.Latitude, eventStub.Location.Longitude,
				eventStub.GroupMinimum, eventStub.GroupMaximum, host.Character.ToCharacter(),
				eventStub.Radius.Kilometres, eventStub.IsDynamic));
		}

		internal async Task<Event> GenerateUpcomingEventAsync(User host)
		{
			var eventStub = CreateTestEvent(host);

			return await GenerateEventUnsafeAsync(eventStub, host);
		}

		internal async Task<Event> GenerateUpcomingEventAsync(User host, params User[] guests)
		{
			var eventStub = await GenerateUpcomingEventAsync(host);

			foreach (var guest in guests)
			{
				await Terminal.EventDatabase.SetUserStateAsync(guest.Id, eventStub.Id, EventBond.Guest);
			}

			return eventStub;
		}

		internal async Task<Event> GenerateOngoingEventAsync(User host, params User[] guests)
		{
			var eventStub = CreateTestEvent(host);
			eventStub.StartTime = DateTime.Now - TimeSpan.FromHours(2);

			eventStub = await GenerateEventUnsafeAsync(eventStub, host);
			await Terminal.EventDatabase.UpdateEventAsync(eventStub.Id, new() { (nameof(EventShard.State), EventState.Open) });
			await Terminal.EventDatabase.SetUserStateAsync(host.Id, eventStub.Id, EventBond.Arrived);

			foreach (var guest in guests)
			{
				await Terminal.EventDatabase.SetUserStateAsync(guest.Id, eventStub.Id, EventBond.Arrived);
			}

			return eventStub;
		}

		internal async Task<Event> GeneratePastEventAsync(User host, params User[] guests)
		{
			var eventStub = CreateTestEvent(host);
			eventStub.StartTime = DateTime.Now - TimeSpan.FromHours(2);

			eventStub = await GenerateEventUnsafeAsync(eventStub, host);
			await Terminal.EventDatabase.SetUserStateAsync(host.Id, eventStub.Id, EventBond.Arrived);

			foreach (var guest in guests)
			{
				await Terminal.EventDatabase.SetUserStateAsync(guest.Id, eventStub.Id, EventBond.Arrived);
			}

			await Terminal.EventDatabase.EndEventAsync(eventStub.Id);

			return eventStub;
		}

		internal async Task<Event> GenerateUniqueEventAsync(User host, params User[] guests)
		{
			var eventStub = CreateTestEvent(host);
			eventStub.Location = new() { Latitude = SafeAdd(ref uniqueEventDegree, 1), Longitude = SafeAdd(ref uniqueEventDegree, 1) };

			if (uniqueEventDegree > 80)
			{ uniqueEventDegree = -89.5; }

			eventStub = await GenerateEventUnsafeAsync(eventStub, host);

			foreach (var guest in guests)
			{
				await Terminal.EventDatabase.SetUserStateAsync(guest.Id, eventStub.Id, EventBond.Arrived);
			}

			return eventStub;
		}

		internal async Task<List<Event>> GenerateMultipleUniqueEventAsync(params User[] hosts)
		{
			double currentDegree = SafeAdd(ref uniqueEventDegree, 1);
			GeoLocation location = new() { Latitude = currentDegree, Longitude = currentDegree };

			if (uniqueEventDegree > 80)
			{ uniqueEventDegree = -89.5; }

			List<Event> events = new();

			foreach (var host in hosts)
			{
				var eventStub = CreateTestEvent(host);
				eventStub.Location = location;

				events.Add(await GenerateEventUnsafeAsync(eventStub, host));
			}

			return events;
		}

		internal async Task AddUserToEventAsync(Event @event, User user, EventBond state)
		{
			await Terminal.EventDatabase.SetUserStateAsync(user.Id, @event.Id, state);
		}

		internal async Task SetEventState(Event @event, EventState state)
		{
			await Terminal.EventDatabase.UpdateEventAsync(@event.Id, new() { (nameof(Event.State), state) });
			@event.State = state;
		}

		/////////
		// Etching Helpers
		////////////////////

		internal async Task<Etching> GenerateEtchingAsync(Event etchedEvent, User etcher)
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

		///////////
		// Notification Helpers
		/////////////////////////
		
		internal async Task SaveNoteAsync(User user, User notifier, string message, string action)
		{
			await Terminal.NotificationDatabase.SaveNoteAsync(user.Id, notifier.Id, new DateTime(0), message, action);
		}

		internal async Task<List<Note>> GetNotesAsync(User user)
		{
			return await Terminal.NotificationDatabase.GetNotesAsync(user.Id);
		}

		internal async Task SubscribeUserAsync(User user, DeviceType deviceType, string deviceToken)
		{
			await Terminal.NotificationDatabase.SubscribeUserAsync(user.Id, deviceType, deviceToken);
		}

		internal async Task<DeviceSilhouette> GetUserSubscriptionAsync(User user)
		{
			DeviceSilhouette subscription = null;

			try
			{
				subscription = await Terminal.NotificationDatabase.GetUserSubscriptionAsync(user.Id);
			}
			catch { }

			return subscription;
		}

		internal List<NotificationServiceStub.NotificationStub> GetUserMessages(User user)
		{
			ConcurrentBag<NotificationServiceStub.NotificationStub> userMessages;
			var exists = NotificationServiceStub.messages.TryGetValue(user.Id.ToString(), out userMessages);
			return exists ? userMessages.ToList() : new();
		}

		//////
		// Clean-up
		/////////////

		public async void DisposeEnvironment()
		{
			// Delete all users generated in this environment
            foreach (ulong id in generatedUserIds)
			{
				try
				{
					Console.WriteLine($"Deleting user {id}");
					await Terminal.AdminDatabase.VoidUserAsync(id);
					Console.WriteLine($"Deleted");
				}
				catch (Exception e) { Console.WriteLine(e); }
			}
		}

        public static double SafeAdd(ref double location1, double value)
        {
            double newCurrentValue = location1;
            while (true)
            {
                double currentValue = newCurrentValue;
                double newValue = currentValue + value;
                newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);
                if (newCurrentValue.Equals(currentValue))
                    return newValue;
            }
        }
    }
}
