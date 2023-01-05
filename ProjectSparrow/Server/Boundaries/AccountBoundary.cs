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
		Account GetAccount(string accountID);

		Account FindAccount(string identification);

		Account UpdateAccount(Account account);

		void DeleteAccount(string accountID);

		User GetUser(string accountID);

		User UpdateUser(User user);

		string GenerateAccountID();
	}
}
