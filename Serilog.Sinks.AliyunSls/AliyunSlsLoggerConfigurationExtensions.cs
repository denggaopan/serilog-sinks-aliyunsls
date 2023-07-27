using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.AliyunSls;
using Serilog.Sinks.AliyunSls.Audit;
using Serilog.Sinks.AliyunSls.Batch;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog
{
    public static class AliyunSlsLoggerSinkConfigurationExtensions
    {
        const int DefaultBatchSizeLimit = 1000;
        const double DefaultPeriod = 2;

        public static LoggerConfiguration AliyunSls(this LoggerSinkConfiguration loggerSinkConfiguration,
            string endpoint,
            string accessKeyId,
            string accessKeySecret,
            string project,
            string logStore,
            string topic,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int? batchSizeLimit = null,
            double? period = null,
            int? queueLimit = null,
            bool? eagerlyEmitFirstEvent = null,
            bool enableBatch = true)
        {
           var aliyunSlsOption = new AliyunSlsOption()
            {
                Endpoint = endpoint,
                AccessKeyId = accessKeyId,
                AccessKeySecret = accessKeySecret,
                Project = project,
                LogStore = logStore,
                Topic = topic
            };

            if (enableBatch)
            {
                var batchedSink = new BatchedAliyunSlsSink(aliyunSlsOption);
                var options = new PeriodicBatchingSinkOptions
                {
                    EagerlyEmitFirstEvent = eagerlyEmitFirstEvent ?? true,
                    BatchSizeLimit = batchSizeLimit ?? DefaultBatchSizeLimit,
                    Period = TimeSpan.FromSeconds(period ?? DefaultPeriod),
                    QueueLimit = queueLimit
                };
                var sink = new PeriodicBatchingSink(batchedSink, options);
                return loggerSinkConfiguration.Sink(sink, restrictedToMinimumLevel);
            }

            var aliyunSlsSink =  new AliyunSlsSink(aliyunSlsOption);
            return loggerSinkConfiguration.Sink(aliyunSlsSink, restrictedToMinimumLevel);
        }
    }
}