using Aliyun.Api.LogService;
using Aliyun.Api.LogService.Domain.Log;
using Aliyun.Api.LogService.Infrastructure.Protocol;
using Aliyun.Api.LogService.Infrastructure.Protocol.Http;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.AliyunSls.Batch
{
    public class BatchedAliyunSlsSink : IBatchedLogEventSink
    {
        private HttpLogServiceClient? client { get; set; }
        private AliyunSlsOption option { get; }

        public BatchedAliyunSlsSink(
            AliyunSlsOption option)
        {
            if (string.IsNullOrWhiteSpace(option.AccessKeyId))
            {
                throw new ArgumentNullException(nameof(option.AccessKeyId));
            }
            if (string.IsNullOrWhiteSpace(option.AccessKeySecret))
            {
                throw new ArgumentNullException(nameof(option.AccessKeySecret));
            }
            if (string.IsNullOrWhiteSpace(option.Project))
            {
                throw new ArgumentNullException(option.Project);
            }
            if (string.IsNullOrWhiteSpace(option.LogStore))
            {
                throw new ArgumentNullException(option.LogStore);
            }

            this.option = option;
            
        }

        private HttpLogServiceClient ClientInstance
        {
            get
            {
                if (client == null)
                {
                    client = LogServiceClientBuilders.HttpBuilder
                    .Endpoint(option.Endpoint, option.Project)
                    .Credential(option.AccessKeyId, option.AccessKeySecret)
                    .Build();
                }
                return client;
            }
        }

        public async Task OnEmptyBatchAsync()
        {
            await Task.Delay(0);
        }

        public async Task EmitBatchAsync(IEnumerable<LogEvent> logEvents)
        {
            var logs = new List<LogInfo>();
            foreach (var logEvent in logEvents)
            {
                var contents = new Dictionary<string, string>
                {
                    { "Level", logEvent.Level.ToString() },
                    { "Message", logEvent.RenderMessage() }
                };
                if(logEvent.Exception != null)
                {
                    contents.Add("Exception", logEvent.Exception.ToString());
                }
                foreach (var prop in logEvent.Properties)
                {
                    contents.Add(prop.Key, prop.Value.ToString().Trim('"'));
                }
                logs.Add(new()
                {
                    Time = DateTime.Now,
                    Contents = contents
                });

            }
            var request = new PostLogsRequest(option.LogStore, new LogGroupInfo()
            {
                Logs = logs,
                Topic = option.Topic ?? "",
            });

            var response = await ClientInstance.PostLogStoreLogsAsync(request);
            if (!response.IsSuccess)
            {
                SelfLog.WriteLine(response.Error.ErrorMessage);
            }
        }
    }
}