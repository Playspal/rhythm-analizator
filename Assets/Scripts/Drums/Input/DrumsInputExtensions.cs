using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input
{
    public static class DrumsInputExtensions
    {
        public static List<List<float>> GetGroupedEntries(this DrumsInput drumsInput, float treshold)
        {
            var groups = new List<List<float>>();

            foreach (var entry in drumsInput.Entries)
            {
                var added = false;
                
                if (entry.Duration < 0.05f)
                {
                    continue;
                }

                foreach (var group in groups)
                {
                    var average = group.Average();
                    var delta = Mathf.Abs(average - entry.Duration);

                    if (delta > average * treshold)
                    {
                        continue;
                    }
                    
                    group.Add(entry.Duration);
                    added = true;
                    break;
                }

                if (added)
                {
                    continue;
                }

                groups.Add(new List<float>() {entry.Duration});
            }
            
            groups.Sort((a, b) => a.Average().CompareTo(b.Average()));

            return groups;
        }
    }
}