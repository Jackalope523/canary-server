using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Boundaries;
using Core.Entities;

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
        public static string GetString(this NotificationGroup group)
        {
            return group switch
            {
                NotificationGroup.SocialInvitation => "preferences/social_invitations",
                NotificationGroup.CompanionActivity => "preferences/companion_activity",
                NotificationGroup.GatheringReminder => "preferences/gathering_reminders",
                NotificationGroup.GatheringActivity => "preferences/gathering_activity",
                NotificationGroup.GatheringDiscovery => "preferences/gathering_discovery",
                _ => throw new ArgumentOutOfRangeException(nameof(group), group, null)
            };
        }
    }

    public partial class CanaryNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string AppUrl { get; set; }
        public string CollapseId { get; set; }

        public NotificationGroup Group { get; set; }

        protected CanaryNotification(string title, string body, string deepLink = "", string collapseId = "")
        {
            Title = title;
            Body = body;
            AppUrl = deepLink;
            CollapseId = collapseId;
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

        public static CanaryNotification UserAdded(UserShard addingUser)
            => SocialInvitation(new("Companion Request", $"{addingUser} added you.", $"nest/{addingUser.Id}", "1"));

        public static CanaryNotification CompanionshipForged(UserShard addingUser)
            => SocialInvitation(new("New Companion", $"Companionship forged, {addingUser.Name} added you.", $"nest/{addingUser.Id}", "1"));

        public static CanaryNotification GatheringInvitation(UserShard invitingUser, GatheringShard gathering)
            => SocialInvitation(new("Gathering Invitation", $"{invitingUser.Name} invited you to {gathering.Title}.", $"gathering/{gathering.Id}?invitedBy={invitingUser.Name}", $"{gathering.Id}:1"));
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
            => CompanionActivity(new(gathering.Title, $"{companion.Name} joined the gathering", $"gathering/{gathering.Id}?focus=guestlist", $"{gathering.Id}:10"));

        public static CanaryNotification CompanionGatheringCreated(UserShard companion, GatheringShard gathering)
            => CompanionActivity(new("Companion Gathering", $"{companion.Name} just created {gathering.Title}", $"gathering/{gathering.Id}"));
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

        public static CanaryNotification NearbyGatherings()
            => GatheringDiscovery(new("New Gatherings Nearby", "There are new gatherings in your area that you may be interested in.", "discovery"));
        // TODO A. Need to actually ensure that they are new (gathering creation time vs last logged in) B. not send multiple
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

        public static CanaryNotification UserMissedGathering(GatheringShard gathering)
            => GatheringActivity(new("", "You have arrived?", $"gathering/{gathering.Id}", "30"));

        // Host

        public static CanaryNotification HostArrived()
            => GatheringActivity(new("", "You have arrived?", ""));

        public static CanaryNotification GatheringSealed(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"was reported too many times and was sealed as a result.", $"gathering/{gathering.Id}?sealed=true"));

        public static CanaryNotification GatheringHeartbeat(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Is the gathering still ongoing?", $"gathering/{gathering.Id}?immediate=true"));

        public static CanaryNotification HostLeavingGatheringArea(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"You are leaving the gathering area, gathering will hide itself.", $"gathering/{gathering.Id}"));

        public static CanaryNotification GatheringWaiting(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Your gathering is waiting to start!", $"gathering/{gathering.Id}?immediate=true", "30"));

        public static CanaryNotification GatheringRemovalWarning(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Is going to be deleted if you do not start it.", $"gathering/{gathering.Id}?immediate=true", "30"));

        public static CanaryNotification GatheringDeleted(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Was cancelled due to lack of host.", $"gathering/{gathering.Id}", "30"));

        // Attendee

        public static CanaryNotification AttendeeArrived(GatheringShard gathering)
            => GatheringActivity(new(gathering.Title, "You have entered the gathering area, get on now!", $"gathering/{gathering.Id}?immediate=true", "30"));

        public static CanaryNotification AttendeeLeavingGatheringArea(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Is starting later.", $"gathering/{gathering.Id}", "30"));

        public static CanaryNotification GatheringTerminated(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Has ended. Thanks for joining!", $"gathering/{gathering.Id}", "30"));

        public static CanaryNotification HostMissedGathering(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Was cancelled due to an absent host.", $"gathering/{gathering.Id}", "20"));
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

        public static CanaryNotification GatheringUpcoming(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Is starting later.", $"gathering/{gathering.Id}", "20"));

        public static CanaryNotification GatheringImminent(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Is starting shortly.", $"gathering/{gathering.Id}?immediate=true", "20"));

        public static CanaryNotification GatheringLive(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Is now live!", $"gathering/{gathering.Id}?immediate=true", "20"));

        public static CanaryNotification GatheringCancelled(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Was cancelled by the host.", $"gathering/{gathering.Id}", "20"));

        public static CanaryNotification GatheringEdited(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Was modified by the host.", $"gathering/{gathering.Id}", "21"));

        public static CanaryNotification GatheringUploadClosing(GatheringShard gathering)
            => GatheringReminders(new(gathering.Title, $"Post your remaining photos before the upload window closes.", $"gathering/{gathering.Id}?focus=gallery"));
    }
}
