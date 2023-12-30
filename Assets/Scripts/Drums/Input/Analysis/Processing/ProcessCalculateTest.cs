using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessCalculateTest : IProcess<int>
    {
        private readonly DrumsInput _drumsInput;
        
        public ProcessCalculateTest(DrumsInput drumsInput)
        {
            _drumsInput = drumsInput;
        }
        
        public int Process()
        {
            AAA();
            return 0;
            var accuracy = 0.025f;

            var result3 = 0f;
            var result4 = 0f;
            
            for (var i = 1; i < 4; i++)
            {
                result3 += GetMatch(3 * i, accuracy);
                result4 += GetMatch(4 * i, accuracy);
            }

            Debug.LogError("result3: " + result3 + "; result4: " + result4);
            
            return 0;
        }

        private void AAA()
        {
            var groups = _drumsInput.GetGroupedEntries(0.1f);

            if (groups.Count <= 0)
            {
                return;
            }

            var groupMin = _drumsInput.Entries.Min(e => e.Duration);//groups[0].Average());

            var totalSize = 0f;

            var d = string.Empty;
            
            foreach (var entry in _drumsInput.Entries)
            {
                var duration = entry.Duration;
                
                /*
                foreach (var group in groups)
                {
                    if (group.Contains(duration))
                    {
                        duration = group.Average();
                        break;
                    }
                }
*/
                var ratio = duration / groupMin;
                var error = ratio - Mathf.Floor(ratio);

                var size = error < 0.75f ? Mathf.Floor(ratio) : Mathf.Ceil(ratio);

                d += "X ";

                for (var i = 1; i < size; i++)
                {
                    d += "x ";
                }
                
                Debug.LogError("! " + ratio + " > " + size);

                totalSize += size;
            }
            
            Debug.LogError(d);
            Debug.LogError("! totalSize " + totalSize);
        }

        private float GetMatch(int pulsesCount, float accuracy)
        {
            var pulsesConfirmed = 0;
            var pulseDuration = _drumsInput.Duration / pulsesCount;
            
            for (var i = 0; i < pulsesCount; i++)
            {
                var pulseTime = pulseDuration * i;
                var pulseOffset = pulseDuration * accuracy;
                
                foreach (var entry in _drumsInput.Entries)
                {
                    if (entry.Time < pulseTime - pulseOffset || entry.Time >= pulseTime + pulseOffset)
                    {
                        continue;
                    }

                    pulsesConfirmed++;
                    break;
                }
            }

            var result = (float) pulsesConfirmed / (float) pulsesCount;

            result *= 1f - accuracy;
            
            //Debug.LogError(">>>>>>>> " + pulsesConfirmed + " / " + pulsesCount);
            
            //var beatSources = new ProcessBeatSourcesCreate(_drumsInput).Process();

            return result;
        }
    }
}