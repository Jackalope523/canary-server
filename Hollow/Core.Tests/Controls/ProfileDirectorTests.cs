using Core.Boundaries;
using Core.Controls;
using Core.Entities;
using Shared;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Entities
{
    public class ProfileDirectorTests : CoreTest
    {
		private ProfileDirector director;

        public ProfileDirectorTests()
        {
			director = environment.Terminal.ProfileDirector;
        }

		[Fact]
		public async Task GetUserProfileAsync_Self_ReturnsProfile()
		{

		}

		[Fact]
		public async Task GetUserProfileAsync_Friend_ReturnsProfile()
		{

		}

		[Fact]
		public async Task GetUserProfileAsync_Neutral_ReturnsProfile()
		{

		}

		[Fact]
		public async Task GetUserProfileAsync_Blocked_Fails()
		{

		}

		[Fact]
		public async Task GetUserNestAsync_Self_ReturnsNest()
		{

		}

		[Fact]
		public async Task GetUserNestAsync_Friend_ReturnsNest()
		{

		}

		[Fact]
		public async Task GetUserNestAsync_NeutralHost_ReturnsNest()
		{

		}

		[Fact]
		public async Task GetUserNestAsync_Blocked_Fails()
		{

		}

		[Fact]
		public async Task GetUserActivityAsync_Self_ReturnsActivity()
		{

		}

		[Fact]
		public async Task GetUserActivityAsync_Friend_ReturnsActivity()
		{

		}

		[Fact]
		public async Task GetUserActivityAsync_Neutral_Fails()
		{

		}

		[Fact]
		public async Task GetUserActivityAsync_Blocked_Fails()
		{

		}

		[Fact]
		public async Task GetUserFriendActivityAsync_ReturnsFriendActivity()
		{

		}

		[Fact]
		public async Task GetFriendsAsync_ReturnsFriends()
		{

		}

		[Fact]
		public async Task GetFollowedUsersAsync_ReturnsUsers()
		{

		}

		[Fact]
		public async Task GetBlockedUsersAsync_ReturnsUsers()
		{

		}

		[Fact]
		public async Task FollowUserAsync_Succeeds()
		{

		}

		[Fact]
		public async Task FollowUserAsync_Blocked_Fails()
		{

		}

		[Fact]
		public async Task UnfollowUserAsync_FollowedUser_Succeeds()
		{

		}

		[Fact]
		public async Task BlockUserAsync_Succeeds()
		{

		}

		[Fact]
		public async Task UnblockUserAsync_Succeeds()
		{

		}

		[Fact]
		public async Task RateUserAsync_Self_Fails()
		{

		}

		[Fact]
		public async Task RateUserAsync_Neutral_Succeeds()
		{

		}

		[Fact]
		public async Task RateUserAsync_OverwriteRating_Succeeds()
		{

		}

		[Fact]
		public async Task RateUserAsync_MultipleRatings_Drops()
		{

		}
	}
}
