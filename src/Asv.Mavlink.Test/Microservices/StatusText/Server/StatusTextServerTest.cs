using System;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Mavlink.Test;

[TestSubject(typeof(StatusTextServer))]
public class StatusTextServerTest(ITestOutputHelper log)
    : ServerTestBase<StatusTextServer>(log)
{
    private StatusTextLoggerConfig _config = new()
    {
        MaxQueueSize = 100,
        MaxSendRateHz = 10
    };

    protected override StatusTextServer CreateClient(MavlinkIdentity identity, CoreServices core) => new(identity, _config, core);

    [Fact]
    public void Ctor_Identity_Arg_Is_Null_Fail()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var server = new StatusTextServer(null,_config,Core);
        });
    }
    
    [Fact]
    public void Ctor_Core_Arg_Is_Null_Fail()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var server = new StatusTextServer(Identity, _config, null);
        });
    }
    
    [Fact]
    public void Ctor_Config_Arg_Is_Null_Fail()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var server = new StatusTextServer(Identity, null, Core);
        });
    }
}