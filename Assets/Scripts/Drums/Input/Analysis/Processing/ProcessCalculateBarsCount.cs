using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessCalculateBarsCount : IProcess<int>
    {
        private readonly DrumsInput _drumsInput;
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        private readonly int _timeSignature;
        
        public ProcessCalculateBarsCount(DrumsInput drumsInput, int timeSignature)
        {
            _drumsInput = drumsInput;
            _beatSources = new ProcessBeatSourcesCreate(_drumsInput).Process();
            _timeSignature = timeSignature;
        }
        
        public int Process()
        {
            var sizeStep = _timeSignature * 4;
            var size = sizeStep;
            var bars = 1;

            while (HasChildren(size))
            {
                size += sizeStep;
                bars++;
            }

            return bars;
        }

        private bool HasChildren(int size)
        {
            if ((float)size / (float) _beatSources.Count < 1.75f)
            {
                return true;
            }
            
            var sizeDuration = _beatSources.Sum(b => b.Duration) / size;
            var filter = new List<DrumsAnalyzerBeatSource>();

            var index = 0;
            
            for (var i = 0; i < size; i++)
            {
                var boundsMin = i * sizeDuration;
                var boundsMax = i * sizeDuration + sizeDuration;

                var hasChild = false;
                
                for(var j = index; j < _beatSources.Count; j++)
                {
                    var beatSource = _beatSources[j];
                    
                    if (beatSource.Time < boundsMin || beatSource.Time >= boundsMax || filter.Contains(beatSource))
                    {
                        continue;
                    }

                    if (hasChild)
                    {
                        return true;
                    }

                    beatSource.Time = i * sizeDuration;
                    index = j + 1;
                    filter.Add(beatSource);
                    hasChild = true;
                }
            }

            return false;
        }
    }
}