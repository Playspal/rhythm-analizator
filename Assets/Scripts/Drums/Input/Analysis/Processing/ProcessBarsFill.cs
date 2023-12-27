using System.Collections.Generic;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsFill : IProcess
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        
        public ProcessBarsFill(List<DrumsAnalyzerBar> bars, List<DrumsAnalyzerBeatSource> beatSources)
        {
            _bars = bars;
            _beatSources = beatSources;
        }
        
        public void Process()
        {
            _beatSources.ForEach(beatSource =>
            {
                var beat = new DrumsAnalyzerBeat
                {
                    Bar = _bars[0],
                    Source = beatSource
                };
            
                _bars[0].AddToEnd(beat);
            });
        }
    }
}