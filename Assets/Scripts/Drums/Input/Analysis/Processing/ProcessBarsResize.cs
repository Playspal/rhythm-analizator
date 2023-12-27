using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsResize : IProcess
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        private readonly int _timeSignature;
        private readonly float _barDuration;
        private readonly float _beatDuration;
        private readonly float _barSize;
        
        public ProcessBarsResize(List<DrumsAnalyzerBar> bars, int timeSignature, float barDuration, float beatDuration, int barSize)
        {
            _bars = bars;
            _timeSignature = timeSignature;
            _barDuration = barDuration;
            _beatDuration = beatDuration;
            _barSize = barSize;
        }

        public void Process()
        {
            var time = 0f;

            for (var i = 0; i < _bars.Count - 1; i++)
            {
                var delta = _barDuration - _bars[i].Duration;

                _bars[i].Beats[^1].Source.Duration += delta;
                _bars[i + 1].Beats[0].Source.Duration -= delta;
            }

            foreach (var bar in _bars)
            {
                foreach (var beat in bar.Beats)
                {
                    // Round at this point is the key reason of potential errors in pattern parsing
                    beat.Source.Size = Mathf.RoundToInt(beat.Source.Duration / _beatDuration);
                    beat.Source.Duration = _beatDuration * beat.Source.Size;
                    beat.Source.Time = time;

                    time += beat.Source.Duration;
                }
                
                // Hack: increase last beat duration to make bar duration size as desired
                while (bar.Size < _barSize)
                {
                    bar.Beats[^1].Source.Size++;
                    bar.Beats[^1].Source.Duration += _beatDuration;
                    
                    time += _beatDuration;
                }

                // Hack: decrease longest beat duration to make bar duration size as desired
                while (bar.Size > _barSize)
                {
                    var beatBiggestSize = bar.Beats.ToList().Max(x => x.Source.Size);
                    var beatFound = false;

                    if (beatBiggestSize <= 1)
                    {
                        break;
                    }
                    
                    foreach (var beat in bar.Beats)
                    {
                        if (beat.Source.Size == beatBiggestSize)
                        {
                            beat.Source.Size--;
                            beat.Source.Duration -= _beatDuration;

                            time -= _beatDuration;
                            
                            beatFound = true;
                            continue;
                        }

                        if (beatFound)
                        {
                            beat.Source.Time -= _beatDuration;
                        }
                    }
                }
            }
        }
    }
}