using Aliyun.Api.LogService;
using Aliyun.Api.LogService.Domain.Log;
using Aliyun.Api.LogService.Infrastructure.Protocol.Http;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;

namespace Serilog.Sinks.AliyunSls.Audit
{
    public class AliyunSlsSink : ILogEventSink
    {
        private HttpLogServiceClient Client { get; }
        private AliyunSlsOption Option { get; }

        public AliyunSlsSink(AliyunSlsOption option)
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

            Option = option;

            Client = LogServiceClientBuilders.HttpBuilder
                .Endpoint(option.Endpoint, option.Project)
                .Credential(option.AccessKeyId, option.AccessKeySecret)
                .Build();
        }

        public void Emit(LogEvent logEvent)
        {
            EmitAsync(logEvent).Wait();
        }

        public async Task EmitAsync(LogEvent logEvent)
        {
            //contents
            var contents = new Dictionary<string, string>();
            contents["Level"] = logEvent.Level.ToString();
            contents["Message"] = logEvent.RenderMessage();
            foreach (var prop in logEvent.Properties)
            {
                contents.Add(prop.Key, prop.Value.ToString().Trim('"'));
            }

            var request = new PostLogsRequest(Option.LogStore, new LogGroupInfo()
            {
                Logs = new List<LogInfo>
            {
                new()
                {
                    Time = DateTime.Now,
                    Contents = contents
                }
            },
                Topic = Option.Topic ?? "",
            });

            // send
            var response = await Client.PostLogStoreLogsAsync(request);
            if (!response.IsSuccess)
            {
                SelfLog.WriteLine(response.Error.ErrorMessage);
            }
        }
    }
}