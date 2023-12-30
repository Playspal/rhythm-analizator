using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input
{
    public static class DrumsInputCollisionExtensions
    {
        public static int Collide(this DrumsInput drumsInput)
        {
            var groups = new List<List<float>>();

            foreach (var entry in drumsInput.Entries)
            {
                if (entry.Duration < 0.05f)
                {
                    continue;
                }
                
                var added = false;
                
                foreach (var group in groups)
                {
                    var average = group.Average();
                    var delta = Mathf.Abs(average - entry.Duration);

                    if (delta > average * 0.25f)
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

            var is34 = 0f;
            var is44 = 0f;
            
            foreach (var group in groups)
            {
                var a = GetTimeSignatureForValue(drumsInput.Duration / group.Average());

                if (a == 3)
                {
                    is34 += group.Count;
                }
                else
                {
                    is44 += group.Count;
                }
            }

            return is34 > is44 ? 3 : 4;
        }

        private static int GetTimeSignatureForValue(float value)
        {
            var distance3 = GetSmallestDistanceToOneOfPoints(value, 
                new float[] {3, 6, 9, 12, 24, 48, 96, 192});

            var distance4 = GetSmallestDistanceToOneOfPoints(value, 
                new float[] {2, 4, 8, 16, 32, 64, 128, 256});
            
            return distance3 < distance4 ? 3 : 4;
        }
        
        private static float GetSmallestDistanceToOneOfPoints(float value, float[] points)
        {
            var output = float.MaxValue;

            foreach (var point in points)
            {
                var distance = Mathf.Abs(point - value);

                if (distance < output)
                {
                    continue;
                }
                
                output = distance;
            }

            return output;
        }
    }
}