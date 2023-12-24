using System.Collections.Generic;
using System.Linq;
using LooperPooper.Drums.Input.Analysis.Comparison;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    /// <summary>
    /// Quantize DrumsAnalyzerBeatSource items time and duration.
    /// </summary>
    public class ProcessBeatSourcesQuantize : IProcess
    {
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        private readonly int _barSize;
        private readonly int _barsCount;
        
        public ProcessBeatSourcesQuantize(List<DrumsAnalyzerBeatSource> beatSources, int barSize, int barsCount)
        {
            _beatSources = beatSources;
            _barSize = barSize;
            _barsCount = barsCount;
        }
        
        public void Process()
        {
            var initialDuration = _beatSources.Sum(beatSource => beatSource.Duration);

            var smallestAmount = _barSize * _barsCount;
            var smallestDuration = initialDuration / smallestAmount;
            
            var time = 0f;

            foreach (var beatSource in _beatSources)
            {
                beatSource.Size = Mathf.RoundToInt(beatSource.Duration / smallestDuration);
                beatSource.Duration = beatSource.Size * smallestDuration;
                beatSource.Time = time;
                
                time += beatSource.Duration;
            }
        }
    }
}