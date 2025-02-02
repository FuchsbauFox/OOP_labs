﻿using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Algorithm.Impl;
using Backups.Backups;
using Backups.FSAdapter;
using Backups.FSAdapter.Impl;
using Backups.MyDateTime;
using BackupsExtra.BackupsExtra.Impl;
using BackupsExtra.Enrichers;
using BackupsExtra.Strategies;
using BackupsExtra.Strategies.Impl;
using NUnit.Framework;
using Serilog;

namespace BackupsExtra.Tests
{
    public class Tests
    {
        private IFsAdapter _adapter;
        private Backup _backup;

        [SetUp]
        public void Setup()
        {
            _adapter = new VirtualFsAdapter();
            _adapter.AddDirectory(@"C:\programming");
            _adapter.AddDirectory(@"C:\programming\OOP");
            _adapter.AddDirectory(@"C:\programming\Algorithm");

            _adapter.AddFile(@"C:\programming\OOP\lab-0.txt");
            _adapter.AddContentOnFile(@"C:\programming\OOP\lab-0.txt", "It's lab-0");
            _adapter.AddFile(@"C:\programming\OOP\lab-1.txt");
            _adapter.AddContentOnFile(@"C:\programming\OOP\lab-1.txt", "It's lab-1");
            _adapter.AddFile(@"C:\programming\OOP\lab-2.txt");
            _adapter.AddContentOnFile(@"C:\programming\OOP\lab-2.txt", "It's lab-2");
            _adapter.AddFile(@"C:\programming\OOP\lab-3.txt");
            _adapter.AddContentOnFile(@"C:\programming\OOP\lab-3.txt", "It's lab-3");

            _adapter.AddFile(@"C:\programming\Algorithm\DFS.txt");
            _adapter.AddContentOnFile(@"C:\programming\Algorithm\DFS.txt", "Algorithm DFS:");
            _adapter.AddFile(@"C:\programming\Algorithm\BFS.txt");
            _adapter.AddContentOnFile(@"C:\programming\Algorithm\BFS.txt", "Algorithm BFS:");
        }

        [Test]
        public void CheckCleaningAlgorithm_ByDateOfCreation()
        {
            _backup = new Backup(_adapter, new ByDateOfCreation(new TimeSpan(10, 0, 0, 0)), 
                new LoggerConfiguration().Enrich.With(new ThreadIdEnricher())
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm} [{Level}] {Message}{NewLine}{Exception}{NewLine}"));
            _backup.SetAlgorithmStorage(new SplitStorage());
            _backup.AddJobObject(@"C:\programming\OOP\lab-0.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-1.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-2.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-3.txt");

            _backup.CreateBackup("Restore1");
            _backup.CreateBackup("Restore2");
            _backup.CreateBackup("Restore3");
            CurrentDate.GetInstance().AddMonths(1);
            _backup.CreateBackup("Restore4");
            var restorePoints = new List<IRestorePoint> {_backup.BackupJobExtra.Points().Last()};

            Assert.AreEqual(_backup.BackupJobExtra.Points().ToList(), restorePoints);
        }

        [Test]
        public void CheckCleaningAlgorithm_ByNumberOfPoints()
        {
            _backup = new Backup(_adapter, new ByNumberOfPoints(2), 
                new LoggerConfiguration().Enrich.With(new ThreadIdEnricher())
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm} [{Level}] {Message}{NewLine}{Exception}{NewLine}"));
            _backup.SetAlgorithmStorage(new SplitStorage());
            _backup.AddJobObject(@"C:\programming\OOP\lab-0.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-1.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-2.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-3.txt");

            _backup.CreateBackup("Restore1");
            _backup.CreateBackup("Restore2");
            _backup.CreateBackup("Restore3");
            _backup.CreateBackup("Restore4");
            var restorePoints = new List<IRestorePoint>
            {
                _backup.BackupJobExtra.Points()[_backup.BackupJobExtra.Points().Count - 2],
                _backup.BackupJobExtra.Points()[_backup.BackupJobExtra.Points().Count - 1]
            };

            Assert.AreEqual(_backup.BackupJobExtra.Points().ToList(), restorePoints);
        }

