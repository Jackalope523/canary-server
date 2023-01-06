using Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Boundaries
{
	public interface IAccountDatabase
	{
		void GetAccount(string accountID);
		void FindAccount(string identification);

		void UpdateAccount(string accountID);
		void DeleteAccount(string accountID);

		void GetUser(string accountID);
		void CreateUser(string identification);
		void UpdateUser(string userID);
	}
}
