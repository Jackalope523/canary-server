using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Repository.Entities;
using Microsoft.AspNetCore.Identity;
using Core.Boundaries;

namespace Frontier.Stores
{
	public class UserAccountStore : IUserStore<UserShard>,
		IUserPhoneNumberStore<UserShard>,
		IUserEmailStore<UserShard>,
		IUserSecurityStampStore<UserShard>,
		IUserLockoutStore<UserShard>
	{
		private IAccountOperations accounts { get; init; }

		public UserAccountStore(IAccountOperations accountOperations)
		{
			accounts = accountOperations;
		}

		public async Task<IdentityResult> CreateAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.CreateUserAsync(user.PhoneNumber, user.Email, user.Name, user.DateOfBirth);
			return IdentityResult.Success;
		}

		public async Task<IdentityResult> DeleteAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.DeleteUserAsync(user.Id);
			return IdentityResult.Success;
		}

		public async Task<UserShard> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await accounts.GetUserAsync(GetId(userId));
		}

		public async Task<UserShard> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await accounts.GetUserAsync(normalizedUserName);
		}

		public async Task<string> GetNormalizedUserNameAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await GetPhoneNumberAsync(user, cancellationToken);
		}

		public Task<string> GetUserIdAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.Id.ToString());
		}

		public async Task<string> GetUserNameAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await GetPhoneNumberAsync(user, cancellationToken);
		}

		public Task SetNormalizedUserNameAsync(UserShard user, string normalizedName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(0);
		}

		public Task SetUserNameAsync(UserShard user, string userName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(0);
		}

		public async Task<IdentityResult> UpdateAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// No-Op'ing here because all operations that would update the user here
			// write to the database immediately instead of just locally.
			// This is based on our current database implementation but is subject to
			// change with a distributed architecture or different framework.
			return IdentityResult.Success;
		}

		public async Task SetPhoneNumberAsync(UserShard user, string phoneNumber, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, phoneNumber: phoneNumber);
		}

		public Task<string> GetPhoneNumberAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.PhoneNumber);
		}

		public Task<bool> GetPhoneNumberConfirmedAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.IsPhoneConfirmed);
		}

		public async Task SetPhoneNumberConfirmedAsync(UserShard user, bool confirmed, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, isPhoneNumberConfirmed: confirmed);
		}

		public async Task SetEmailAsync(UserShard user, string email, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, email: email);
		}

		public Task<string> GetEmailAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.Email);
		}

		public Task<bool> GetEmailConfirmedAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.IsEmailConfirmed);
		}

		public async Task SetEmailConfirmedAsync(UserShard user, bool confirmed, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, isEmailConfirmed: confirmed);
		}

		public async Task<UserShard> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await accounts.GetUserAsync(normalizedEmail);
		}

		public Task<string> GetNormalizedEmailAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.Email);
		}

		public Task SetNormalizedEmailAsync(UserShard user, string normalizedEmail, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(0);
		}

		public async Task SetSecurityStampAsync(UserShard user, string stamp, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, securityStamp: stamp);
		}

		public Task<string> GetSecurityStampAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.SecurityStamp);
		}

		public Task<DateTimeOffset?> GetLockoutEndDateAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.LockoutDate);
		}

		public async Task SetLockoutEndDateAsync(UserShard user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (!lockoutEnd.HasValue)
			{
				throw new ArgumentException("Lockout null.");
			}
			await accounts.EditUserAsync(user.Id, lockoutDate: lockoutEnd.Value);
		}

		public async Task<int> IncrementAccessFailedCountAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			int currentTries = user.AccessTries;
			await accounts.EditUserAsync(user.Id, accessTries: currentTries + 1);
			return currentTries + 1;
		}

		public async Task ResetAccessFailedCountAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, accessTries: 0);
		}

		public Task<int> GetAccessFailedCountAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.AccessTries);
		}

		public Task<bool> GetLockoutEnabledAsync(UserShard user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(true);
		}

		public Task SetLockoutEnabledAsync(UserShard user, bool enabled, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(0);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		private ulong GetId(string id)
		{
			if (!ulong.TryParse(id, out ulong parsedId))
			{
				throw new ArgumentException("Not a valid ulong.", nameof(id));
			}

			return parsedId;
		}
	}
}
