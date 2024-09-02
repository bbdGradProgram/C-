using Amazon.CloudWatchEvents;
using Amazon.CloudWatchEvents.Model;
using Amazon.Lambda;
using Shared.DTOs;
using Shared.Models;
using System.Text.Json;
using Amazon;

namespace Server.Services {
    public interface IMonitorService
{
    Task SetupMonitoringAsync(Website website);
}

public class MonitorService : IMonitorService
{
    private readonly AmazonCloudWatchEventsClient _cloudWatchEventsClient;
    private readonly AmazonLambdaClient _lambdaClient;

    public MonitorService()
    {
        _cloudWatchEventsClient = new AmazonCloudWatchEventsClient(RegionEndpoint.EUWest1);
        _lambdaClient = new AmazonLambdaClient(RegionEndpoint.EUWest1);
    }

    public async Task SetupMonitoringAsync(Website website)
    {
        var ruleName = $"monitor-website-{website.WebsiteID}";
        await _cloudWatchEventsClient.PutRuleAsync(new PutRuleRequest
        {
            Name = ruleName,
            ScheduleExpression = "rate(5 minutes)", // set the standard here 
            State = RuleState.ENABLED,
            Description = $"Check website availability for {website.Url}"
        });
        await SetupRuleTarget(ruleName, website);
    }

    private async Task SetupRuleTarget(string ruleName, Website website)
    {
        var lambdaFunctionName = "monitorWebsite"; 
        var ruleArn = $"arn:aws:events:eu-west-1:683044484462:rule/{ruleName}";

        await _lambdaClient.AddPermissionAsync(new Amazon.Lambda.Model.AddPermissionRequest
        {
            FunctionName = lambdaFunctionName,
            StatementId = $"{ruleName}-permission",
            Action = "lambda:InvokeFunction",
            Principal = "events.amazonaws.com",
            SourceArn = ruleArn
        });
        await _cloudWatchEventsClient.PutTargetsAsync(new PutTargetsRequest
        {
            Rule = ruleName,
            Targets =
            [
                new Target
                {
                    Id = "1",
                    Arn = $"arn:aws:lambda:eu-west-1:683044484462:function:{lambdaFunctionName}",
                    Input = JsonSerializer.Serialize(new WebsiteGetDto(
                        website.WebsiteID, website.Url, website.UserID
                    ))
                }
            ]
        });
    }
}
}