﻿using System;
using System.Collections.Generic;
using Backups.Algorithm;
using Backups.MyDateTime;
using Newtonsoft.Json;

namespace Backups.Backups.Impl
{
    public class RestorePoint : IRestorePoint
    {
        public RestorePoint(string name, IAlgorithmStorage algorithm, List<string> backupJobs)
        {
            CheckName(name);

            Time = CurrentDate.GetInstance().Date;
            Name = name;
            Algorithm = algorithm;
            BackupJobs = new List<string>(backupJobs);
        }

        public DateTime Time { get; }
        public string Name { get; }
        public IAlgorithmStorage Algorithm { get; }
        [JsonProperty]
        protected List<string> BackupJobs { get; }
        public IReadOnlyList<string> Jobs() => BackupJobs;

        private void CheckName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }
        }
    }
}