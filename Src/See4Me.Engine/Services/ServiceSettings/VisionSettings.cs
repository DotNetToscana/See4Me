using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Engine.Services.ServiceSettings
{
    public class VisionSettings
    {
        /* Somes strings are actually invalid or need to be temporaly removed
         * because very often they aren't so accurate
         * (they will be probably added again in a future release).
         */

        public IEnumerable<string> InvalidDescriptions { get; set; }

        public IEnumerable<string> DescriptionsToRemove { get; set; }

        public Dictionary<string, string> DescriptionsToReplace { get; set; }

        // The minimum confidence to consider a description valid.
        public double MinimumConfidence { get; set; } = 0.16;
    }
}
