using Core.Boundaries;
using Core.Controls;
using Core.Entities;
using Shared;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Entities
{
	public class CoreTest : IAsyncLifetime
	{
		protected readonly CoreEnvironment environment;

		public CoreTest()
		{
			environment = new();
		}

		public async Task InitializeAsync()
		{ }

		public Task DisposeAsync()
		{
			environment.Dispose();
			return Task.CompletedTask;
		}
	}
}
