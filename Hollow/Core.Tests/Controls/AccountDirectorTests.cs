using Core.Boundaries;
using Core.Controls;
using Core.Entities;
using Shared;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Entities
{
    public class AccountDirectorTests : IAsyncLifetime
    {
        private TestEnvironment environment;
		private AccountDirector director;

		private User testUser;

        public AccountDirectorTests()
        {
            environment = new();
			director = environment.Terminal.AccountDirector;
        }

		public async Task InitializeAsync()
		{
			testUser = await environment.GenerateTestUserAsync();
		}

		public Task DisposeAsync()
		{
			environment.Dispose();
			return Task.CompletedTask;
		}

		[Fact]
		public async Task GetUserAsync_ValidId_ReturnsUser()
		{
			// Act
			var user = await director.GetUserAsync(testUser.Id);

			// Assert
			Assert.True(testUser.Equals(user));
		}

		[Fact]
		public async Task GetUserAsync_InvalidId_ThrowsException()
		{
			// Act
			var userSync = director.GetUserAsync(ulong.MaxValue);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await userSync);
		}

		[Fact]
		public async Task GetUserAsync_ValidPhoneNumber_ReturnsUser()
		{
			// Act
			var user = await director.GetUserAsync(testUser.PhoneNumber);

			// Assert
			Assert.True(testUser.Equals(user));
		}

		[Fact]
		public async Task GetUserAsync_InvalidPhoneNumber_ThrowsException()
		{
			// Act
			var userSync = director.GetUserAsync("000");

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await userSync);
		}

		[Fact]
		public async Task CreateUserAsync_ValidUser_Succeeds()
		{
			// Arrange
			var newUser = environment.CreateTestUser();

			// Act
			await director.CreateUserAsync(newUser.PhoneNumber,
				newUser.Email, newUser.Name, newUser.DateOfBirth);
			// If no exception is thrown, the test is successful
		}

		[Fact]
		public async Task CreateUserAsync_ExistingUser_ThrowsException()
		{
			// Act
			var userSync = director.CreateUserAsync(testUser.PhoneNumber,
				testUser.Email, testUser.Name, testUser.DateOfBirth);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await userSync);
		}

		[Fact]
		public async Task EditUserAsync_ValidInput_UpdatesUser()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();
			string newName = "Henry Old Boy";

			// Act
			await director.EditUserAsync(user.Id, name: newName);

			// Assert
			var updatedUser = await director.GetUserAsync(user.Id);
			Assert.Equal(user.Name, updatedUser.Name);
		}

		[Fact]
		public async Task DeleteUserAsync_ValidUser_DeletesUser()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();

			// Act
			await director.DeleteUserAsync(user.Id);

			// Assert
			await Assert.ThrowsAnyAsync<HollowException>(async () => await director.GetUserAsync(user.Id));
		}

		[Fact]
		public async Task UpdateUserLocationAsync_UpdatesLocation()
		{
			// Arrange
			var user = await environment.GenerateTestUserAsync();
			await user.LastKnownLocation.Sync();
			GeoLocation newLocation = new() { Latitude = -1, Longitude = -1 };

			// Act
			await director.UpdateUserLocationAsync(user.Id, newLocation.Latitude, newLocation.Longitude);
			User updatedUser = new(await director.GetUserAsync(user.Id));

			// Assert
			Assert.NotEqual(await user.LastKnownLocation.Value(),
				await updatedUser.LastKnownLocation.Value());
		}
	}
}
