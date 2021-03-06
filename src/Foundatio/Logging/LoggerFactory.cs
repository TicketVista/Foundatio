﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundatio.Logging {
    public class LoggerFactory : ILoggerFactory {
        private readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>(StringComparer.Ordinal);
        private readonly Dictionary<string, LogLevel> _logLevels = new Dictionary<string, LogLevel>(StringComparer.Ordinal);
        private ILoggerProvider[] _providers = new ILoggerProvider[0];
        private readonly object _sync = new object();

        public ILogger CreateLogger(string categoryName) {
            Logger logger;
            lock (_sync) {
                if (_loggers.TryGetValue(categoryName, out logger))
                    return logger;

                logger = new Logger(this, categoryName);
                _loggers[categoryName] = logger;
            }

            return logger;
        }

        public void AddProvider(ILoggerProvider provider) {
            lock (_sync) {
                _providers = _providers.Concat(new[] { provider }).ToArray();
                foreach (var logger in _loggers)
                    logger.Value.AddProvider(provider);
            }
        }

        internal ILoggerProvider[] GetProviders() {
            return _providers;
        }
    }
}
