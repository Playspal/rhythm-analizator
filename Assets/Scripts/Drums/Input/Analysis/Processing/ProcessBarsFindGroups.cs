using System.Collections.Generic;
using System.Linq;
using LooperPooper.Drums.Input.Analysis.Comparison;

namespace LooperPooper.Drums.Input.Analysis.Processing
{
    public class ProcessBarsFindGroups : IProcess<bool>
    {
        private readonly List<DrumsAnalyzerBar> _bars;
        
        public ProcessBarsFindGroups(List<DrumsAnalyzerBar> bars)
        {
            _bars = bars;
        }
        
        public bool Process()
        {
            for (var i = 0; i < 100; i++)
            {
                var beats = GetAllBeatsAt(_bars, i);
                var equals = new List<List<DrumsAnalyzerBeat>>();

                for (var n = 0; n < beats.Count; n++)
                {
                    for (var k = n + 1; k < beats.Count; k++)
                    {
                        var beatA = beats[n];
                        var beatB = beats[k];

                        if (!new BeatComparator(beatA, beatB).IsEquals)
                        {
                            continue;
                        }

                        var wasAdded = false;

                        foreach (var equal in equals)
                        {
                            if (equal.Contains(beatA) && !equal.Contains(beatB))
                            {
                                equal.Add(beatB);
                                wasAdded = true;
                                break;
                            }
                            else if (equal.Contains(beatB) && !equal.Contains(beatA))
                            {
                                equal.Add(beatA);
                                wasAdded = true;
                                break;
                            }
                        }

                        if (!wasAdded)
                        {
                            equals.Add(new List<DrumsAnalyzerBeat>() {beatA, beatB});
                        }
                    }
                }

                foreach (var equal in equals)
                {
                    equal.ForEach(beat =>
                    {
                        //if (beat.Source.GroupIsSet)
                        //{
                            //return;
                        //}
                        
                        //beat.Source.Group = i;
                        //beat.Source.GroupIsSet = true;
                    });
                }
                
                foreach (var equal in equals)
                {
                    var average = equal.Average(beat => beat.Duration);
                
                    equal.ForEach(beat =>
                    {
                        beat.Source.Duration = average;
                    });
                }
                
                var time = 0f;
        
                foreach (var bar in _bars)
                {
                    foreach (var beat in bar.Beats)
                    {
                        beat.Source.Time = time;
                        time += beat.Source.Duration;
                    }
                }
            }
            
            return true;
        }
        
        private static List<DrumsAnalyzerBeat> GetAllBeatsAt(List<DrumsAnalyzerBar> bars, int index)
        {
            var beats = new List<DrumsAnalyzerBeat>();

            foreach (var bar in bars)
            {
                if (index < bar.Beats.Count)
                {
                    beats.Add(bar.Beats[index]);
                }
            }

            return beats;
        }
        
        /*
private static List<DrumsParserBar> NormalizeBars(List<DrumsParserBar> bars, float barDuration)
    {

        for (var i = 0; i < 100; i++)
        {
            var beats = GetAllBeatsAt(bars, i);
            var equals = new List<List<DrumsParserBeat>>();
            
            for (var n = 0; n < beats.Count; n++)
            {
                for (var k = n + 1; k < beats.Count; k++)
                {
                    var beatA = beats[n];
                    var beatB = beats[k];

                    if (new BeatComparator(beatA, beatB).IsEquals)
                    {
                        var wasAdded = false;
                        
                        foreach (var equal in equals)
                        {
                            if (equal.Contains(beatA) && !equal.Contains(beatB))
                            {
                                equal.Add(beatB);
                                wasAdded = true;
                                break;
                            }
                            else if (equal.Contains(beatB) && !equal.Contains(beatA))
                            {
                                equal.Add(beatA);
                                wasAdded = true;
                                break;
                            }
                        }

                        if (!wasAdded)
                        {
                            equals.Add(new List<DrumsParserBeat>() { beatA, beatB });
                        }
                    }
                }
            }

            foreach (var equal in equals)
            {
                var average = equal.Average(beat => beat.Duration);
                
                equal.ForEach(beat =>
                {
                    beat.Source.Duration = average;
                });
            }
        }
        
        var time = 0f;
        
        foreach (var bar in bars)
        {
            foreach (var beat in bar.Beats)
            {
                beat.Source.Time = time;
                time += beat.Source.Duration;
            }
        }
        

        return bars;
    }
         */
    }
}