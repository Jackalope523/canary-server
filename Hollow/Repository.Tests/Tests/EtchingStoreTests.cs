using Core.Boundaries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.GeometriesGraph;
using System.Reflection.Metadata.Ecma335;
using Xunit.Abstractions;

namespace Repository.Tests
{
    [Collection("Database Collection")]
    public class EtchingStoreTests : IDisposable
    {
        private static TestSentry sentry = new TestSentry();
        private static EtchingStore etchingStore = new EtchingStore(sentry);

        private readonly ITestOutputHelper _testOutputHelper;

        private User subject;
        private Event testEvent;
        public EtchingStoreTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            subject = new UserFactory().Create();
            testEvent = new EventFactory().Create(subject);

            sentry.ExecuteWrite(ctx => ctx.Users.Add(subject));
            sentry.ExecuteWrite(ctx => ctx.Events.Add(testEvent));
        }
        public void Dispose()
        {
            sentry.ExecuteWrite(ctx => ctx.PostLinks.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Posts.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Users.ExecuteDelete());
            sentry.ExecuteWrite(ctx => ctx.Events.ExecuteDelete());
        }

        [Fact]
        public async Task AddEtchingAsync_SUCCESS()
        {
            DateTimeOffset postTime = DateTimeOffset.MinValue;
            string url = "URL";

            await etchingStore.AddEtchingAsync(testEvent.Id, subject.Id, postTime, url);

            Post created = await sentry.ExecuteReadAsync(ctx => ctx.Posts.FirstAsync());

            Assert.NotNull(created);
            Assert.Equal(subject.Id, created.OwnerId);
            Assert.Equal(testEvent.Id, created.EventId);
            Assert.Equal(postTime, created.PostedAt);
            Assert.Equal(url, created.PhotoURL);
            Assert.False(created.IsHidden);
        }
        [Fact]
        public async Task RemoveEtchingAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            await sentry.ExecuteWriteAsync(ctx => ctx.Posts.AddAsync(testEtching));

            await etchingStore.RemoveEtchingAsync(testEtching.Id);

            int numPosts = await sentry.ExecuteReadAsync(ctx => ctx.Posts.CountAsync());

