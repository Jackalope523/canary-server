using System;
using Core.Boundaries;
using System.Threading.Tasks;
using Core.Entities;
using Xunit;

namespace Core.Tests.Entities
{
    public class EventTests
	{
		///////
		// Composition
		////////////////

		[Fact]
		public void ValidateAndNormalise_ValidEvent_ReturnsTrue()
		{
			// Arrange
			var validEvent = new Event
			{
				Name = "Valid Event",
				Description = "A valid event description",
				StartTime = DateTimeOffset.Now,
				GroupMinimum = 5,
				GroupMaximum = 10
			};

			// Act
			bool result = validEvent.ValidateAndNormalise();

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void ValidateAndNormalise_InvalidEvent_ReturnsFalse()
		{
			// Arrange
			var invalidEvent = new Event
			{
				Name = "Invalid Event",
				Description = "A".PadLeft(Event.MaximumDescLength + 1),
				StartTime = DateTimeOffset.Now - TimeSpan.FromDays(8),
				GroupMinimum = 5,
				GroupMaximum = 2
			};

			// Act
			bool result = invalidEvent.ValidateAndNormalise();

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void GetFriendsOf_ReturnsFriends()
		{
			
		}

		[Fact]
		public void GetFriendsOf_InvalidEvent_ReturnsFriends()
		{
			
		}

		/////
		// Checks
		///////////

		[Fact]
		public void IsVisibleTo_Neutral_ReturnsTrue()
		{
			
		}

		[Fact]
		public void IsVisibleTo_Blocked_ReturnsFalse()
		{
			
		}

		[Fact]
		public void IsJoinableTo_Neutral_ReturnsTrue()
		{
			
		}

		[Fact]
		public void IsJoinableTo_Blocked_ReturnsFalse()
		{
			
		}

		[Fact]
		public void IsModifiableBy_Host_ReturnsTrue()
		{
			
		}

		[Fact]
		public void IsModifiableBy_Neutral_ReturnsFalse()
		{
			
		}

		[Fact]
		public void IsHostedBy_Host_ReturnsTrue()
		{
			
		}

		[Fact]
		public void IsHostedBy_Neutral_ReturnsFalse()
		{
			
		}

		[Fact]
		public void HasUserRelationship_Host_ReturnsTrue()
		{
			
		}

		[Fact]
		public void HasUserRelationship_Watching_ReturnsTrue()
		{
			
		}

		[Fact]
		public void HasUserRelationship_Incoming_ReturnsTrue()
		{
			
		}

		[Fact]
		public void HasUserRelationship_Guest_ReturnsTrue()
		{
			
		}

		[Fact]
		public void HasUserRelationship_LeftGuest_ReturnsTrue()
		{
			
		}

		[Fact]
		public void HasUserRelationship_KickedGuest_ReturnsTrue()
		{
			
		}

		[Fact]
		public void HasUserRelationship_Neutral_ReturnsFalse()
		{
			
		}

		[Fact]
		public void WasAttendedBy_Guest_ReturnsTrue()
		{
			
		}

		[Fact]
		public void WasAttendedBy_LeftGuest_ReturnsTrue()
		{
			
		}

		[Fact]
		public void WasAttendedBy_KickedGuest_ReturnsFalse()
		{
			
		}

		[Fact]
		public void WasAttendedBy_Neutral_ReturnsTrue()
		{
			
		}

		[Fact]
		public void IsInRange_CloseUser_ReturnsTrue()
		{
			
		}

		[Fact]
		public void IsInRange_FarUser_ReturnsFalse()
		{
			
		}

		[Fact]
		public void IsStartable_Upcoming_ReturnsTrue()
		{
			
		}

		[Fact]
		public void IsStartable_Started_ReturnsFalse()
		{
			
		}

		[Fact]
		public void IsStartable_HostFar_ReturnsFalse()
		{
			
		}

		//////
		// Effects
		////////////

		[Fact]
		public void Started_Succeeds()
		{
			
		}

		[Fact]
		public void Ended_Succeeds()
		{
			
		}

		[Fact]
		public void Etched_Succeeds()
		{
			
		}

		[Fact]
		public void Reported_Succeeds()
		{
			
		}

		//////
		// Actions
		////////////

		[Fact]
		public void NotifyActive_Succeeds()
		{
			
		}

		[Fact]
		public void NotifyGuests_Succeeds()
		{
			
		}
	}
}
