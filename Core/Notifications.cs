using System;
using Core.Boundaries;

namespace Core.Notifications
{
    public enum NotificationGroup
    {
        SocialInvitation,
        CompanionActivity,
        GatheringReminder,
        GatheringActivity,
        GatheringDiscovery,
    }

    public static class NotificationGroupExtensions
    {
        public static bool CheckEnabled(this NotificationGroup group, NotificationProfile profile)
        {
            return group switch
            {
                NotificationGroup.SocialInvitation => profile.SocialInvitation,
                NotificationGroup.CompanionActivity => profile.CompanionActivity,
                NotificationGroup.GatheringReminder => profile.GatheringReminder,
                NotificationGroup.GatheringActivity => profile.CompanionActivity,
                NotificationGroup.GatheringDiscovery => profile.GatheringDiscovery,
                _ => throw new ArgumentOutOfRangeException(nameof(group), group, null)
            };
        }
    }

    public interface IDeepLink
    {
        public static string BasePath => "almostcanary://";

        public string RelativePath { get; }

        public static string ParseOption(string option, bool? value)
        {
            if (value == null || !value.HasValue)
            { return ""; }

            return value.Value ? $"{option}=true&" : $"{option}=false&";
        }

        public static string ParseOption(string option, string value)
        {
            if (value == null)
            { return ""; }

            return $"{option}={value}&";
        }

        public static string ParseOption(string option, object value)
        {
            if (value == null)
            { return ""; }

            return $"{option}={value}&";
        }

        public static string ParseOption<T>(string option, T? value) where T : struct
        {
            if (value == null || !value.HasValue)
            { return ""; }

            return $"{option}={value.Value}&";
        }
    }

    public struct GatheringDeepLink : IDeepLink
    {
        public enum FocusTarget
        {
            GuestList,
            Gallery,
        }

        public string RelativePath { get; private set; }

        public GatheringDeepLink(long gatheringId,
            FocusTarget? focus = null, string invitedBy = null,
            bool? immediate = null, bool? @sealed = null)
        {
            RelativePath = $"{IDeepLink.BasePath}gathering/{gatheringId}";
            
            string options = "";

            options += IDeepLink.ParseOption("focus", focus);
            options += IDeepLink.ParseOption("invitedBy", invitedBy);
            options += IDeepLink.ParseOption("immediate", immediate);
            options += IDeepLink.ParseOption("sealed", @sealed);

            if (!string.IsNullOrEmpty(options))
            {
                RelativePath += $"?{options.Remove(options.Length - 1)}";
            }
        }
    }

    public struct DiscoveryDeepLink : IDeepLink
    {
        public string RelativePath => $"{IDeepLink.BasePath}discovery";
    }

    public struct NestDeepLink : IDeepLink
    {
        public string RelativePath { get; private set; }

        public NestDeepLink(long userId)
        {
            RelativePath = $"{IDeepLink.BasePath}nest/{userId}";
        }
    }

    public partial class CanaryNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string AppUrl { get; set; }
        public string CollapseId { get; set; }

        public NotificationGroup Group { get; set; }

        protected CanaryNotification(string title, string body, IDeepLink deepLink = null, string collapseId = "")
        {
            Title = title;
            Body = body;
            AppUrl = deepLink != null ? deepLink.RelativePath : "";
            CollapseId = collapseId;
        }

