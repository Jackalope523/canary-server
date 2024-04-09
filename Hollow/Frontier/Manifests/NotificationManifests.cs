using Shared;
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
}
