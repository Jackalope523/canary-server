using System;

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Core.Boundaries;

namespace Frontier.Manifests
{
    ////////
    // Incoming Manifests
    ///////////////////////

    public class SeedManifest
    {
        public List<AccountSignUpManifest> Users { get; set; }

        public List<EventDetailsManifest> Events { get; set; }

        public List<List<int>> Attendance { get; set; }
        public List<List<int>> Follows { get; set; }
        public List<List<int>> Blocks { get; set; }
    }
}

