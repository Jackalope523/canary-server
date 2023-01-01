using Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	internal interface IAccountDatabase
	{
		public Account GetAccount(string accountID);

		public Account FindAccount(string identification);

		public Account UpdateAccount(Account account);

		public User GetUser(string accountID);

		public User UpdateUser(User user);

		public string GenerateAccountID();
	}
}
