using Repository.Contexts;
using Repository.Entities;

namespace Repository.Tests
{
    public class UserQueryTests
    {
        private static TestContext _context = new TestContext();
        private static QueryStore store = new QueryStore(QueryStore.StoreMode.Test, 1);

        [Fact]
        public void CreateUser_Success()
        {
            // Arrange
            store.CreateUser("000-000-0000", "email_0@test.com", "user0", new DateTimeOffset(new DateTime(0)));

            // Act
            User created = _context.Users.Where(u => u.PhoneNumber == "000-000-0000").Single();

            // Assert
            Assert.NotNull(created);

        }
    }
}
