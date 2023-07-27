namespace Serilog.Sinks.AliyunSls
{
    public class AliyunSlsOption
    {

        /// <summary>
        /// 域
        /// </summary>
        public string? Endpoint { get; set; }

        /// <summary>
        /// Access Key ID
        /// </summary>
        public string? AccessKeyId { get; set; }

        /// <summary>
        /// Access Key Secret
        /// </summary>
        public string? AccessKeySecret { get; set; }

        /// <summary>
        /// Project
        /// </summary>
        public string? Project { get; set; }

        /// <summary>
        /// Logstore
        /// </summary>
        public string? LogStore { get; set; }

        /// <summary>
        /// Topic
        /// </summary>
        public string? Topic { get; set; }
    }
}