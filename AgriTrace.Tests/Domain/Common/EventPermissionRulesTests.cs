using AgriTrace.Domain.Common;
using FluentAssertions;

namespace AgriTrace.Tests.Domain.Common;

public class EventPermissionRulesTests
{
    [Theory]
    [InlineData("FARM", "HARVEST", true)]
    [InlineData("FARM", "RECEIVE", false)]
    [InlineData("FARM", "INSPECTION", false)]
    [InlineData("PROCESSOR", "RECEIVE", true)]
    [InlineData("PROCESSOR", "PROCESSING", true)]
    [InlineData("PROCESSOR", "PACKAGING", true)]
    [InlineData("PROCESSOR", "SPLIT", true)]
    [InlineData("PROCESSOR", "MERGE", true)]
    [InlineData("PROCESSOR", "TRANSPORT", false)]
    [InlineData("DISTRIBUTOR", "RECEIVE", true)]
    [InlineData("DISTRIBUTOR", "TRANSPORT", true)]
    [InlineData("DISTRIBUTOR", "DISTRIBUTION", true)]
    [InlineData("DISTRIBUTOR", "SPLIT", true)]
    [InlineData("DISTRIBUTOR", "MERGE", true)]
    [InlineData("DISTRIBUTOR", "HARVEST", false)]
    [InlineData("RETAILER", "RECEIVE", true)]
    [InlineData("RETAILER", "RETAIL", true)]
    [InlineData("RETAILER", "SPLIT", true)]
    [InlineData("RETAILER", "PROCESSING", false)]
    [InlineData("INSPECTION", "INSPECTION", true)]
    [InlineData("INSPECTION", "HARVEST", false)]
    [InlineData("SYSTEM", "HARVEST", true)]
    [InlineData("SYSTEM", "RECEIVE", true)]
    [InlineData("SYSTEM", "PROCESSING", true)]
    [InlineData("SYSTEM", "PACKAGING", true)]
    [InlineData("SYSTEM", "TRANSPORT", true)]
    [InlineData("SYSTEM", "DISTRIBUTION", true)]
    [InlineData("SYSTEM", "RETAIL", true)]
    [InlineData("SYSTEM", "INSPECTION", true)]
    [InlineData("SYSTEM", "SPLIT", true)]
    [InlineData("SYSTEM", "MERGE", true)]
    [InlineData("SYSTEM", "RECALL", true)]
    [InlineData("UNKNOWN_ORG", "HARVEST", false)]
    [InlineData("FARM", "UNKNOWN_EVENT", false)]
    [InlineData("", "HARVEST", false)]
    [InlineData("FARM", "", false)]
    public void IsAllowed(string orgTypeCode, string eventTypeCode, bool expected)
    {
        EventPermissionRules.IsAllowed(orgTypeCode, eventTypeCode).Should().Be(expected);
    }
}
