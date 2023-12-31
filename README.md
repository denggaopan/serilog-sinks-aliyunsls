# step 1
```
Install-Package Serilog.Sinks.AliyunSls
```
# step 2
add services
```
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();
```
# step 3
edit config
```
"Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "AliyunSls",
        "Args": {
          "Endpoint": "",
          "AccessKeyId": "",
          "AccessKeySecret": "",
          "Project": "",
          "LogStore": "",
          "Topic": "",
          "BatchSizeLimit": 1000,
          "Period": 2,
          "QueueLimit": 100000,
          "EagerlyEmitFirstEvent": true,
          "EnableBatch": true
        }
      }
    ],
    "Enrich": ["FromLogContext"]
  }
  ```
# step 4
run & verify
![aliyun sls query result](https://github.com/denggaopan/serilog-sinks-aliyunsls/blob/main/imgs/aliyunsls_query_result.png)
