using System.Collections.Generic;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsCreate : IProcess
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        private readonly int _barsCount;
        
        public ProcessBarsCreate(List<DrumsAnalyzerBar> bars, int barsCount)
        {
            _bars = bars;
            _barsCount = barsCount;
        }
        
        public void Process()
        {
            for (var i = _bars.Count; i < _barsCount; i++)
            {
                _bars.Add(new DrumsAnalyzerBar());
            }
        }
    }
}