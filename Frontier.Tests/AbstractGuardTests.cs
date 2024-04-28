using Core.Boundaries;
using Frontier.Controllers;
using Frontier.Manifests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

using Xunit;

namespace Frontier.Tests
{
	public class AbstractGuardTests
	{
		AbstractGuard testGuard = new(null, null);

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
			Func<Task<object>> action = async () => "some silly string";

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
			Func<Task<object>> action = async () => manifest;

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
			ManifestSeries<UserSilhouetteManifest> manifest = new() { new UserSilhouetteManifest(new(117, "John")), new UserSilhouetteManifest(new(3, "Thel")) };
			Func<Task<object>> action = async () => manifest;

			// Act
			var result = await testGuard.Execute(action) as ObjectResult;

			// Assert
			Assert.NotNull(result);

			var resultList = result.Value as ManifestSeries<UserSilhouetteManifest>;
			Assert.NotNull(resultList);
			Assert.Equal(manifest.Count, resultList.Count);
		}

		[Fact]
		public async Task Execute_InvalidData_Failure()
		{
			// Arrange
			UserSilhouette bastardData = new(117, "John");
			Func<Task<object>> action = async () => bastardData;

			// Act
			var result = await testGuard.Execute(action) as IStatusCodeActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.NotEqual(200, result.StatusCode);
		}

		private static void NoOp()
		{ }
	}
}