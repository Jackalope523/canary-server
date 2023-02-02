using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Server.Boundaries;

namespace Web.Stores
{
	public class UserAccountStore : IUserStore<ThinUser>,
		IUserPhoneNumberStore<ThinUser>,
		IUserEmailStore<ThinUser>,
		IUserSecurityStampStore<ThinUser>,
		IUserLockoutStore<ThinUser>
	{
		private IAccountOperations accounts { get; init; }

		public UserAccountStore(IAccountOperations accountOperations)
		{
			accounts = accountOperations;
		}

		public async Task<IdentityResult> CreateAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.CreateUserAsync(user.PhoneNumber, user.Email, user.Name, user.DateOfBirth);
			return IdentityResult.Success;
		}

		public async Task<IdentityResult> DeleteAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.DeleteUserAsync(user.Id);
			return IdentityResult.Success;
		}

		public async Task<ThinUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await accounts.GetUserAsync(GetGUID(userId));
		}

		public async Task<ThinUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await accounts.GetUserAsync(normalizedUserName);
		}

		public async Task<string> GetNormalizedUserNameAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await GetPhoneNumberAsync(user, cancellationToken);
		}

		public Task<string> GetUserIdAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.Id.ToString());
		}

		public async Task<string> GetUserNameAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await GetPhoneNumberAsync(user, cancellationToken);
		}

		public Task SetNormalizedUserNameAsync(ThinUser user, string normalizedName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(0);
		}

		public Task SetUserNameAsync(ThinUser user, string userName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(0);
		}

		public async Task<IdentityResult> UpdateAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id);
			return IdentityResult.Success;
		}

		public async Task SetPhoneNumberAsync(ThinUser user, string phoneNumber, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, phoneNumber: phoneNumber);
		}

		public Task<string> GetPhoneNumberAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.PhoneNumber);
		}

		public Task<bool> GetPhoneNumberConfirmedAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.IsPhoneConfirmed);
		}

		public async Task SetPhoneNumberConfirmedAsync(ThinUser user, bool confirmed, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, isPhoneNumberConfirmed: confirmed);
		}

		public async Task SetEmailAsync(ThinUser user, string email, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, email: email);
		}

		public Task<string> GetEmailAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.Email);
		}

		public Task<bool> GetEmailConfirmedAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.IsEmailConfirmed);
		}

		public async Task SetEmailConfirmedAsync(ThinUser user, bool confirmed, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, isEmailConfirmed: confirmed);
		}

		public async Task<ThinUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return await accounts.GetUserAsync(normalizedEmail);
		}

		public Task<string> GetNormalizedEmailAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.Email);
		}

		public Task SetNormalizedEmailAsync(ThinUser user, string normalizedEmail, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(0);
		}

		public async Task SetSecurityStampAsync(ThinUser user, string stamp, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, securityStamp: stamp);
		}

		public Task<string> GetSecurityStampAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.SecurityStamp);
		}

		public Task<DateTimeOffset?> GetLockoutEndDateAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.LockoutDate);
		}

		public async Task SetLockoutEndDateAsync(ThinUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (!lockoutEnd.HasValue)
			{
				throw new ArgumentException("Lockout null.");
			}
			await accounts.EditUserAsync(user.Id, lockoutDate: lockoutEnd.Value);
		}

		public async Task<int> IncrementAccessFailedCountAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			int currentTries = user.AccessTries;
			await accounts.EditUserAsync(user.Id, accessTries: currentTries + 1);
			return currentTries + 1;
		}

		public async Task ResetAccessFailedCountAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			await accounts.EditUserAsync(user.Id, accessTries: 0);
		}

		public Task<int> GetAccessFailedCountAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(user.AccessTries);
		}

		public Task<bool> GetLockoutEnabledAsync(ThinUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(true);
		}

		public Task SetLockoutEnabledAsync(ThinUser user, bool enabled, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(0);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		private Guid GetGUID(string id)
		{
			if (!Guid.TryParse(id, out Guid guid))
			{
				throw new ArgumentException("Not a valid GUID.", nameof(id));
			}

			return guid;
		}
	}
}
