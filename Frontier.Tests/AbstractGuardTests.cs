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
		public async Task Execute_Shard_Success()
		{
			// Arrange
			var outgoing = new UserSilhouette(117, "John");
			Func<Task<object>> action = async () => outgoing;

			// Act
			var result = await testGuard.Execute(action) as ObjectResult;

			// Assert
			Assert.NotNull(result);

			var resultManifest = result.Value as UserSilhouette;
			Assert.NotNull(resultManifest);
			Assert.Equal(outgoing.Id, resultManifest.Id);
			Assert.Equal(outgoing.Name, resultManifest.Name);
		}

		[Fact]
		public async Task Execute_List_Success()
		{
			// Arrange
			List<UserSilhouette> outgoing = new() { new UserSilhouette(117, "John"), new UserSilhouette(3, "Thel") };
			Func<Task<object>> action = async () => outgoing;

			// Act
			var result = await testGuard.Execute(action) as ObjectResult;

			// Assert
			Assert.NotNull(result);

			var resultList = result.Value as List<UserSilhouette>;
			Assert.NotNull(resultList);
			Assert.Equal(outgoing.Count, resultList.Count);
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