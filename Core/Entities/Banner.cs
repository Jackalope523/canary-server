using System;
using System.Collections.Generic;
using Core.Boundaries;

namespace Core.Entities
{
    using static CoreTerminal;

	internal class Banner
	{
        #region Variables

        ///////
        // Properties
        ///////////////

        public ulong Id { get; init; }
        public string Designation { get; set; }
        public string Colour { get; set; }
        public string Code { get; set; }

        ////////
        // Synced Properties
        //////////////////////

        public Synced<List<User>> Members { get; }

        #endregion

        #region Initialisation & Extraction

        public Banner()
		{
            Members = new(() => Terminal.BannerDirector.RequestBannerMembersAsync(this));
        }

        public Banner(CoreBanner fromBanner) : this()
        {
            Id = fromBanner.Id;
            Designation = fromBanner.Banner;
            Colour = fromBanner.Colour;
            Code = fromBanner.Code;
        }

        public Banner(BannerShard fromBanner) : this()
        {
            Id = fromBanner.Id;
            Designation = fromBanner.Banner;
            Colour = fromBanner.Colour;
        }

        public CoreBanner ToCoreBanner()
        {
            return new(Id, Designation, Colour, Code);
        }

        public BannerShard ToBannerShard()
        {
            return new(Id, Designation, Colour);
        }

        #endregion

        #region Dissimilation

        public override bool Equals(object obj)
        {
            return obj is Banner other && Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }
}
