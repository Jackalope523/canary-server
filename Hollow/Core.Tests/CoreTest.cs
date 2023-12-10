using Core.Boundaries;
using Core.Controls;
using Core.Entities;
using Shared;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Core.Tests.Entities
{
	public class CoreTest : IAsyncLifetime, IDisposable
	{
		private static int instanceCount = 0;
		
		protected readonly CoreEnvironment environment;

		public CoreTest()
		{
			instanceCount++;

			environment = new();
		}

		public async Task InitializeAsync()
		{ }

		public async Task DisposeAsync()
		{ }

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			if (--instanceCount == 0)
			{ environment.DisposeEnvironment(); }
		}
	}
}