            Assert.Equal(0, numPosts);
        }
        [Fact]
        public async Task GetEtchingAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            await sentry.ExecuteWriteAsync(ctx => ctx.Posts.AddAsync(testEtching));

            Etching retrieved = await etchingStore.GetEtchingAsync(testEtching.Id);

            Assert.NotNull(retrieved);
            Assert.Equal(testEtching.OwnerId, retrieved.UserId);
            Assert.Equal(testEtching.EventId, retrieved.EventId);
            Assert.Equal(testEtching.PostedAt, retrieved.TimeEtched);
            Assert.Equal(testEtching.PhotoURL, retrieved.ImageURL);
            Assert.Equal(testEtching.IsHidden, retrieved.IsHidden);
        }
        [Fact]
        public async Task GetEtchingsByUserAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            sentry.ExecuteWrite(ctx => ctx.Posts.Add(testEtching));

            int a = sentry.ExecuteRead(ctx => ctx.Posts.Count());
            _testOutputHelper.WriteLine(a.ToString());

           Etching retrieved = (await etchingStore.GetEtchingsByUserAsync(subject.Id)).First();

            Assert.NotNull(retrieved);
            Assert.Equal(testEtching.OwnerId, retrieved.UserId);
            Assert.Equal(testEtching.EventId, retrieved.EventId);
            Assert.Equal(testEtching.PostedAt, retrieved.TimeEtched);
            Assert.Equal(testEtching.PhotoURL, retrieved.ImageURL);
            Assert.Equal(testEtching.IsHidden, retrieved.IsHidden);
        }
        [Fact]
        public async Task GetEtchingsForEventAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            sentry.ExecuteWrite(ctx => ctx.Posts.Add(testEtching));

            Etching retrieved = (await etchingStore.GetEtchingsForEventAsync(testEvent.Id)).First();

            Assert.NotNull(retrieved);
            Assert.Equal(testEtching.OwnerId, retrieved.UserId);
            Assert.Equal(testEtching.EventId, retrieved.EventId);
            Assert.Equal(testEtching.PostedAt, retrieved.TimeEtched);
            Assert.Equal(testEtching.PhotoURL, retrieved.ImageURL);
            Assert.Equal(testEtching.IsHidden, retrieved.IsHidden);
        }
        [Fact]
        public async Task RateEtchingAsync_SUCCESS()
        {
            PostLink.PostLinkType rating = PostLink.PostLinkType.RateUp;
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            sentry.ExecuteWrite(ctx => ctx.Posts.Add(testEtching));

            await etchingStore.RateEtchingAsync(testEtching.Id, subject.Id, Shared.UserRating.Positive);

            PostLink created = await sentry.ExecuteReadAsync(ctx => ctx.PostLinks.FirstAsync());

            Assert.Equal(subject.Id, created.UserId);
            Assert.Equal(testEtching.Id, created.PostId);
            Assert.Equal(rating, created.Type);
        }
        [Fact]
        public async Task RemoveEtchingRatingAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            await sentry.ExecuteWriteAsync(ctx => ctx.Posts.AddAsync(testEtching));

            PostLink rating = new PostLinkFactory().Create(subject, testEtching);
            await sentry.ExecuteWriteAsync(ctx => ctx.PostLinks.AddAsync(rating));

            await etchingStore.RemoveEtchingRatingAsync(testEtching.Id, subject.Id);

            int count = await sentry.ExecuteReadAsync(ctx => ctx.PostLinks.CountAsync());

            Assert.Equal(0, count);
        }
        [Fact]
        public async Task GenerateFeedForUserAsync_SUCCESS()
        {
            /// Feed test layout
            /// subject friends: e, j, m
            /// bait: x
            ///                    depth charge
            ///                         v 
            /// Event | Time A | Time B | Time C | Time D
            /// alpha | e1 j1  | e2 x1  | e3     | m1
            /// echo  | j2 x2  | j3     |        |
            /// hotel |        |        | e4 m2  |
            /// kilo  | e5     | j4     | m3     |
            /// romeo | j5     |        |        | m4 x3
            /// 
            /// exclusion list: kilo
            /// returned feed: alpha { e1, j1, e2, e3, m1 }, echo { j2, j3 }, romeo { j5, m4 }
            /// 


            DateTimeOffset timeA = DateTimeOffset.UtcNow;
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
			Post e5 = new EtchingFactory().Create(friendE, kilo, timeA);
			Post j4 = new EtchingFactory().Create(friendJ, kilo, timeB);
			Post m3 = new EtchingFactory().Create(friendM, kilo, timeC);

            // romeo
			Post j5 = new EtchingFactory().Create(friendJ, romeo, timeA);
			Post m4 = new EtchingFactory().Create(friendM, romeo, timeD);
			Post x3 = new EtchingFactory().Create(baitX, romeo, timeD);
            
            await BulkWritePost(e1, e2, e3, e4, e5, j1, j2, j3, j4, j5, m1, m2, m3, m4, x1, x2, x3);


            List<ulong> exclusionList = new() { kilo.Id };
			var retrieved = await etchingStore.GenerateFeedForUserAsync(subject.Id, depthCharge, exclusionList);


			Assert.NotNull(retrieved);
            Assert.Equal(9, retrieved.Count);

            Etching e1Etching = retrieved.Find(etching => etching.Id.Equals(e1.Id));
            Assert.NotNull(e1Etching);
            Assert.Equal(e1.OwnerId, e1Etching.UserId);
            Assert.Equal(e1.EventId, e1Etching.EventId);
            Assert.Equal(e1.PhotoURL, e1Etching.ImageURL);
            Assert.Equal(e1.PostedAt, e1Etching.TimeEtched);

            var retrievedAsPostIds = retrieved.ConvertAll(etching => etching.Id);

            Assert.Contains(e1.Id, retrievedAsPostIds);
            Assert.Contains(e2.Id, retrievedAsPostIds);
            Assert.Contains(e3.Id, retrievedAsPostIds);
            Assert.Contains(j1.Id, retrievedAsPostIds);
            Assert.Contains(j2.Id, retrievedAsPostIds);
            Assert.Contains(j3.Id, retrievedAsPostIds);
            Assert.Contains(j5.Id, retrievedAsPostIds);
            Assert.Contains(m1.Id, retrievedAsPostIds);
            Assert.Contains(m4.Id, retrievedAsPostIds);
		}
        private async Task BulkWritePost(params Post[] posts)
        {
            foreach (var post in posts)
            {
                sentry.ExecuteWrite(ctx => ctx.Posts.Add(post));
            }
        }
        [Fact]
        public async Task HideEtchingAsync_SUCCESS()
        {
            Post testEtching = new EtchingFactory().Create(subject, testEvent);
            sentry.ExecuteWrite(ctx => ctx.Posts.Add(testEtching));

            await etchingStore.HideEtchingAsync(testEtching.Id);

            Etching retrieved = (await etchingStore.GetEtchingsForEventAsync(testEvent.Id)).First();

            Assert.NotNull(retrieved);
            Assert.Equal(testEtching.OwnerId, retrieved.UserId);
            Assert.Equal(testEtching.EventId, retrieved.EventId);
            Assert.Equal(testEtching.PostedAt, retrieved.TimeEtched);
            Assert.Equal(testEtching.PhotoURL, retrieved.ImageURL);
            Assert.NotEqual(testEtching.IsHidden, retrieved.IsHidden);
            Assert.True(retrieved.IsHidden);
        }
    }
}

