using Core.Boundaries;
using Frontier.Controllers;
using Frontier.Manifests;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Xunit;

namespace Frontier.Tests
{
	public class AbstractGuardTests
	{
		AbstractGuard testGuard = new(null, null, null, null, null, null, null, null, null, null, null, null);

		[Fact]
		public async Task Execute_NoData_Success()
		{
			// Arrange
			var action = async () => NoOp();

			// Act
			var result = await testGuard.Execute(action) as ObjectResult;

			// Assert
			Assert.NotNull(result);
			Assert.Null(result.Value);
		}

		[Fact]
		public async Task Execute_String_Success()
		{
			// Arrange
			var action = async () => "some silly string";

			// Act
			var result = await testGuard.Execute(action) as ObjectResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal(await action.Invoke(), result.Value);
		}

		[Fact]
		public async Task Execute_Manifest_Success()
		{
			// Arrange
			var manifest = new UserSilhouetteManifest(new(117, "John"));
			var action = async () => manifest;

			// Act
			var result = await testGuard.Execute(action) as ObjectResult;

			// Assert
			Assert.NotNull(result);

			var resultManifest = result.Value as UserSilhouetteManifest;
			Assert.NotNull(resultManifest);
			Assert.Equal(manifest.Id, resultManifest.Id);
			Assert.Equal(manifest.Name, resultManifest.Name);
		}

		[Fact]
		public async Task Execute_List_Success()
		{
			// Arrange
			List<UserSilhouetteManifest> list = new() { new UserSilhouetteManifest(new(117, "John")), new UserSilhouetteManifest(new(3, "Thel")) };
			var action = async () => list;

			// Act
			var result = await testGuard.Execute(action) as ObjectResult;

			// Assert
			Assert.NotNull(result);

			var resultList = result.Value as List<UserSilhouetteManifest>;
			Assert.NotNull(resultList);
			Assert.Equal(list.Count, resultList.Count);
		}

		[Fact]
		public async Task Execute_InvalidData_Failure()
		{
			// Arrange
			UserSilhouette bastardData = new(117, "John");
			var action = async () => bastardData;

			// Act
			var resultSync = testGuard.Execute(action);

			// Assert
			await Assert.ThrowsAsync<HollowException>(async () => await resultSync);
		}

		private static void NoOp()
		{ }
	}
}