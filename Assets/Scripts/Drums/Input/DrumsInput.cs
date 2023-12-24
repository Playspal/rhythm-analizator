using System.Collections.Generic;
using UnityEngine;

namespace LooperPooper.Drums.Input
{
    public class DrumsInput
    {
        public readonly List<DrumsInputEntry> Entries = new();
        public float Duration;

        private float _timeBegin;
        private bool _isStarted;

        public void Begin()
        {
            _timeBegin = Time.realtimeSinceStartup;
            _isStarted = true;
        }

        public void End()
        {
            UpdateLastEntryDuration();
            Duration = GetRelativeTime();
        }

        public void Add(DrumsBeatType beat)
        {
            if (!_isStarted)
            {
                Begin();
            }

            UpdateLastEntryDuration();
            Entries.Add(new DrumsInputEntry(beat, GetRelativeTime(), 0));
        }


        private void UpdateLastEntryDuration()
        {
            if (Entries.Count <= 0)
            {
                return;
            }

            var entry = Entries[^1];
            var entryDuration = GetRelativeTime() - entry.Time;

            Entries[^1] = new DrumsInputEntry(entry.Type, entry.Time, entryDuration);
        }

        private float GetRelativeTime()
            => Time.realtimeSinceStartup - _timeBegin;
    }
}