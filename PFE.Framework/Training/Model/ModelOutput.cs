using System;
using System.Collections.Generic;
using System.Text;

namespace PFE.Framework.Training.Model
{
    public class ModelOutput
    {
        public int[] ForecastedConnection { get; set; }

        public int[] LowerBoundConnection { get; set; }

        public int[] UpperBoundConnection { get; set; }
    }
}
