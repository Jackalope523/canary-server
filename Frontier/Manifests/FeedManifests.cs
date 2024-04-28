
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frontier.Manifests
{
	////////
	// Outgoing Manifests
	///////////////////////

	public class FeedManifest : Manifest
	{
		public List<EventHeaderManifest> Headers { get; set; }
		public List<EtchingManifest> Etchings { get; set; }
	}
}
