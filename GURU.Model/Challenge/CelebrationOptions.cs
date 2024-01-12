using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GURU.Model.Challenge
{
    public class CelebrationOptions
    {
        public PartyLocation PartyLocation { get; set; }
        public List<RainChancesOnWeekday> BestDaysForLocation { get; set; }
    }
}
