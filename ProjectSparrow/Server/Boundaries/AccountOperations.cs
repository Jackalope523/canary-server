using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public interface IAccountOperations
	{
		public void GetUserProfile(string identification, string targetIdentification);

		public void CreateUser(string identification, string name, DateTime dateOfBirth);
		public void EditUser(string identification);
		public void DeleteUser(string identification);

		public void UpdatePhoto(string identification);

		public void GetFollowedUsers(string identification);
		public void GetBlockedUsers(string identification);

		public void FollowUser(string identification, string targetIdentification);
		public void UnfollowUser(string identification, string targetIdentification);
		public void BlockUser(string identification, string targetIdentification);
		public void UnblockUser(string identification, string targetIdentification);

	}
}
