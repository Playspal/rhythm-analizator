using System.Collections.Generic;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsFill : IProcess
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        private readonly DrumsInput _drumsInput;
        
        public ProcessBarsFill(List<DrumsAnalyzerBar> bars, DrumsInput input)
        {
            _bars = bars;
            _drumsInput = input;
        }
        
        public void Process()
        {
            // Fill first bar with recording data
            _drumsInput.Entries.ForEach(entry =>
            {
                var beat = new DrumsAnalyzerBeat
                {
                    Bar = _bars[0],
                    Source = new DrumsAnalyzerBeatSource
                    {
                        Type = entry.Type,
                        Time = entry.Time,
                        Duration = entry.Duration
                    }
                };
            
                _bars[0].AddToEnd(beat);
            });
        }
    }
}