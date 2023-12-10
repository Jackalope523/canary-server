using System;
using Core.Boundaries;
using System.Threading.Tasks;
using Core.Entities;
using Xunit;

namespace Core.Tests.Entities
{
    public class UserTests : CoreTest
    {
		///////
		// Composition
		////////////////

		[Fact]
		public void ValidateAndNormalise_ValidUser_ReturnsTrue()
		{
			// Arrange
			var validUser = new User
			{
				PhoneNumber = "+1234567890",
				Email = "user@example.com",
				DateOfBirth = DateTimeOffset.Now.AddYears(-25)
			};

			// Act
			bool result = validUser.ValidateAndNormalise();

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void ValidateAndNormalise_InvalidUser_ReturnsFalse()
		{
			// Arrange
			var invalidUser = new User
			{
				PhoneNumber = "invalid",
				Email = "invalid_email",
				DateOfBirth = DateTimeOffset.Now.AddYears(-15)
			};

			// Act
			bool result = invalidUser.ValidateAndNormalise();

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task CalculateReputation_UpdatesReputation()
		{

		}

		[Fact]
		public async Task NextEvent_HasEvent_ReturnsEvent()
		{

		}

		/////
		// Checks
		///////////

		[Fact]
		public async Task IsFriendsWith_Friend_ReturnsTrue()
		{

		}

		[Fact]
		public async Task IsFriendsWith_Neutral_ReturnsFalse()
		{

		}

		[Fact]
		public async Task IsFollowing_FollowedUser_ReturnsTrue()
		{

		}

		[Fact]
		public async Task IsFollowing_Neutral_ReturnsFalse()
		{

		}

		[Fact]
		public async Task IsBlocking_BlockedUser_ReturnsTrue()
		{

		}

		[Fact]
		public async Task IsBlocking_Neutral_ReturnsFalse()
		{

		}

		[Fact]
		public async Task IsBlockedBy_Blocked_ReturnsTrue()
		{

		}

		[Fact]
		public async Task IsBlockedBy_Neutral_ReturnsFalse()
		{

		}

		[Fact]
		public async Task IsAtEvent_AtEvent_ReturnsTrue()
		{

		}

		[Fact]
		public async Task IsAtEvent_Sload_ReturnsFalse()
		{

		}

		[Fact]
		public async Task CanView_ValidEvent_ReturnsTrue()
		{

		}

		[Fact]
		public async Task CanView_Blocked_ReturnsFalse()
		{

		}

		[Fact]
		public async Task CanView_Banned_ReturnsFalse()
		{

		}

		[Fact]
		public async Task CanJoin_JoinableEventAndVisibleUser_ReturnsTrue()
		{
			// Arrange
			var joinableEvent = new Event
			{
				State = EventState.upcoming,
				StartTime = DateTimeOffset.Now.AddHours(1),
				Location = new GeoLocation { Latitude = 1.0, Longitude = 2.0 },
				Radius = new Distance { Kilometres = 100 },
				IsDynamic = false
				// Set other required properties
			};

			var visibleUser = new User();
			visibleUser.LastKnownLocation.Set(new GeoLocation { Latitude = 1.5, Longitude = 2.5 });

			// Act
			bool result = await visibleUser.CanJoin(joinableEvent);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task Etched_OwnedEtching_ReturnsTrue()
		{

		}

		[Fact]
		public async Task Etched_UnownedEtching_ReturnsFalse()
		{

		}

		[Fact]
		public async Task CanReport_Chill_ReturnsTrue()
		{

		}

		[Fact]
		public async Task CanReport_Volatile_ReturnsFalse()
		{

		}

		//////
		// Effects
		////////////

		[Fact]
		public async Task HandleHaunt_Unmoved_Succeeds()
		{

		}

		[Fact]
		public async Task HandleHaunt_Moved_Succeeds()
		{

		}

		[Fact]
		public async Task Penalised_Unmoved_Succeeds()
		{

		}

		//////
		// Actions
		////////////

		[Fact]
		public async Task PostNote_Succeeds()
		{

		}

		[Fact]
		public async Task Notify_Succeeds()
		{

		}

		[Fact]
		public async Task NotifyFollowers_Succeeds()
		{

		}
	}
}
