using System.Collections.Generic;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsSpread : IProcess
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        private readonly float _barDuration;
        
        public ProcessBarsSpread(List<DrumsAnalyzerBar> bars, float barDuration)
        {
            _bars = bars;
            _barDuration = barDuration;
        }
        
        public void Process()
        {
            // Move beats to next bar until bar duration longer desired duration
            for (var i = 0; i < _bars.Count - 1; i++)
            {
                var barA = _bars[i];
                var barB = _bars[i + 1];
            
                var operation = new DrumsAnalyzerBarOperation(barA, barB, null);
            
                while (barA.Beats[^1].Time + barA.Beats[^1].Duration * 0.25f > _barDuration)
                {
                    operation.TryDo(DrumsAnalyzerBarOperationType.AtoB);
                }
            }
        }
    }
}