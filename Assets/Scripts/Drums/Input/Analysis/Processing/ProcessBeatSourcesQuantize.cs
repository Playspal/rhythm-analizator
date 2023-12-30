using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    /// <summary>
    /// Quantize DrumsAnalyzerBeatSource items time and duration.
    /// </summary>
    public class ProcessBeatSourcesQuantize : IProcess<bool>
    {
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        private readonly int _barSize;
        private readonly int _barsCount;
        
        public ProcessBeatSourcesQuantize(List<DrumsAnalyzerBeatSource> beatSources, int barSize, int barsCount)
        {
            _beatSources = beatSources;
            _barSize = barSize;
            _barsCount = barsCount;
        }

        public bool Process()
        {
            var steps = new int[] {64, 32, 16, 8, 4, 2, 1};

            foreach (var step in steps)
            {
                Quantize(_barSize * _barsCount * step);
            }

            return true;
        }

        private void Quantize(int beatsCount)
        {
            var initialDuration = _beatSources.Sum(beatSource => beatSource.Duration);
            var barDuration = initialDuration / _barsCount;
            var beatDuration = barDuration / _barSize;
            //var beatsCount = _barSize * _barsCount;

            var slots = CreateSlots(beatsCount);
            var weights = CreateWeights(slots, beatsCount);

            for (var i = 0; i < beatsCount; i++)
            {
                var indexLeft = i > 0 ? i - 1 : i;
                var indexCenter = i;
                var indexRight = i < beatsCount - 2 ? i + 1 : i;

                var weightLeft = weights[indexLeft];
                var weightCenter = weights[indexCenter];
                var weightRight = weights[indexRight];

                var isLeftAvailable = slots[indexLeft] == null;
                var isRightAvailable = slots[indexRight] == null;

                var weightTreshold = weightCenter * 1;
                
                if (isLeftAvailable && isRightAvailable)
                {
                    if (weightLeft > weightRight && weightLeft > weightTreshold)
                    {
                        slots[indexLeft] = slots[indexCenter];
                        slots[indexCenter] = null;
                    }
                
                    else if (weightRight > weightLeft && weightRight > weightTreshold)
                    {
                        slots[indexRight] = slots[indexCenter];
                        slots[indexCenter] = null;
                        
                        i++;
                    }
                }
                
                else if (isLeftAvailable && !isRightAvailable && weightLeft > weightTreshold)
                {
                    slots[indexLeft] = slots[indexCenter];
                    slots[indexCenter] = null;
                }
                
                else if (!isLeftAvailable && isRightAvailable && weightRight > weightTreshold)
                {
                    slots[indexRight] = slots[indexCenter];
                    slots[indexCenter] = null;

                    i++;
                }
            }

            for (var i = 0; i < beatsCount; i++)
            {
                if (slots[i] == null)
                {
                    continue;
                }
                
                slots[i].Time = i * beatDuration;
            }

            foreach (var beatSource in _beatSources.Where(beatSource => !slots.Contains(beatSource)))
            {
                beatSource.Time = Mathf.Round(beatSource.Time / beatDuration) * beatDuration;
            }
            
            for (var i = 0; i < _beatSources.Count - 1; i++)
            {
                _beatSources[i].Duration = _beatSources[i + 1].Time - _beatSources[i].Time;
                _beatSources[i].Size = Mathf.RoundToInt(_beatSources[i].Duration / beatDuration);
            }

            _beatSources[^1].Duration = initialDuration - _beatSources[^1].Time;
            _beatSources[^1].Size = Mathf.RoundToInt(_beatSources[^1].Duration / beatDuration);
        }

        private DrumsAnalyzerBeatSource[] CreateSlots(int beatsCount)
        {
            var initialDuration = _beatSources.Sum(beatSource => beatSource.Duration);
            var barDuration = initialDuration / _barsCount;
            var beatDuration = barDuration / _barSize;
            //var beatsCount = _barSize * _barsCount;
            
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

        private float[] CreateWeights(DrumsAnalyzerBeatSource[] slots, int beatsCount)
        {
            //var beatsCount = _barSize * _barsCount;
            
            var weightsCycle = CreateWeightsGroup(slots, beatsCount, _barSize);
            var weightsCycleIndex = 0;
            
            var weights = new float[beatsCount];

            for(var i = 0; i < beatsCount; i++)
            {
                if (i % 4 == 0)
                {
                    weights[i] += 4;
                }
                
                weights[i] += weightsCycle[weightsCycleIndex];

                weightsCycleIndex++;

                if (weightsCycleIndex >= weightsCycle.Length)
                {
                    weightsCycleIndex = 0;
                }
            }

            return weights;
        }

        private float[] CreateWeightsGroup(DrumsAnalyzerBeatSource[] slots, int beatsCount, int size)
        {
            if (size == 0)
            {
                Debug.LogError("CreateWeightsGroup > Size was zero");
                size = beatsCount;
            }
            
//            var beatsCount = _barSize * _barsCount;
            var weights = new float[size];

            var index = 0;
            
            while (index < beatsCount)
            {
                for (var i = 0; i < size; i++)
                {
                    weights[i] += slots[index] == null ? 0 : 1f;
                    index++;
                }
            }
            
            return weights;
        }
    }
}