        [Test]
        public void CheckCleaningAlgorithm_HybridIfAll()
        {
            _backup = new Backup(_adapter,
                new HybridIfAll(new List<CleaningStrategy>()
                {
                    new ByNumberOfPoints(2),
                    new ByDateOfCreation(new TimeSpan(10, 0, 0, 0)),
                }), 
                new LoggerConfiguration().Enrich.With(new ThreadIdEnricher())
                    .WriteTo.Console(
                        outputTemplate: "{Timestamp:HH:mm} [{Level}] {Message}{NewLine}{Exception}{NewLine}"));
            
            _backup.SetAlgorithmStorage(new SplitStorage());
            _backup.AddJobObject(@"C:\programming\OOP\lab-0.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-1.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-2.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-3.txt");

            _backup.CreateBackup("Restore1");
            _backup.CreateBackup("Restore2");
            _backup.CreateBackup("Restore3");
            CurrentDate.GetInstance().AddMonths(1);
            _backup.CreateBackup("Restore4");
            var restorePoints = new List<IRestorePoint>
            {
                _backup.BackupJobExtra.Points()[_backup.BackupJobExtra.Points().Count - 2],
                _backup.BackupJobExtra.Points()[_backup.BackupJobExtra.Points().Count - 1]
            };

            Assert.AreEqual(_backup.BackupJobExtra.Points().ToList(), restorePoints);
        }

        [Test]
        public void CheckCleaningAlgorithm_HybridIfAtLeastOne()
        {
            _backup = new Backup(_adapter,
                new HybridIfAtLeastOne(new List<CleaningStrategy>()
                {
                    new ByNumberOfPoints(2),
                    new ByDateOfCreation(new TimeSpan(10, 0, 0, 0)),
                }),
                new LoggerConfiguration().Enrich.With(new ThreadIdEnricher())
                    .WriteTo.Console(
                        outputTemplate: "{Timestamp:HH:mm} [{Level}] {Message}{NewLine}{Exception}{NewLine}"));
            
            _backup.SetAlgorithmStorage(new SplitStorage());
            _backup.AddJobObject(@"C:\programming\OOP\lab-0.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-1.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-2.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-3.txt");

            _backup.CreateBackup("Restore1");
            _backup.CreateBackup("Restore2");
            _backup.CreateBackup("Restore3");
            CurrentDate.GetInstance().AddMonths(1);
            _backup.CreateBackup("Restore4");
            var restorePoints = new List<IRestorePoint> {_backup.BackupJobExtra.Points().Last()};

            Assert.AreEqual(_backup.BackupJobExtra.Points().ToList(), restorePoints);
        }

        [Test]
        public void MergeRestorePoints()
        {
            _backup = new Backup(_adapter, new ByNumberOfPoints(10),
                new LoggerConfiguration().Enrich.With(new ThreadIdEnricher())
                    .WriteTo.Console(
                        outputTemplate: "{Timestamp:HH:mm} [{Level}] {Message}{NewLine}{Exception}{NewLine}"));
            _backup.SetAlgorithmStorage(new SplitStorage());
            _backup.AddJobObject(@"C:\programming\OOP\lab-0.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-1.txt");
            _backup.CreateBackup("Restore1");
            _backup.RemoveJobObject(@"C:\programming\OOP\lab-0.txt");
            _backup.RemoveJobObject(@"C:\programming\OOP\lab-1.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-2.txt");
            _backup.AddJobObject(@"C:\programming\OOP\lab-3.txt");
            _backup.CreateBackup("Restore2");

            var jobObjects = new List<string>
            {
                _backup.BackupJobExtra.Points()[1].Jobs()[0],
                _backup.BackupJobExtra.Points()[1].Jobs()[1],
                _backup.BackupJobExtra.Points()[0].Jobs()[0],
                _backup.BackupJobExtra.Points()[0].Jobs()[1],
            };

            _backup.MergeRestorePoints("Restore1", "Restore2");

            Assert.AreEqual(_backup.BackupJobExtra.Points().Count, 1);
            Assert.AreEqual(_backup.BackupJobExtra.Points()[0].Jobs(), jobObjects);
        }
    }
}