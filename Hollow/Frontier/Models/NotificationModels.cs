using Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace Frontier.Models
{
	public class NotificationSubscriptionModel
	{
		[Required]
		public DeviceType DeviceType { get; set; }

		[Required]
		public string DeviceToken { get; set; }
	}
}
