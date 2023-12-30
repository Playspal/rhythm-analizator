using System.IO;
using System.Linq;
using LooperPooper.Drums.Input.Analysis;
using LooperPooper.Drums.Input.Analysis.Processing;
using LooperPooper.Drums.Playback;
using Newtonsoft.Json;
using UnityEngine;

namespace LooperPooper.Drums.Input
{
    public class DrumsInputParser
    {
        private readonly DrumsInput _drumsInput;
        public DrumsInputParser(DrumsInput drumsInput)
        {
            _drumsInput = drumsInput;
            //_drumsInput = LoadDrumsInput();
            Debug.Log(JsonConvert.SerializeObject(_drumsInput));
        }

        public DrumsLoop Parse()
        {
            var analyzer34 = new DrumsAnalyzer(_drumsInput, 3);
            var analyzer44 = new DrumsAnalyzer(_drumsInput, 4);

            var score34 = 0f;
            var score44 = 0f;
            
            var (scorePulsation34, scoreRepeats34) = Validate(analyzer34.OutputDrumsLoop);
            var (scorePulsation44, scoreRepeats44) =  Validate(analyzer44.OutputDrumsLoop);

            if (analyzer34.OutputDrumsLoop.Bars.Length > 1 && analyzer44.OutputDrumsLoop.Bars.Length > 1)
            {
                score34 += scoreRepeats34;
                score44 += scoreRepeats44;
            }
            else
            {
                var timeSignature = new ProcessCalculateTimeSignature(_drumsInput).Process();

                if (timeSignature == 3)
                {
                    score34 += 0.1f;
                }
                else
                {
                    score44 += 0.1f;
                }
            }

            score34 += scorePulsation34;
            score44 += scorePulsation44;
            
//            Debug.LogError("OP !!>> score34: " + score34 + "; score44: " + score44);

            var analyzer = score34 > score44 ? analyzer34 : analyzer44;

            
                
            Debug.Log(analyzer.OutputDrumsLoop.BeatsPerMinute + " // " + analyzer.OutputDrumsLoop.BeatsPerBar + " // " + analyzer.OutputDrumsLoop.Bars.Length);
            
            return analyzer.OutputDrumsLoop;
        }

        private (float, float) Validate(DrumsLoop drumsLoop)
        {
            var scorePulsation = 0f;
            var scoreRepeats = 0f;
            
            //
            // Repeats
            if (drumsLoop.Bars.Length > 1)
            {
                var repeats = new float[drumsLoop.Bars[0].Length];
                var repeatsIterations = 0;

                for (var a = 0; a < drumsLoop.Bars.Length; a++)
                {
                    for (var b = 0; b < drumsLoop.Bars.Length; b++)
                    {
                        if (a == b)
                        {
                            continue;
                        }

                        repeatsIterations++;

                        var barA = drumsLoop.Bars[a];
                        var barB = drumsLoop.Bars[b];

                        for (var i = 0; i < barA.Length; i++)
                        {
                            if (barA.BeatsKick[i].Type == DrumsBeatType.Kick &&
                                barB.BeatsKick[i].Type == DrumsBeatType.Kick)
                            {
                                repeats[i] += 1f;
                            }
                            else if (barA.BeatsSnare[i].Type == DrumsBeatType.Snare &&
                                     barB.BeatsSnare[i].Type == DrumsBeatType.Snare)
                            {
                                repeats[i] += 1f;
                            }
                        }
                    }
                }
                
                var d = string.Empty;

                scoreRepeats = repeats.Sum(x => x / repeatsIterations);

                foreach (var r in repeats)
                {
                    var rr = r/ repeatsIterations;
                    d += "[" + rr + "]";
                }
            
                Debug.LogError(d + " < " + scoreRepeats);
            }

            //
            // Pulsations
            for (var i = 0; i < drumsLoop.Bars.Length; i++)
            {
                var bar = drumsLoop.Bars[i];
                
                for (var j = 0; j < bar.Length; j++)
                {
                    if (j % 4 == 0)
                    {
                        scorePulsation += bar.IsEmptyAt(j) ? -0.1f / (float) bar.Length : 0.25f / (float) bar.Length;
                    }
                    
                    if (j % 2 == 0)
                    {
                        //scorePulsation += bar.IsEmptyAt(j) ? -0.05f / (float) bar.Length : 0.125f / (float) bar.Length;
                    }
                }
            }
            
            return (scorePulsation, scoreRepeats);
        }
        
        private DrumsInput LoadDrumsInput()
        {
            var path = Application.streamingAssetsPath + "/beat1.txt";
            var reader = new StreamReader(path);
            var jsonInput = (reader.ReadToEnd()).Trim();
            reader.Close();

            return JsonConvert.DeserializeObject<DrumsInput>(jsonInput);
        }
    }
}