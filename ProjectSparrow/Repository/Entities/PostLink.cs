using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class PostLink : Link
    {
        public enum PostLinkType { RateUp, RateDown }

        public Guid PostId { get; init; }
        internal Post Post { get; init; }
        public PostLinkType Type { get; set; }
    }
}
