using System.Collections.Generic;
using System.Linq;
using LooperPooper.Drums.Playback;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessCalculateError : IProcess<bool>
    {
        public float Error { get; private set; }

        private readonly DrumsLoop _drumsLoop;
        private readonly DrumsInput _drumsInput;
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        private readonly int _timeSignature;
        private readonly int _barSize;
        private readonly int _barsCount;
        private readonly float _referenceBPM;
        
        public ProcessCalculateError(DrumsLoop drumsLoop, DrumsInput drumsInput, List<DrumsAnalyzerBeatSource> beatSources, 
            int timeSignature, int barSize, int barsCount, float referenceBPM)
        {
            _drumsLoop = drumsLoop;
            _drumsInput = drumsInput;
            _beatSources = beatSources;
            _timeSignature = timeSignature;
            _barSize = barSize;
            _barsCount = barsCount;
            _referenceBPM = referenceBPM;
        }

        public bool Process()
        {
            var beatDuration = 60f / _drumsLoop.BeatsPerMinute / (_drumsLoop.BeatsPerBar / 4);
            
            var time = 0f;
            var index = 0;

            var kickError = 0f;
            var snareError = 0f;
            
            for (var i = 0; i < _drumsLoop.Bars.Length; i++)
            {
                for (var j = 0; j < _drumsLoop.Bars[i].BeatsKick.Length; j++)
                {
                    if (_drumsLoop.Bars[i].BeatsKick[j].Type == DrumsBeatType.Kick)
                    {
                        kickError += Mathf.Abs(_drumsInput.Entries[index].Time - time);
                        
                        index++;
                    }
                    
                    else if (_drumsLoop.Bars[i].BeatsSnare[j].Type == DrumsBeatType.Snare)
                    {
                        snareError += Mathf.Abs(_drumsInput.Entries[index].Time - time);
                        
                        index++;
                    }
                    
                    time += beatDuration;
                }
            }
            
            var bpmError = Mathf.Abs(_referenceBPM - _drumsLoop.BeatsPerMinute);

            Error = kickError + snareError + bpmError * 1f;

            Debug.Log(_barSize + " / " + _barsCount + " >> " + kickError + " + " + snareError + " = " + (kickError + snareError) + "; bpmError = " + bpmError + "; Total error = " + Error);
            
            return true;
        }

        public void Process2()
        {
            Error = Mathf.Abs(_drumsInput.Entries.Sum(entry => entry.Duration) -
                              _beatSources.Sum(beatSource => beatSource.Duration));

            for (var i = 0; i < _drumsInput.Entries.Count; i++)
            {
                Error += Mathf.Abs(_drumsInput.Entries[i].Time - _beatSources[i].Time);
            }

            Error -= (float) (_barSize * _barsCount) * 0.0001f;

            var barDuration = _beatSources.Sum(beatSource => beatSource.Duration) / _barsCount;
            var bpm = Mathf.Round(_timeSignature / barDuration * 60);
            var bpmError = Mathf.Abs(120 - bpm);

            //Error += bpmError * 0.01f;
            
            var sizeTotal = _beatSources.Sum(beatSource => beatSource.Size);
            
            Debug.Log(_barSize + " / " + _barsCount + " >> " + Error + " / " + sizeTotal + " / " + bpm);
        }
    }
}