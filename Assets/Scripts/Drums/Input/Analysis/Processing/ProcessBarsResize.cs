using System.Collections.Generic;
using System.Linq;
using LooperPooper.Drums.Input.Analysis.Comparison;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsResize : IProcess
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        private readonly float _barDuration;
        private readonly float _barSize;
        
        public ProcessBarsResize(List<DrumsAnalyzerBar> bars, float barDuration, int barSize)
        {
            _bars = bars;
            _barDuration = barDuration;
            _barSize = barSize;
        }

        public void Process()
        {
            var smallestDuration = _barDuration / _barSize;
            var time = 0f;
        
            foreach (var bar in _bars)
            {
                //smallestDuration = bar.Duration / _barSize;
                
                foreach (var beat in bar.Beats)
                {
                    beat.Source.Size = Mathf.Max(1, Mathf.RoundToInt(beat.Source.Duration / smallestDuration));
                    beat.Source.Duration = smallestDuration * beat.Source.Size;
                    beat.Source.Time = time;

                    time += beat.Source.Duration;
                }

                while (bar.Size < _barSize)
                {
                    bar.Beats[^1].Source.Size++;
                    bar.Beats[^1].Source.Duration += smallestDuration;
                    
                    time += smallestDuration;
                }

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
                            beat.Source.Duration -= smallestDuration;

                            time -= smallestDuration;
                            
                            beatFound = true;
                            continue;
                        }

                        if (beatFound)
                        {
                            beat.Source.Time -= smallestDuration;
                        }
                    }
                }
            }
        }
    }
}