        public bool CheckEnabled(NotificationProfile profile)
        {
            return Group.CheckEnabled(profile);
        }
    }

    /////////
    // Social Invitations
    ///////////////////////

    public partial class CanaryNotification
    {
        protected static CanaryNotification SocialInvitation(CanaryNotification notification)
        {
            notification.Group = NotificationGroup.SocialInvitation;
            return notification;
        }

        public static CanaryNotification CompanionshipRequest(UserShard addingUser)
            => SocialInvitation(new("Companion Request",
                $"{addingUser} sent you a companionship request.",
                new NestDeepLink(addingUser.Id),
                "1"));

        public static CanaryNotification CompanionshipForged(UserShard addingUser)
            => SocialInvitation(new("New Companion",
                $"Companionship forged with {addingUser.Name} accepted.",
                new NestDeepLink(addingUser.Id),
                "1"));

        public static CanaryNotification GatheringInvitation(UserShard invitingUser, GatheringShard gathering)
            => SocialInvitation(new("Gathering Invitation",
                $"{invitingUser.Name} invited you to {gathering.Title}.",
                new GatheringDeepLink(gathering.Id, invitedBy: invitingUser.Name),
                $"{gathering.Id}:1"));
    }

    /////////
    // Companion Activity
    ///////////////////////

    public partial class CanaryNotification
    {
        protected static CanaryNotification CompanionActivity(CanaryNotification notification)
        {
            notification.Group = NotificationGroup.CompanionActivity;
            return notification;
        }

        public static CanaryNotification CompanionJoined(UserShard companion, GatheringShard gathering)
            => CompanionActivity(new(gathering.Title,
                $"{companion.Name} joined the gathering.",
                new GatheringDeepLink(gathering.Id, focus: GatheringDeepLink.FocusTarget.GuestList),
                $"{gathering.Id}:10"));

        public static CanaryNotification CompanionGatheringCreated(UserShard companion, GatheringShard gathering)
            => CompanionActivity(new("Companion Gathering",
                $"{companion.Name} just created {gathering.Title}",
                new GatheringDeepLink(gathering.Id)));
    }

    //////////
    // Gathering Discovery
    ////////////////////////

    public partial class CanaryNotification
    {
        protected static CanaryNotification GatheringDiscovery(CanaryNotification notification)
        {
            notification.Group = NotificationGroup.GatheringDiscovery;
            return notification;
        }

        public static CanaryNotification NearbyGatherings() // TODO Slot in
            => GatheringDiscovery(new("New Gatherings Nearby",
                "There are new gatherings in your area that you may be interested in.",
                new DiscoveryDeepLink()));
        // TODO A. Need to actually ensure that they are new (gathering creation time vs last logged in) B. not send multiple
        // ^Advanced profile and filter system

        public static CanaryNotification CompanionMotive(GatheringShard gathering) // TODO Slot in
            => GatheringDiscovery(new("Companion Movement",
                "Your companions are headed somewhere interesting...",
                new GatheringDeepLink(gathering.Id)));
    }

    //////////
    // Gathering Reminders
    ////////////////////////

    public partial class CanaryNotification
    {
        protected static CanaryNotification GatheringReminders(CanaryNotification notification)
        {
            notification.Group = NotificationGroup.GatheringReminder;
            return notification;
        }

        public static CanaryNotification GatheringUpcoming(GatheringShard gathering, string relativeTime = "later")
            => GatheringReminders(new(gathering.Title,
                $"Is starting {relativeTime}.",
                new GatheringDeepLink(gathering.Id),
                "20"));

        public static CanaryNotification GatheringImminent(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title,
                $"Is starting shortly.",
                new GatheringDeepLink(gathering.Id, immediate: true),
                "20"));

        public static CanaryNotification GatheringLive(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title,
                $"Is now live!",
                new GatheringDeepLink(gathering.Id, immediate: true),
                "20"));

        public static CanaryNotification GatheringCancelled(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title,
                $"Was cancelled by the host.",
                new GatheringDeepLink(gathering.Id),
                "20"));

        public static CanaryNotification GatheringEdited(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title,
                $"Was modified by the host.",
                new GatheringDeepLink(gathering.Id),
                "21"));

        public static CanaryNotification GatheringUploadClosing(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title,
                $"Post your remaining photos before the upload window closes.",
                new GatheringDeepLink(gathering.Id, focus: GatheringDeepLink.FocusTarget.Gallery)));
    }

    /////////
    // Gathering Activity
    ///////////////////////

    public partial class CanaryNotification
    {
        protected static CanaryNotification GatheringActivity(CanaryNotification notification)
        {
            notification.Group = NotificationGroup.GatheringActivity;
            return notification;
        }

        // Host

        public static CanaryNotification GatheringSealed(GatheringShard gathering)
            => GatheringActivity(new(gathering.Title,
                $"Was reported too many times and was sealed as a result.",
                new GatheringDeepLink(gathering.Id, @sealed: true)));

        public static CanaryNotification GatheringWaiting(GatheringShard gathering)
            => GatheringActivity(new(gathering.Title,
                $"Your gathering is waiting to start!",
                new GatheringDeepLink(gathering.Id, immediate: true),
                "30"));

        public static CanaryNotification GatheringAutoCancellationWarning(GatheringShard gathering)
            => GatheringActivity(new(gathering.Title,
                $"Is going to be cancelled if you do not start it.",
                new GatheringDeepLink(gathering.Id, immediate: true),
                "30"));

        public static CanaryNotification GatheringAutoCancelled(GatheringShard gathering)
            => GatheringActivity(new(gathering.Title,
                $"Was cancelled due to your absence.",
                new GatheringDeepLink(gathering.Id),
               "30"));

        public static CanaryNotification GatheringHeartbeat(GatheringShard gathering) // TODO Slot in
            => GatheringActivity(new(gathering.Title,
                $"Is the gathering still ongoing?",
                new GatheringDeepLink(gathering.Id, immediate: true)));

        public static CanaryNotification HostLeavingGatheringArea(GatheringShard gathering)
            => GatheringActivity(new(gathering.Title,
                $"You are leaving the gathering area, gathering will hide itself.",
                new GatheringDeepLink(gathering.Id)));

        // Attendee

        public static CanaryNotification AttendeeArrived(GatheringShard gathering) // TODO Slot in
            => GatheringActivity(new(gathering.Title,
                "You have entered the gathering area, get on now!",
                new GatheringDeepLink(gathering.Id, immediate: true),
                "30"));

        public static CanaryNotification AttendeeLeavingGatheringArea(GatheringShard gathering)
            => GatheringActivity(new(gathering.Title,
                $"You are leaving the gathering area.",
                new GatheringDeepLink(gathering.Id),
                "30"));

        public static CanaryNotification GatheringTerminated(GatheringShard gathering)
            => GatheringActivity(new(gathering.Title,
                $"Has ended. Thanks for joining!",
                new GatheringDeepLink(gathering.Id),
                "30"));

        public static CanaryNotification HostMissedGathering(GatheringShard gathering)
            => GatheringActivity(new(gathering.Title,
                $"Was cancelled due to an absent host.",
                new GatheringDeepLink(gathering.Id),
               "20"));

        public static CanaryNotification UserMissedGathering(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title,
                "You missed the gathering.",
                new GatheringDeepLink(gathering.Id),
                "20"));
    }
}
