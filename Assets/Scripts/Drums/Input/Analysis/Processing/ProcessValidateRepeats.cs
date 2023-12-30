using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessValidateRepeats : IProcess<float>
    {
        private readonly DrumsInput _drumsInput;
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        private readonly int _repeats;
        
        public ProcessValidateRepeats(DrumsInput drumsInput, int repeats)
        {
            _drumsInput = drumsInput;
            _beatSources = new ProcessBeatSourcesCreate(_drumsInput).Process();
            _repeats = repeats;
        }
        
        public float Process()
        {
            NormalizeBeatSources();
            
            var initialDuration = _beatSources.Sum(b => b.Duration);
            var repeatDuration = initialDuration / _repeats;
            
            var output = new List<List<DrumsAnalyzerBeatSource>>();

            for (var i = 0; i < _repeats; i++)
            {
                output.Add(new List<DrumsAnalyzerBeatSource>());
            }

            foreach (var beatSource in _beatSources)
            {
                for (var j = 0; j < _repeats; j++)
                {
                    var boundMin = repeatDuration * j;
                    var boundMax = repeatDuration * j + repeatDuration;

                    if (beatSource.Time + beatSource.Duration < boundMin || beatSource.Time > boundMax)
                    {
                        continue;
                    }
                    
                    output[j].Add(beatSource);
                }
            }

            var result = 0f;
            var resultsCount = 0;
            
            for (var i = 0; i < output.Count; i++)
            {
                for (var j = 0; j < output.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var a = output[i];
                    var b = output[j];

                    var length = Mathf.Min(a.Count, b.Count);
                    var equalsA = 0;
                    
                    for (var n = 0; n < length; n++)
                    {
                        if (a[n].Duration == b[n].Duration)
                        {
                            equalsA++;
                        }
                    }

                    result += (float) equalsA / (float) length;
                    resultsCount++;
                }
            }

            return result / resultsCount;
        }

        private void NormalizeBeatSources()
        {
            var groups = _drumsInput.GetGroupedEntries(0.25f);

            foreach (var beatSource in _beatSources)
            {
                foreach (var group in groups)
                {
                    if (!group.Contains(beatSource.Duration))
                    {
                        continue;
                    }

                    beatSource.Duration = group.Average();
                    break;
                }
            }

            var time = 0f;

            foreach (var beatSource in _beatSources)
            {
                beatSource.Time = time;
                time += beatSource.Duration;
            }
        }
    }
}