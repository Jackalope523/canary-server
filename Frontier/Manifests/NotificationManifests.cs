using Core.Boundaries;

using System;
using System.ComponentModel.DataAnnotations;

namespace Frontier.Manifests
{
	////////
	// Incoming Manifests
	///////////////////////

	public class NotificationSubscriptionManifest
	{
		[Required]
		public DeviceType DeviceType { get; set; }

		[Required]
		public string DeviceToken { get; set; }
    }

    ////////
    // Outgoing Manifests
    ///////////////////////

	public class NoteManifest : Manifest
	{
		public ulong NotifierId { get; }
		public DateTimeOffset Time { get; }
		public string Message { get; }
		public string Action { get; }

		public NoteManifest(NoteShard note)
		{
			NotifierId = note.NotifierId;
			Time = note.Time;
			Message = note.Message;
			Action = note.Action;
		}
	}
}
