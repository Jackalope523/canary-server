using Core.Boundaries;
using Shared;

namespace Repository
{
    internal class ProfileStore : QueryStore, IProfileDatabase
    {
        public static IProfileDatabase ProfileDatabaseAccess => new ProfileStore(new TestSentry());

        public ProfileStore(Sentry sentry) : base(sentry)
        {
        }

        public bool FollowUser(Guid selfId, Guid targetId) { return addLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }); }
        public bool UnfollowUser(Guid selfId, Guid targetId) { return removeLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Follow }); }
        public bool BlockUser(Guid selfId, Guid targetId) { return addLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }); }
        public bool UnblockUser(Guid selfId, Guid targetId) { return removeLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = UserLink.UserLinkType.Block }); }


        public List<UserSilhouette> GetFollowedUsers(Guid id) { return getUsersBy(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow); }
        public List<UserSilhouette> GetBlockedUsers(Guid id) { return getUsersBy(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Block); }
        public List<UserSilhouette> GetFriends(Guid id)
        {
            List<UserSilhouette> following = getUsersBy(l => l.SelfId == id && l.Type == UserLink.UserLinkType.Follow);
            List<UserSilhouette> followingMe = getUsersBy(l => l.OtherId == id && l.Type == UserLink.UserLinkType.Follow);
            return following.Intersect(followingMe).ToList();
        }

        public (int Positive, int Negative) GetUserRatings(Guid id)
        {
            List<UserLink.UserLinkType> ratings;
            ratings = storeSentry.ExecuteRead(ctx => ctx.UserLinks.Where(l => l.OtherId == id && (l.Type == UserLink.UserLinkType.RateUp || l.Type == UserLink.UserLinkType.RateDown)).Select(l => l.Type).ToList());

            int up = 0;
            int down = 0;
            foreach (UserLink.UserLinkType rating in ratings)
            {
                if (rating == UserLink.UserLinkType.RateUp) up++;
                else down--;
            }

            return (up, down);
        }

        public bool RateUser(Guid selfId, Guid targetId, UserRating rating)
        {
            UserLink.UserLinkType type;
            if (rating.Equals(UserRating.Positive)) type = UserLink.UserLinkType.RateUp;
            else type = UserLink.UserLinkType.RateDown;

            return addLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId, Type = type });
        }

        public bool RemoveUserRating(Guid selfId, Guid targetId)
        {
            return removeLinkOperation(new UserLink { SelfId = selfId, OtherId = targetId });
        }
    }
}
