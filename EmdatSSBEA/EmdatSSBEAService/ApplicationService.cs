﻿using System;
using System.Diagnostics;

namespace EmdatSSBEAService
{
    public class ApplicationService
    {
        private readonly string _databaseName;
        private readonly string _schemaName;
        private readonly string _queueName;
        private readonly string _executablePath;
        private readonly string _commandLineArguments;
        private readonly Process[] _processes;        

        public ApplicationService(ApplicationServiceConfig config)
        {
            _databaseName = config.DatabaseName;
            _schemaName = config.SchemaName;
            _queueName = config.QueueName;
            _executablePath = config.ExecutablePath;
            _commandLineArguments = config.CommandLineArguments;
            _processes = new Process[config.MaxConcurrency];
        }

        internal void Execute()
        {
            bool newProcessStarted = false;
            for (int i = 0; i < _processes.Length && !newProcessStarted; i++)
            {
                if (_processes[i] == null || _processes[i].HasExited)
                {                    
                    _processes[i] = Process.Start(_executablePath, _commandLineArguments);
                    newProcessStarted = true;
                }
            }

            if(!newProcessStarted)
            {
                throw new QueueActivationException($"The maximum number of processes are already running: {_processes.Length}");
            }            
        }
    }
}