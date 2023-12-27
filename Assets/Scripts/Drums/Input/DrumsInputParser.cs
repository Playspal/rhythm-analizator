using System.Collections.Generic;
using System.IO;
using System.Linq;
using LooperPooper.Drums.Input.Analysis;
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
            _drumsInput = LoadDrumsInput();
            Debug.Log(JsonConvert.SerializeObject(_drumsInput));
        }

        public DrumsLoop Parse()
        {
            var analyzers = new List<DrumsAnalyzer>()
            {
                new DrumsAnalyzer(_drumsInput, 4, 16, 4, 0),
                /*
                 new DrumsAnalyzer(_drumsInput, 4, 16, 4, 0),
                new DrumsAnalyzer(_drumsInput, 4, 16, 3, 1),
                new DrumsAnalyzer(_drumsInput, 4, 16, 2, 2),
                new DrumsAnalyzer(_drumsInput, 4, 16, 1, 3),
                
                new DrumsAnalyzer(_drumsInput, 3, 12, 4, 4),
                new DrumsAnalyzer(_drumsInput, 3, 12, 3, 5),
                new DrumsAnalyzer(_drumsInput, 3, 12, 2, 6),
                new DrumsAnalyzer(_drumsInput, 3, 12, 1, 7)
                */
            };

            var analyzer = analyzers.OrderByDescending(analyzer => analyzer.Error).Last();
            
            Debug.Log(analyzer.OutputDrumsLoop.BeatsPerMinute + " // " + analyzer.OutputDrumsLoop.BeatsPerBar + " // " + analyzer.OutputDrumsLoop.Bars.Length);
            
            return analyzer.OutputDrumsLoop;
        }

        private DrumsInput LoadDrumsInput()
        {
            var path = Application.streamingAssetsPath + "/beat3.txt";
            var reader = new StreamReader(path);
            var jsonInput = (reader.ReadToEnd()).Trim();
            reader.Close();

            return JsonConvert.DeserializeObject<DrumsInput>(jsonInput);
        }
    }
}