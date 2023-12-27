using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LooperPooper.Drums.Playback;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessGenerateOutput : IProcess
    {
        public DrumsLoop OutputDrumsLoop { get; private set; }
        
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        private readonly float _timeSignature;
        private readonly int _barsCount;
            
        public ProcessGenerateOutput(List<DrumsAnalyzerBeatSource> beatSources, float timeSignature, int barsCount)
        {
            _beatSources = beatSources;
            _timeSignature = timeSignature;
            _barsCount = barsCount;
        }

        public void Process()
        {
            var outputBars = new List<DrumsLoopBar>();
            
            for (var i = 0; i < _barsCount; i++)
            {
                var outputBeatsKick = new List<DrumsLoopBeat>();
                var outputBeatsSnare = new List<DrumsLoopBeat>();

                foreach (var beatSource in _beatSources.Where(beatSource => beatSource.Bar == i))
                {
                    outputBeatsKick.Add(new DrumsLoopBeat(beatSource.Type == DrumsBeatType.Kick ? DrumsBeatType.Kick : DrumsBeatType.None));
                    outputBeatsSnare.Add(new DrumsLoopBeat(beatSource.Type == DrumsBeatType.Snare ? DrumsBeatType.Snare : DrumsBeatType.None));

                    for (var j = 1; j < beatSource.Size; j++)
                    {
                        outputBeatsKick.Add(new DrumsLoopBeat(DrumsBeatType.None));
                        outputBeatsSnare.Add(new DrumsLoopBeat(DrumsBeatType.None));
                    }
                }

                outputBars.Add(new DrumsLoopBar(outputBeatsKick.ToArray(), outputBeatsSnare.ToArray()));
            }
            
            var barDuration = _beatSources.Sum(beatSource => beatSource.Duration) / _barsCount;
            var bpm = _timeSignature / barDuration * 60;//Mathf.Round(_timeSignature / barDuration * 60);

            var drumsPatterns = new DrumsPatterns();
            foreach (var bar in outputBars)
            {
                var a = drumsPatterns.A(bar);

                for (var i = 0; i < bar.Length; i++)
                {
                    var aa = a.Substring(i, 1);
                    
                    if (aa == "K")
                    {
                        Debug.Log("K");
                        bar.BeatsKick[i] = new DrumsLoopBeat(DrumsBeatType.Kick);
                        bar.BeatsSnare[i] = new DrumsLoopBeat(DrumsBeatType.None);
                    }
                    
                    else if (aa == "S")
                    {
                        bar.BeatsKick[i] = new DrumsLoopBeat(DrumsBeatType.None);
                        bar.BeatsSnare[i] = new DrumsLoopBeat(DrumsBeatType.Snare);
                    }
                    else
                    {
                        bar.BeatsKick[i] = new DrumsLoopBeat(DrumsBeatType.None);
                        bar.BeatsSnare[i] = new DrumsLoopBeat(DrumsBeatType.None);
                    }
                }
            }
            
            OutputDrumsLoop = new DrumsLoop(outputBars.ToArray(), bpm);
        }
    }
}