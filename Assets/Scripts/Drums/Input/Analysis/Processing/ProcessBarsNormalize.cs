using System.Collections.Generic;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsNormalize : IProcess
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        private readonly float _barDuration;
        
        public ProcessBarsNormalize(List<DrumsAnalyzerBar> bars, float barDuration)
        {
            _bars = bars;
            _barDuration = barDuration;
        }
        
        public void Process()
        {
            var time = 0f;

            foreach (var bar in _bars)
            {
                var barDurationDelta = _barDuration - bar.Duration;
                var beatDurationDelta = barDurationDelta / bar.Beats.Count;

                foreach (var beat in bar.Beats)
                {
                    beat.Source.Time = time;
                    beat.Source.Duration += beatDurationDelta;

                    time += beat.Source.Duration;
                }
            }
        }
    }
}