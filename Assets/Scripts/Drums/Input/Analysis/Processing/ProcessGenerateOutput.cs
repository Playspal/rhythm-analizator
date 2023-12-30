using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LooperPooper.Drums.Playback;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessGenerateOutput : IProcess<bool>
    {
        public DrumsLoop OutputDrumsLoop { get; private set; }
        
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        private readonly float _bpm;
        private readonly float _timeSignature;
        private readonly int _barSize;
        private readonly int _barsCount;
            
        public ProcessGenerateOutput(List<DrumsAnalyzerBeatSource> beatSources, float bpm, float timeSignature, int barSize, int barsCount)
        {
            _beatSources = beatSources;
            _bpm = bpm;
            _timeSignature = timeSignature;
            _barSize = barSize;
            _barsCount = barsCount;
        }

        public bool Process()
        {
            var outputBars = new List<DrumsLoopBar>();
            
            var slots = CreateSlots();
            
            for (var i = 0; i < _barsCount; i++)
            {
                var outputBeatsKick = new List<DrumsLoopBeat>();
                var outputBeatsSnare = new List<DrumsLoopBeat>();
                
                for (var j = 0; j < _barSize; j++)
                {
                    var beatSource = slots[_barSize * i + j];

                    outputBeatsKick.Add(new DrumsLoopBeat(beatSource != null && beatSource.Type == DrumsBeatType.Kick ? DrumsBeatType.Kick : DrumsBeatType.None));
                    outputBeatsSnare.Add(new DrumsLoopBeat(beatSource != null && beatSource.Type == DrumsBeatType.Snare ? DrumsBeatType.Snare : DrumsBeatType.None));
                }
                
                outputBars.Add(new DrumsLoopBar(outputBeatsKick.ToArray(), outputBeatsSnare.ToArray()));
            }
            
            OutputDrumsLoop = new DrumsLoop(outputBars.ToArray(), _bpm);
            
            return true;
        }

        public void Process2()
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
        
        private DrumsAnalyzerBeatSource[] CreateSlots()
        {
            var initialDuration = _beatSources.Sum(beatSource => beatSource.Duration);
            var barDuration = initialDuration / _barsCount;
            var beatDuration = barDuration / _barSize;
            var beatsCount = _barSize * _barsCount;
            
            var slots = new DrumsAnalyzerBeatSource[beatsCount];
            var filter = new List<DrumsAnalyzerBeatSource>();
            
            for (var i = 0; i < beatsCount; i++)
            {
                if (slots[i] != null)
                {
                    continue;
                }
                
                var boundsOffset = beatDuration / 2;
                var boundsCenter = i * beatDuration;
                var boundsMin = boundsCenter - boundsOffset;
                var boundsMax = boundsCenter + boundsOffset;

                foreach (var beatSource in _beatSources)
                {
                    if (beatSource.Time < boundsMin || beatSource.Time >= boundsMax || filter.Contains(beatSource))
                    {
                        continue;
                    }

                    for (var j = i; j < beatsCount; j++)
                    {
                        if (slots[j] != null)
                        {
                            continue;
                        }
                        
                        slots[j] = beatSource;
                        filter.Add(beatSource);
                            
                        break;
                    }
                }
            }

            return slots;
        }
    }
}