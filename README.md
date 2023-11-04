# Project FormatLog

Command line formatting tool for structured logs

## Why?

The initial driver for this project was the lack of command line tools that could render logs produced
by Serilog in dotnet applications.

## Usage

Type ```FormatLog --help``` for all options

## Publish exe file

```
dotnet publish -p:SelfContained=true -p:RuntimeIdentifier=win-x64 -p:EnableCompressionInSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=true
```

## Advanced examples

Following example gets all logs of the application via _ssh_ and passes it for formatting. It ensures that timestamp and client ip address is in the formatted output.

```
ssh <machine-ip> "find /var/log/bronekfoto/website-*.log -type f | xargs -I {} cat {}" | FormatLog.exe --format "${@t} ${ClientIp} ${FormattedMessage}"
```

Reading logs via __SSH__ tunnel:
```
ssh -p 1022 -T -L 22:192.168.0.169:22 pi@<public-ip-address>
ssh -l pi localhost "find /var/log/bronekfoto/website-*.log -type f | xargs -I {} cat {}" | FormatLog.exe | grep -E "404"
```

### Example

To get 50 last successful GET requests, execute the following command:
```
λ ssh -l pi localhost "find /var/log/bronekfoto/website-*.log -type f | xargs -I {} cat {}" | FormatLog.exe | grep -E "GET.+200" | grep -vE "(images|css|lib|jpg)" | tail -50
```

