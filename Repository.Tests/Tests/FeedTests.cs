using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Repository.Tests
{
    [Collection("Database Collection")]
    public class FeedTests : IDisposable
    {
        private static EFCoreSentry sentry = new(Harbor.Flag.Development);
        private static EFCoreEtchingStore store = new(Harbor.Flag.Development);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject;
        private Event testEvent;
        public FeedTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            subject = new UserFactory().Create();
            testEvent = new EventFactory().Create(subject);

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));
            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));
        }
        public void Dispose()
        {

        }


    
        [Fact]
        public async Task GenerateFeedForUserAsync_SUCCESS()
        {
            /// Feed test layout
            /// subject friends: e, j, m
            /// bait: x
            ///                    depth charge
            ///                         v 
            /// Event | Time Z | Time A | Time B | Time C | Time D
            /// alpha |        | e1 j1  | e2 x1  | e3     | m1
            /// echo  |        | j2 x2  | j3     |        |
            /// hotel |        |        |        | e4 m2  |
            /// kilo  | e5     |        |        |        |
            /// romeo |        | j4     |        |        | m3 x3
            /// 
            /// previously seen: kilo { e5 }
            /// returned feed: alpha { e1, j1, e2, e3, m1 }, echo { j2, j3 }, romeo { j4, m3 }
            /// 


            DateTimeOffset timeZ = DateTimeOffset.UtcNow;
            DateTimeOffset timeA = timeZ - TimeSpan.FromMinutes(60);
            DateTimeOffset timeB = timeA - TimeSpan.FromMinutes(60);
            DateTimeOffset depthCharge = timeA - TimeSpan.FromMinutes(90);
            DateTimeOffset timeC = timeA - TimeSpan.FromDays(120);
            DateTimeOffset timeD = timeA - TimeSpan.FromDays(180);


            // User creating block
            UserFactory userFactory = new UserFactory();
            User friendE = userFactory.Create();
            User friendJ = userFactory.Create();
            User friendM = userFactory.Create();
            User baitX = userFactory.Create();
            User irrelevantHost = userFactory.Create();

            sentry.ExecuteWrite(ctx => ctx.Users.Add(friendE));
            sentry.ExecuteWrite(ctx => ctx.Users.Add(friendJ));
            sentry.ExecuteWrite(ctx => ctx.Users.Add(friendM));
            sentry.ExecuteWrite(ctx => ctx.Users.Add(baitX));
            sentry.ExecuteWrite(ctx => ctx.Users.Add(irrelevantHost));


            // Friend making block
            UserLinkFactory factory = new UserLinkFactory();

            UserLink link1 = factory.Create(subject, friendE, UserLink.UserLinkType.Follow);
            UserLink link2 = factory.Create(friendE, subject, UserLink.UserLinkType.Follow);

            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link1));
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link2));

            link1 = factory.Create(subject, friendJ, UserLink.UserLinkType.Follow);
            link2 = factory.Create(friendJ, subject, UserLink.UserLinkType.Follow);

            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link1));
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link2));

            link1 = factory.Create(subject, friendM, UserLink.UserLinkType.Follow);
            link2 = factory.Create(friendM, subject, UserLink.UserLinkType.Follow);

            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link1));
            sentry.ExecuteWrite(ctx => ctx.UserLinks.Add(link2));


            // Event block
            EventFactory eventFactory = new EventFactory();

            Event alpha = eventFactory.Create(irrelevantHost);
            Event echo = eventFactory.Create(irrelevantHost);
            Event hotel = eventFactory.Create(irrelevantHost);
            Event kilo = eventFactory.Create(irrelevantHost);
            Event romeo = eventFactory.Create(irrelevantHost);

            sentry.ExecuteWrite(ctx => ctx.Events.Add(alpha));
            sentry.ExecuteWrite(ctx => ctx.Events.Add(echo));
            sentry.ExecuteWrite(ctx => ctx.Events.Add(hotel));
            sentry.ExecuteWrite(ctx => ctx.Events.Add(kilo));
            sentry.ExecuteWrite(ctx => ctx.Events.Add(romeo));


            // Post block
            // alpha
            Post e1 = new EtchingFactory().Create(friendE, alpha, timeA);
            Post j1 = new EtchingFactory().Create(friendJ, alpha, timeA);
            Post e2 = new EtchingFactory().Create(friendE, alpha, timeB);
            Post x1 = new EtchingFactory().Create(baitX, alpha, timeB);
            Post e3 = new EtchingFactory().Create(friendE, alpha, timeC);
            Post m1 = new EtchingFactory().Create(friendM, alpha, timeD);

            // echo
            Post j2 = new EtchingFactory().Create(friendJ, echo, timeA);
            Post j3 = new EtchingFactory().Create(friendJ, echo, timeB);
            Post x2 = new EtchingFactory().Create(baitX, echo, timeB);

            // hotel
            Post e4 = new EtchingFactory().Create(friendE, hotel, timeC);
            Post m2 = new EtchingFactory().Create(friendM, hotel, timeC);

            // kilo
            Post e5 = new EtchingFactory().Create(friendE, kilo, timeZ);

            // romeo
            Post j4 = new EtchingFactory().Create(friendJ, romeo, timeA);
            Post m3 = new EtchingFactory().Create(friendM, romeo, timeD);
            Post x3 = new EtchingFactory().Create(baitX, romeo, timeD);

            await BulkWritePost(e1, e2, e3, e4, e5, j1, j2, j3, j4, m1, m2, m3, x1, x2, x3);


            List<ulong> exclusionList = new() { kilo.Id };
            var retrieved = await store.GenerateFeedForUserAsync(subject.Id, depthCharge, timeA);


            Assert.NotNull(retrieved);
            Assert.Equal(9, retrieved.Count);

            EtchingShard e1Etching = retrieved.Find(etching => etching.Id.Equals(e1.Id));
            Assert.NotNull(e1Etching);
            //Assert.Equal(e1.OwnerId, e1Etching.UserId);
            Assert.Equal(e1.EventId, e1Etching.EventId);
            Assert.Equal(e1.PostedAt, e1Etching.TimeEtched);

            var retrievedAsPostIds = retrieved.ConvertAll(etching => etching.Id);

            Assert.Contains(e1.Id, retrievedAsPostIds);
            Assert.Contains(e2.Id, retrievedAsPostIds);
            Assert.Contains(e3.Id, retrievedAsPostIds);
            Assert.Contains(j1.Id, retrievedAsPostIds);
            Assert.Contains(j2.Id, retrievedAsPostIds);
            Assert.Contains(j3.Id, retrievedAsPostIds);
            Assert.Contains(j4.Id, retrievedAsPostIds);
            Assert.Contains(m1.Id, retrievedAsPostIds);
            Assert.Contains(m3.Id, retrievedAsPostIds);
        }
        private async Task BulkWritePost(params Post[] posts)
        {
            foreach (var post in posts)
            {
                sentry.ExecuteWrite(ctx => ctx.Posts.Add(post));
            }
        }    
    }
}
