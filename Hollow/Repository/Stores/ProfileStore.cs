using Core.Boundaries;
using Shared;

namespace Repository
{
    public class ProfileStore : QueryStore, IProfileDatabase
    {
        public static IProfileDatabase ProfileDatabaseAccess => new ProfileStore(new TestSentry());

        public ProfileStore(Sentry sentry) : base(sentry)
        {
        }

        public async Task<bool> FollowUserAsync(Guid selfId, Guid targetId) { return await AddLinkOperationAsync(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }); }
        public async Task<bool> UnfollowUserAsync(Guid selfId, Guid targetId) { return await RemoveLinkOperationAsync(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }); }
        public async Task<bool> BlockUserAsync(Guid selfId, Guid targetId) { return await AddLinkOperationAsync(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }); }
        public async Task<bool> UnblockUserAsync(Guid selfId, Guid targetId) { return await RemoveLinkOperationAsync(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }); }


        public async Task<List<UserSilhouette>> GetFollowedUsersAsync(Guid id) { return await GetUsersByAsync(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow); }
        public async Task<List<UserSilhouette>> GetBlockedUsersAsync(Guid id) { return await GetUsersByAsync(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Block); }
        public async Task<List<UserSilhouette>> GetFriendsAsync(Guid id)
        {
            Task<List<UserSilhouette>> following = GetUsersByAsync(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow);
            Task<List<UserSilhouette>> followingMe = GetUsersByAsync(l => l.OtherId == id && l.Type == UserLink.UserLinkType.Follow);
            List<UserSilhouette> a = await following;
            List<UserSilhouette> b = await followingMe;
            return a.Intersect(b).ToList();
        }

        public async Task<(int Positive, int Negative)> GetUserRatingsAsync(Guid id)
        {
            List<UserLink.UserLinkType> ratings;
            ratings = await storeSentry.ExecuteReadAsync(async ctx => ctx.UserLinks.Where(l => l.OtherId == id && (l.Type == UserLink.UserLinkType.RateUp || l.Type == UserLink.UserLinkType.RateDown)).Select(l => l.Type).ToList());

            int up = 0;
            int down = 0;
            foreach (UserLink.UserLinkType rating in ratings)
            {
                if (rating == UserLink.UserLinkType.RateUp) up++;
                else down--;
            }

            return (up, down);
        }

        public async Task<bool> RateUserAsync(Guid selfId, Guid targetId, UserRating rating)
        {
            UserLink.UserLinkType type;
            if (rating.Equals(UserRating.Positive)) type = UserLink.UserLinkType.RateUp;
            else type = UserLink.UserLinkType.RateDown;

            return await AddLinkOperationAsync(new UserLink { SelfId = selfId, OtherId = targetId, Type = type });
        }

        public async Task<bool> RemoveUserRatingAsync(Guid selfId, Guid targetId)
        {
            return await RemoveLinkOperationAsync(new UserLink { SelfId = selfId, OtherId = targetId });
        }
    }
}
