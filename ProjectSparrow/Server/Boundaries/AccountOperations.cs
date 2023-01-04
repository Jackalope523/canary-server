using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public interface IAccountOperations
	{
		public void GetUserProfile(string targetIdentification);

		public string TryLogin(string identification, string passkey);
		public void CreateUser(string identification, string passkey, string name, DateTime dateOfBirth);
		public void EditUser(string identification, string newName, DateTime newDateOfBirth, string newPhoto);
		public void DeleteUser(string identification);

		public List<string> GetFollowedUsers(string identification);
		public List<string> GetBlockedUsers(string identification);

		public void FollowUser(string identification, string targetIdentification);
		public void UnfollowUser(string identification, string targetIdentification);
		public void BlockUser(string identification, string targetIdentification);
		public void UnblockUser(string identification, string targetIdentification);

	}
}
