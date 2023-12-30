using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessCalculateTimeSignature : IProcess<int>
    {
        private readonly DrumsInput _drumsInput;
        
        public ProcessCalculateTimeSignature(DrumsInput drumsInput)
        {
            _drumsInput = drumsInput;
        }
        
        public int Process()
        {
            return GetTimeSignatureFromBeats();
            return GetTimeSignatureFromGroups();
            
            var timeSignature = GetTimeSignatureFromGroups();
            
            var barsCount34 = new ProcessCalculateBarsCount(_drumsInput, 3).Process();
            var barsCount44 = new ProcessCalculateBarsCount(_drumsInput, 4).Process();
            
            var validation34 = new ProcessValidateRepeats(_drumsInput, barsCount34).Process();
            var validation44 = new ProcessValidateRepeats(_drumsInput, barsCount44).Process();
            
            if (timeSignature == 3 && validation34 >= validation44)
            {
                return 3;
            }

            if (timeSignature == 3)
            {
                Debug.Log("Time signature invalid");
            }

            return 4;
        }

        private int GetTimeSignatureFromBeats()
        {
            var is34 = 0f;
            var is44 = 0f;
            
            for (var i = 1; i < Mathf.CeilToInt((float)_drumsInput.Entries.Count / 2f); i++)
            {
                var r = i == 1 ? 1 : new ProcessValidateRepeats(_drumsInput, i).Process();
                var barDuration = _drumsInput.Duration / i;

                foreach (var entry in _drumsInput.Entries)
                {
                    var a = GetTimeSignatureForValue(barDuration / entry.Duration);

                    if (a == 3)
                    {
                        is34 += 1 * r;
                    }
                    else
                    {
                        is44 += 1 * r;
                    }
                }
            }
//            Debug.LogError("is34: " + is34 + " / is44: " + is44);
            return is34 > is44 ? 3 : 4;
        }
        
        private int GetTimeSignatureFromGroups()
        {
            var groups = _drumsInput.GetGroupedEntries(0.75f);
            
            var is34 = 0f;
            var is44 = 0f;
            
            foreach (var group in groups)
            {
                var a = GetTimeSignatureForValue(_drumsInput.Duration / group.Average());

                if (a == 3)
                {
                    is34 += group.Count;
                }
                else
                {
                    is44 += group.Count;
                }
            }
            //Debug.LogError("is34: " + is34 + " / is44: " + is44);
            return is34 > is44 ? 3 : 4;
        }
        
        private static int GetTimeSignatureForValue(float value)
        {
            while (value > 10)
            {
                value /= 2;
            }
            
            var distance3 = GetSmallestDistanceToOneOfPoints(value, 
                new float[] {3, 6, 9, 12, 24, 48, 96, 192}, out var closestPoint3);

            var distance4 = GetSmallestDistanceToOneOfPoints(value, 
                new float[] {2, 4, 8, 16, 32, 64, 128, 256}, out var closestPoint4);

//            Debug.LogError(value + " >> closestPoint3: " + closestPoint3 + "; distance3: " + distance3);
//            Debug.LogError(value + " >> closestPoint4: " + closestPoint4 + "; distance4: " + distance4);

            
            return distance3 < distance4 ? 3 : 4;
        }
        
        private static float GetSmallestDistanceToOneOfPoints(float value, float[] points, out float closestPoint)
        {
            var output = float.MaxValue;
            
            closestPoint = value;
            
            foreach (var point in points)
            {
                var distance = Mathf.Abs(point - value);

                if (distance > output)
                {
                    continue;
                }

                closestPoint = point;
                output = distance;
            }

            return output;
        }
    }
}