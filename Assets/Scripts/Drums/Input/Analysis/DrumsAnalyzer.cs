using System.Collections.Generic;
using System.Linq;
using LooperPooper.Drums.Input.Analysis.Comparison;
using LooperPooper.Drums.Input.Analysis.Processing;
using LooperPooper.Drums.Playback;
using UnityEngine;

namespace LooperPooper.Drums.Input.Analysis
{
    public class DrumsAnalyzer
    {
        public DrumsLoop OutputDrumsLoop { get; private set; }
        public float Error { get; private set; }
        
        public DrumsAnalyzer(DrumsInput drumsInput, int timeSignature, int barSize, int barsCount, int attempt)
        {
            var beatSources = new List<DrumsAnalyzerBeatSource>();
            new ProcessBeatSourcesCreate(drumsInput, beatSources).Process();
            new ProcessBeatSourcesQuantize(beatSources, barSize, barsCount).Process();
            

            foreach (var entry in drumsInput.Entries)
            {
                var x = 10 + Mathf.RoundToInt(entry.Time * 200);
                var y = 10;

                for (var yy = 0; yy < (entry.Type == DrumsBeatType.Kick ? 10 : 20); yy++)
                {
                    Index.Texture.SetPixel(x, y + yy, Color.black);
                }

            }

            foreach (var beatSource in beatSources)
            {
                var x = 10 + Mathf.RoundToInt(beatSource.Time * 200);
                var y = 10 + 30 + attempt * 30;

                for (var yy = 0; yy < (beatSource.Type == DrumsBeatType.Kick ? 10 : 20); yy++)
                {
                    Index.Texture.SetPixel(x, y + yy, Color.blue);    
                }
                
            }
            
            Index.Texture.Apply();

            Error = Mathf.Abs(drumsInput.Entries.Sum(entry => entry.Duration) -
                              beatSources.Sum(beatSource => beatSource.Duration));
            
            for (var i = 0; i < drumsInput.Entries.Count; i++)
            {
                Error += Mathf.Abs(drumsInput.Entries[i].Time - beatSources[i].Time);
            }

            Error -= (float) (barSize * barsCount) * 0.0001f;

            var barDuration = beatSources.Sum(beatSource => beatSource.Duration) / barsCount;
            var bpm = Mathf.Round(timeSignature / barDuration * 60);
            var bpmError = Mathf.Abs(120 - bpm);

            Error += bpmError * 0.01f;
            
            var sizeTotal = beatSources.Sum(beatSource => beatSource.Size);
            
            Debug.Log(barSize + " / " + barsCount + " >> " + Error + " / " + sizeTotal + " / " + bpm);

            SetOutput(beatSources, timeSignature, barSize, barsCount);


            var beatDuration = 60f / OutputDrumsLoop.BeatsPerMinute / (OutputDrumsLoop.BeatsPerBar / 4);
            for (var i = 0; i < OutputDrumsLoop.Bars.Length; i++)
            {
                for (var j = 0; j < OutputDrumsLoop.Bars[i].BeatsKick.Length; j++)
                {
                    if(OutputDrumsLoop.Bars[i].BeatsKick[j].Type == DrumsBeatType.None)
                    {
                        continue;
                    }
                    
                    var index = i * OutputDrumsLoop.Bars[i].BeatsKick.Length + j;
                    var time = beatDuration * index;
                    
                    var x = 10 + Mathf.RoundToInt(time * 200);
                    var y = 10 + 15 + attempt * 30;

                    for (var yy = 0; yy < 10; yy++)
                    {
                        Index.Texture.SetPixel(x, y + yy, Color.red);    
                    }

                }
            }
        }

        private void SetOutput(List<DrumsAnalyzerBeatSource> beatSources, float timeSignature, int barSize, int barsCount)
        {
            var filter = new List<DrumsAnalyzerBeatSource>();
            var beatDuration = beatSources.Sum(beatSource => beatSource.Duration) / (barSize * barsCount);
            var outputBars = new List<DrumsLoopBar>();
            
            for (var i = 0; i < barsCount; i++)
            {
                var outputBeatsKick = new List<DrumsLoopBeat>();
                var outputBeatsSnare = new List<DrumsLoopBeat>();
                
                for (var j = 0; j < barSize; j+=1)
                {
                    var index = barSize * i + j;
                    var timeStep = beatDuration;
                    var timeBoundsMin = index * timeStep;
                    var timeBoundsMax = timeBoundsMin + timeStep;
                    
                    var beatsFound = new List<DrumsAnalyzerBeatSource>();
                    
                    var beatKickFound = false;
                    var beatSnareFound = false;
                    
                    foreach (var beatSource in beatSources)
                    {
                        if (beatSource.Time < timeBoundsMin || beatSource.Time > timeBoundsMax || filter.Contains(beatSource))
                        {
                            continue;
                        }

                        beatsFound.Add(beatSource);
                        filter.Add(beatSource);
                    }

                    foreach (var beatFound in beatsFound)
                    {
                        if (beatFound.Type == DrumsBeatType.Kick)
                        {
                            beatKickFound = true;
                        }
                        
                        if (beatFound.Type == DrumsBeatType.Snare)
                        {
                            beatSnareFound = true;
                        }
                    }

                    outputBeatsKick.Add(new DrumsLoopBeat(beatKickFound ? DrumsBeatType.Kick : DrumsBeatType.None));
                    outputBeatsSnare.Add(new DrumsLoopBeat(beatSnareFound ? DrumsBeatType.Kick : DrumsBeatType.None));
                }
                
                outputBars.Add(new DrumsLoopBar(outputBeatsKick.ToArray(), outputBeatsSnare.ToArray()));
            }

            var barDuration = beatSources.Sum(beatSource => beatSource.Duration) / barsCount;
            var bpm = Mathf.Round(timeSignature / barDuration * 60);
            
            OutputDrumsLoop = new DrumsLoop(outputBars.ToArray(), bpm);
        }
    }
}