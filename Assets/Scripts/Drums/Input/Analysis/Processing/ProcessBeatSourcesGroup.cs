using System.Collections.Generic;
using System.Linq;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBeatSourcesGroup : IProcess
    {
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        private readonly int _timeSignature;
        private readonly int _barSize;
        private readonly int _barsCount;

        public ProcessBeatSourcesGroup(List<DrumsAnalyzerBeatSource> beatSources, 
            int timeSignature, int barSize, int barsCount)
        {
            _beatSources = beatSources;
            _timeSignature = timeSignature;
            _barSize = barSize;
            _barsCount = barsCount;
        }
        
        public void Process()
        {
            var initialDuration = _beatSources.Sum(beatSource => beatSource.Duration);
            
            var beatsCount = _beatSources.Count;
            var beatDuration = initialDuration / (_barSize * _barsCount);
            var barDuration = initialDuration / _barsCount;
            var bars = new List<DrumsAnalyzerBar>();

            new ProcessBarsCreate(bars, _barsCount).Process();
            new ProcessBarsFill(bars, _beatSources).Process();
            new ProcessBarsSpread(bars, barDuration).Process();
            new ProcessBarsBalance(bars, barDuration, beatsCount).Process();
            new ProcessBarsResize(bars, _timeSignature, barDuration, beatDuration, _barSize).Process();
            new ProcessBarsNormalize(bars, barDuration).Process();
            //new ProcessBarsDebug(bars, "T").Process();
            new ProcessBarsDebugPattern(bars, _timeSignature, "T").Process();

            for (var i = 0; i < bars.Count; i++)
            {
                bars[i].Beats.ForEach(beat => beat.Source.Bar = i);
            }
            
            var time = 0f;
            
            foreach (var beatSource in _beatSources)
            {
                beatSource.Time = time;
                beatSource.Duration = beatSource.Size * beatDuration;

                time += beatSource.Duration;
            }
        }
    }
}