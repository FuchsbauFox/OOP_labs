﻿using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Backups;
using Backups.MyDateTime;
using BackupsExtra.BackupsExtra.Impl;
using BackupsExtra.Tools.BackupsExtra;
using Newtonsoft.Json;

namespace BackupsExtra.Strategies.Impl
{
    public class ByDateOfCreation : ICleaningStrategy
    {
        [JsonProperty]
        private TimeSpan _timeSpan;

        public ByDateOfCreation(TimeSpan timeSpan)
        {
            CheckData(timeSpan);
            _timeSpan = timeSpan;
        }

        public void CleaningPoints(BackupJobExtra backupJobExtra)
        {
            List<IRestorePoint> pointsToRemove = GetListPointsToRemove(backupJobExtra);

            if (pointsToRemove.Count >= backupJobExtra.Points().Count)
                throw new AllRestorePointsNotPassedLimitsException();

            foreach (IRestorePoint point in pointsToRemove)
                backupJobExtra.DeleteRestorePoint(point.Name);
        }

        public List<IRestorePoint> GetListPointsToRemove(BackupJobExtra backupJobExtra)
        {
            return backupJobExtra.Points()
                .Select(point => point)
                .Where(point => CurrentDate.GetInstance().Date > point.Time + _timeSpan)
                .Select(point => point).ToList();
        }

        private void CheckData(TimeSpan timeSpan)
        {
            if (timeSpan == null)
                throw new ArgumentNullException();
            if (timeSpan.Ticks <= 0)
                throw new ArgumentException();
        }
    }
}