using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.AliyunSls
{
    public class ControlledLevelSwitch
    {
        LoggingLevelSwitch? _loggingLevelSwitch;
        LogEventLevel? _originalLevel;

        public ControlledLevelSwitch(LoggingLevelSwitch? loggingLevelSwitch = null)
        {
            _loggingLevelSwitch = loggingLevelSwitch;
        }

        public bool IsActive => _loggingLevelSwitch != null;

        public bool IsIncluded(LogEvent logEvent)
        {
            return _loggingLevelSwitch == null || (int)_loggingLevelSwitch.MinimumLevel <= (int)logEvent.Level;
        }

        public void Update(LogEventLevel? minimumAcceptedLevel)
        {
            if (minimumAcceptedLevel == null)
            {
                if (_loggingLevelSwitch != null && _originalLevel.HasValue)
                {
                    _loggingLevelSwitch.MinimumLevel = _originalLevel.Value;
                }
                return;
            }

            if (_loggingLevelSwitch == null)
            {
                _originalLevel = LevelAlias.Minimum;
                _loggingLevelSwitch = new LoggingLevelSwitch(minimumAcceptedLevel.Value);
                return;
            }
            _originalLevel ??= _loggingLevelSwitch.MinimumLevel;
            _loggingLevelSwitch.MinimumLevel = minimumAcceptedLevel.Value;
        }
    }
}
