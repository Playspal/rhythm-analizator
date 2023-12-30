using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsComparison : IProcess<bool>
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        private readonly int _timeSignature;
        private readonly int _barSize;
        private readonly float _beatDuration;
        
        public ProcessBarsComparison(List<DrumsAnalyzerBar> bars, int timeSignature, int barSize, float beatDuration)
        {
            _bars = bars;
            _timeSignature = timeSignature;
            _barSize = barSize;
            _beatDuration = beatDuration;
        }
        
        public bool Process()
        {
            var bars = new List<DrumsAnalyzerBar>();
            
            var weights = new float[_barSize];

            /*
            for (var i = 0; i < _barSize; i++)
            {
                if (i % _timeSignature == 0)
                {
                    weights[i] += _timeSignature;
                }
                
                if (i % (_timeSignature / 2) == 0)
                {
                    weights[i] += _timeSignature / 2;
                }
            }
            */
            foreach (var bar in _bars)
            {
                var index = 0;
                
                for (var i = 0; i < bar.Beats.Count; i++)
                {
                    var beat = bar.Beats[i];

                    if (beat.Type != DrumsBeatType.None)
                    {
                        if (index % _timeSignature == 0)
                        {
                            weights[index] += 1;
                        }
                        if (index % (_timeSignature / 2) == 0)
                        {
                            weights[index] += 1;
                        }
                        else
                        {
                            weights[index] += 1;
                        }
                    }
                    

                    index += beat.Size;
                }
            }
            
            foreach (var bar in _bars)
            {
                var index = 0;
                
                for (var i = 0; i < bar.Beats.Count; i++)
                {
                    var beat = bar.Beats[i];
                    var beatLeft = i > 0 ? bar.Beats[i - 1] : null;
                    
                    var indexLeft = index > 0 ? index - 1 : index;
                    var indexRight = index < _barSize - 2 ? index + 1 : index;
                    
                    var weightCenter = weights[index];
                    var weightLeft = weights[indexLeft];
                    var weightRight = weights[indexRight];

                    /*
                    if (weightLeft > weightRight && weightLeft > weightCenter && weightCenter < _bars.Count)
                    {
                        Debug.Log("LEFT");
                        var beatLeft = bar.Beats[i - 1];

                        beat.Source.Time -= _beatDuration;
                        beat.Source.Duration += _beatDuration;
                        beat.Source.Size += 1;
                        
                        beatLeft.Source.Duration -= _beatDuration;
                        beatLeft.Source.Size -= 1;
                    }
                    
                    if (weightRight > weightLeft && weightRight > weightCenter && weightCenter < _bars.Count)
                    {
                        Debug.Log("RIGHT");
                        beat.Source.Time += _beatDuration;
                        beat.Source.Duration -= _beatDuration;
                        beat.Source.Size -= 1;
                    }
                    */
                    Debug.Log(index + " " + weightLeft + " <> " + weightCenter + " <> " + weightRight);
                    if (beatLeft != null && beatLeft.Size > 0 && weightLeft >= weightCenter - 1)
                    {
                        beat.Source.Time -= _beatDuration;
                        beat.Source.Duration += _beatDuration;
                        beat.Source.Size = Mathf.RoundToInt(beat.Source.Duration / _beatDuration);

                        beatLeft.Source.Duration -= _beatDuration;
                        beatLeft.Source.Size = Mathf.RoundToInt(beatLeft.Source.Duration / _beatDuration);

                        /*
                        for (var j = i + 1; j < bar.Beats.Count; j++)
                        {
                            bar.Beats[j].Source.Time -= _beatDuration;
                        }
                        
                        bar.Beats[bar.Beats.Count - 1].Source.Duration += 
                        */
                        index--;
                    }
                    
                    index += beat.Size;
                }
            }

            var d = string.Empty;

            for (var i = 0; i < weights.Length; i++)
            {
                d += "[" + i + " : " + weights[i] + "], ";
            }
            
            Debug.Log(d);
            
            return true;
        }
    }
}