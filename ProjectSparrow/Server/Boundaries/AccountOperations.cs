using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public interface IAccountOperations
	{
		void GetUserProfile(string targetIdentification);

		string TryLogin(string identification, string passkey);
		void CreateUser(string identification, string passkey, string name, DateTime dateOfBirth);
		void EditUser(string identification, string newName, DateTime newDateOfBirth, string newPhoto);
		void DeleteUser(string identification);

		List<string> GetFollowedUsers(string identification);
		List<string> GetBlockedUsers(string identification);

		void FollowUser(string identification, string targetIdentification);
		void UnfollowUser(string identification, string targetIdentification);
		void BlockUser(string identification, string targetIdentification);
		void UnblockUser(string identification, string targetIdentification);

	}
}
