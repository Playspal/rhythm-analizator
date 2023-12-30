using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessCalculateRepeats : IProcess<bool>
    {
        private readonly DrumsInput _drumsInput;
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        
        public ProcessCalculateRepeats(DrumsInput drumsInput)
        {
            _drumsInput = drumsInput;
            _beatSources = new ProcessBeatSourcesCreate(_drumsInput).Process();
        }
        
        public bool Process()
        {
            NormalizeBeatSources();

            var results = new Dictionary<int, float>();
            
            for (var i = 2; i < 24; i++)
            {
                results.Add(i, Find(i));
            }

            var bestResult = float.MinValue;
            var bestResultIndex = 0;

            foreach (var (index, result) in results)
            {
                if (result > bestResult)
                {
                    bestResult = result;
                    bestResultIndex = index;
                }
            }
            
            Debug.LogError("Found: " + bestResultIndex + " repeats; " + bestResult);
            return true;
        }

        private float Find(float division)
        {
            var initialDuration = _beatSources.Sum(b => b.Duration);
            var frameDuration = initialDuration / division;
            
            var d = _beatSources.Sum(b => b.Duration) / division;

            var output = new List<List<DrumsAnalyzerBeatSource>>();

            for (var i = 0; i < division; i++)
            {
                output.Add(new List<DrumsAnalyzerBeatSource>());
            }

            for (var i = 0; i < _beatSources.Count; i++)
            {
                var beatSource = _beatSources[i];

                for (var j = 0; j < division; j++)
                {
                    var boundMin = frameDuration * j;
                    var boundMax = frameDuration * j + frameDuration;

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

                    var ratio = (float) equalsA / (float) length;
                    result += ratio;
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