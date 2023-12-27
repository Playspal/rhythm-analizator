using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    /// <summary>
    /// Quantize DrumsAnalyzerBeatSource items time and duration.
    /// </summary>
    public class ProcessBeatSourcesQuantize : IProcess
    {
        private readonly List<DrumsAnalyzerBeatSource> _beatSources;
        private readonly int _timeSignature;
        private readonly int _barSize;
        private readonly int _barsCount;
        
        public ProcessBeatSourcesQuantize(List<DrumsAnalyzerBeatSource> beatSources, int timeSignature, int barSize, int barsCount)
        {
            _beatSources = beatSources;
            _timeSignature = timeSignature;
            _barSize = barSize;
            _barsCount = barsCount;
        }
        
        public void Process()
        {
            var initialDuration = _beatSources.Sum(beatSource => beatSource.Duration);
            var barDuration = initialDuration / _barsCount;
            var beatDuration = barDuration / _barSize;
            var beatsCount = _barSize * _barsCount;

            for (var x = 0; x < 1; x++)
            {
                var powersQ = Powers(_timeSignature);
                var powersH = Powers(_timeSignature * 2);
                var powersF = Powers(_timeSignature * 4);
                var powersD = Powers(_timeSignature * 8);

                var powers = new List<float>();

                var powersQIndex = 0;
                var powersHIndex = 0;
                var powersFIndex = 0;
                var powersDIndex = 0;
                
                for (var i = 0; i < beatsCount; i++)
                {
                    powers.Add(0);

                    powers[i] += powersQ[powersQIndex];
                    powers[i] += powersH[powersHIndex];
                    powers[i] += powersF[powersFIndex];
                    powers[i] += powersD[powersDIndex];

                    powersQIndex++;
                    powersHIndex++;
                    powersFIndex++;
                    powersDIndex++;

                    if (powersQIndex >= powersQ.Count)
                    {
                        powersQIndex = 0;
                    }

                    if (powersHIndex >= powersH.Count)
                    {
                        powersHIndex = 0;
                    }

                    if (powersFIndex >= powersF.Count)
                    {
                        powersFIndex = 0;
                    }

                    if (powersDIndex >= powersD.Count)
                    {
                        powersDIndex = 0;
                    }
                }

                var powersMax = powers.Max();

                for (var i = 0; i < powers.Count; i++)
                {
                    //powers[i] /= powersMax;
                }
                
                for (var i = 0; i < powers.Count; i++)
                {
                    //powers[i] *= powers[i];
                }

                var slots = new DrumsAnalyzerBeatSource[beatsCount];

                for (var i = 0; i < beatsCount; i++)
                {
                    var boundsMin = i * beatDuration - beatDuration / 2;
                    var boundsMax = i * beatDuration + beatDuration - beatDuration / 2;

                    foreach (var beatSource in _beatSources)
                    {
                        if (beatSource.Time < boundsMin || beatSource.Time >= boundsMax)
                        {
                            continue;
                        }

                        //beatSource.Time = i * beatDuration;
                        slots[i] = beatSource;
                        break;
                    }
                }

                for (var i = 0; i < beatsCount; i++)
                {
                    var indexLeft = i > 0 ? i - 1 : 0;
                    var indexCenter = i;
                    var indexRight = i < beatsCount - 1 ? i + 1 : i;

                    var powerLeft = powers[indexLeft];
                    var powerCenter = powers[indexCenter];
                    var powerRight = powers[indexRight];

                    var isLeftLeftFree = i > 1 && slots[i - 2] == null;
                    var isRightRightFree = i < beatsCount - 2 && slots[i + 2] == null;

                    if (slots[indexCenter] == null)
                    {
                        continue;
                    }

                    if (powerLeft > powerCenter && powerLeft > powerRight && slots[indexLeft] == null)
                    {
                        slots[indexLeft] = slots[indexCenter];
                        slots[indexLeft].Time = indexLeft * beatDuration;
                        slots[indexCenter] = null;
                    }

                    if (indexRight > powerCenter && powerRight > powerLeft && slots[indexRight] == null)
                    {
                        slots[indexRight] = slots[indexCenter];
                        slots[indexRight].Time = indexRight * beatDuration;
                        slots[indexCenter] = null;
                    }
                }


                for (var i = 0; i < _beatSources.Count - 1; i++)
                {
                    _beatSources[i].Duration = _beatSources[i + 1].Time - _beatSources[i].Time;
                    
                }
                
                var d = string.Empty;
                
                powers.ForEach(v =>
                {
                    d += v + "; ";
                });
                
                Debug.Log(d);
            }
        }

        private List<float> PowersEmpty(int size)
        {
            var powers = new List<float>();

            for (var i = 0; i < size; i++)
            {
                powers.Add(0);
            }

            return powers;
        }

        private List<float> Powers(int size)
        {
            var initialDuration = _beatSources.Sum(beatSource => beatSource.Duration);
            var barDuration = initialDuration / _barsCount;
            var beatDuration = barDuration / _barSize;
            
            var powers = new List<float>();

            for (var i = 0; i < size; i++)
            {
                powers.Add(0);
            }
            
            var index = 0f;

            while (index < _barSize * _barsCount)
            {
                for (var i = 0; i < size; i++)
                {
                    if (i == 0)
                    {
                        powers[i] += 2f;
                       // powers[i] += 1f;
                    }
                    
                    if (i == size / 2)
                    {
                        powers[i] += 1f;
                       // powers[i] += 0.5f;//size / 2;
                    }

                    var boundsOffset = beatDuration * 0.5f;
                    var boundsCenter = index * beatDuration;
                    var boundsMin = boundsCenter - boundsOffset;
                    var boundsMax = boundsCenter + boundsOffset;

                    foreach (var beatSource in _beatSources)
                    {
                        if (beatSource.Time < boundsMin || beatSource.Time >= boundsMax)
                        {
                            continue;
                        }

                        var distance = 1 - Mathf.Abs(boundsCenter - beatSource.Time) / boundsOffset;
                        
                        powers[i] += 1;//distance * distance;

                        //break;
                    }

                    index++;
                }
            }

            return powers;
        }
    }
}