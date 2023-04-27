using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Asv.Mavlink.V2.AsvSdr;
using Asv.Mavlink.V2.Common;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Mavlink.Test;

public class AsvSdrTest
{
    private readonly ITestOutputHelper _output;
    
    public AsvSdrTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Server_Sdr_Set_Argument_Null_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentNullException>(() =>
        {
            serverSdr.Set(null);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Argument_Null_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentNullException>(() =>
        {
            serverSdr.SendRecord(null);
        });
    }
    
    [Theory]
    [InlineData(AsvSdrCustomMode.AsvSdrCustomModeIdle)]
    [InlineData(AsvSdrCustomMode.AsvSdrCustomModeGp)]
    [InlineData(AsvSdrCustomMode.AsvSdrCustomModeLlz)]
    [InlineData(AsvSdrCustomMode.AsvSdrCustomModeVor)]
    public async Task Server_Sdr_Send_Record_Data_Argument_Null_Exception(AsvSdrCustomMode customMode)
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentNullException>(() =>
        {
            serverSdr.SendRecordData(customMode, null);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Data_Idle_Argument_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentException>(() =>
        {
            serverSdr.SendRecordData(AsvSdrCustomMode.AsvSdrCustomModeIdle, _ => { _.GetMaxByteSize(); });
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Create_Record_Data_Idle_Argument_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentException>(() =>
        {
            serverSdr.CreateRecordData(AsvSdrCustomMode.AsvSdrCustomModeIdle);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Data_Response_Argument_Null_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentNullException>(() =>
        {
            serverSdr.SendRecordDataResponse(null);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Tag_Delete_Response_Argument_Null_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentNullException>(() =>
        {
            serverSdr.SendRecordTagDeleteResponse(null);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Tag_Argument_Null_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentNullException>(() =>
        {
            serverSdr.SendRecordTag(null);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Tag_Response_Argument_Null_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentNullException>(() =>
        {
            serverSdr.SendRecordTagResponse(null);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Delete_Response_Argument_Null_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentNullException>(() =>
        {
            serverSdr.SendRecordDeleteResponse(null);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Response_Argument_Null_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentNullException>(() =>
        {
            serverSdr.SendRecordResponse(null);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Response_Fail_Result_Code_Argument_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentException>(() =>
        {
            serverSdr.SendRecordResponseFail(new AsvSdrRecordRequestPayload(), AsvSdrRequestAck.AsvSdrRequestAckOk);
        });
    }
    
    [Theory]
    [InlineData(AsvSdrRequestAck.AsvSdrRequestAckInProgress)]
    [InlineData(AsvSdrRequestAck.AsvSdrRequestAckFail)]
    public async Task Server_Sdr_Send_Record_Response_Fail_Null_Reference_Exception(AsvSdrRequestAck requestAck)
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<NullReferenceException>(() =>
        {
            serverSdr.SendRecordResponseFail(null, requestAck);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Response_Success_Null_Reference_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<NullReferenceException>(() =>
        {
            serverSdr.SendRecordResponseSuccess(null, 0);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Tag_Response_Fail_Result_Code_Argument_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentException>(() =>
        {
            serverSdr.SendRecordTagResponseFail(new AsvSdrRecordTagRequestPayload(), AsvSdrRequestAck.AsvSdrRequestAckOk);
        });
    }
    
    [Theory]
    [InlineData(AsvSdrRequestAck.AsvSdrRequestAckInProgress)]
    [InlineData(AsvSdrRequestAck.AsvSdrRequestAckFail)]
    public async Task Server_Sdr_Send_Record_Tag_Response_Fail_Null_Reference_Exception(AsvSdrRequestAck requestAck)
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<NullReferenceException>(() =>
        {
            serverSdr.SendRecordTagResponseFail(null, requestAck);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Tag_Response_Success_Null_Reference_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<NullReferenceException>(() =>
        {
            serverSdr.SendRecordTagResponseSuccess(null, 0);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Delete_Response_Fail_Result_Code_Argument_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentException>(() =>
        {
            serverSdr.SendRecordDeleteResponseFail(new AsvSdrRecordDeleteRequestPayload(), AsvSdrRequestAck.AsvSdrRequestAckOk);
        });
    }
    
    [Theory]
    [InlineData(AsvSdrRequestAck.AsvSdrRequestAckInProgress)]
    [InlineData(AsvSdrRequestAck.AsvSdrRequestAckFail)]
    public async Task Server_Sdr_Send_Record_Delete_Response_Fail_Null_Reference_Exception(AsvSdrRequestAck requestAck)
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<NullReferenceException>(() =>
        {
            serverSdr.SendRecordDeleteResponseFail(null, requestAck);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Delete_Response_Success_Null_Reference_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<NullReferenceException>(() =>
        {
            serverSdr.SendRecordDeleteResponseSuccess(null);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Tag_Delete_Response_Fail_Result_Code_Argument_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentException>(() =>
        {
            serverSdr.SendRecordTagDeleteResponseFail(new AsvSdrRecordTagDeleteRequestPayload(), AsvSdrRequestAck.AsvSdrRequestAckOk);
        });
    }
    
    [Theory]
    [InlineData(AsvSdrRequestAck.AsvSdrRequestAckInProgress)]
    [InlineData(AsvSdrRequestAck.AsvSdrRequestAckFail)]
    public async Task Server_Sdr_Send_Record_Tag_Delete_Response_Fail_Null_Reference_Exception(AsvSdrRequestAck requestAck)
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<NullReferenceException>(() =>
        {
            serverSdr.SendRecordTagDeleteResponseFail(null, requestAck);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Tag_Delete_Response_Success_Null_Reference_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<NullReferenceException>(() =>
        {
            serverSdr.SendRecordTagDeleteResponseSuccess(null);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Data_Response_Fail_Result_Code_Argument_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<ArgumentException>(() =>
        {
            serverSdr.SendRecordDataResponseFail(new AsvSdrRecordDataRequestPayload(), AsvSdrRequestAck.AsvSdrRequestAckOk);
        });
    }
    
    [Theory]
    [InlineData(AsvSdrRequestAck.AsvSdrRequestAckInProgress)]
    [InlineData(AsvSdrRequestAck.AsvSdrRequestAckFail)]
    public async Task Server_Sdr_Send_Record_Data_Response_Fail_Null_Reference_Exception(AsvSdrRequestAck requestAck)
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<NullReferenceException>(() =>
        {
            serverSdr.SendRecordDataResponseFail(null, requestAck);
        });
    }
    
    [Fact]
    public async Task Server_Sdr_Send_Record_Data_Response_Success_Null_Reference_Exception()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity(), new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity(), 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        Assert.Throws<NullReferenceException>(() =>
        {
            serverSdr.SendRecordDataResponseSuccess(null, 0);
        });
    }

    [Fact]
    public async Task Client_Get_Record_List_And_Check()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity
            {
                ComponentId = 2,
                SystemId = 2
            }, new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity
            {
                SystemId = 13,
                ComponentId = 13,
                TargetSystemId = 2,
                TargetComponentId = 2
            }, 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();


        var requestId = 0;
        
        serverSdr.OnRecordRequest.Subscribe(async _ =>
        {
            if (_.Skip == 0 & _.Count == 1)
            {
                await serverSdr.SendRecordResponse(__ =>
                {
                    __.Result = AsvSdrRequestAck.AsvSdrRequestAckOk;
                    __.ItemsCount = 1;
                    __.RequestId = _.RequestId;
                    requestId = _.RequestId;
                });
            }
        });
        
        var records = await clientSdr.GetRecordList(0, 1);

        Assert.Equal(AsvSdrRequestAck.AsvSdrRequestAckOk ,records.Result);
        Assert.Equal(1 ,records.ItemsCount);
        Assert.Equal(requestId ,records.RequestId);
    }
    
    [Fact]
    public async Task Client_Get_Record_Data_List_And_Check()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity
            {
                ComponentId = 2,
                SystemId = 2
            }, new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity
            {
                SystemId = 13,
                ComponentId = 13,
                TargetSystemId = 2,
                TargetComponentId = 2
            }, 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        var recordGuid = new Guid();
        recordGuid.TryWriteBytes(new byte[] { 2, 4, 6, 8 });
        
        var requestId = 0;
        
        serverSdr.OnRecordDataRequest.Subscribe(async _ =>
        {
            if (_.Skip == 0 & _.Count == 1)
            {
                await serverSdr.SendRecordDataResponse(__ =>
                {
                    __.DataType = AsvSdrCustomMode.AsvSdrCustomModeLlz;
                    __.RecordGuid = recordGuid.ToByteArray();
                    __.Result = AsvSdrRequestAck.AsvSdrRequestAckOk;
                    __.ItemsCount = 1;
                    __.RequestId = _.RequestId;
                    requestId = _.RequestId;
                });
            }
        });
        
        var recordDataList = await clientSdr.GetRecordDataList(recordGuid, 0, 1);

        Assert.Equal(AsvSdrRequestAck.AsvSdrRequestAckOk ,recordDataList.Result);
        Assert.Equal(AsvSdrCustomMode.AsvSdrCustomModeLlz ,recordDataList.DataType);
        Assert.Equal(requestId, recordDataList.RequestId);
        Assert.Equal((uint)1, recordDataList.ItemsCount);
        Assert.Equal(recordGuid, new Guid(recordDataList.RecordGuid));
    }
    
    [Fact]
    public async Task Client_Get_Record_Tag_List_And_Check()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity
            {
                ComponentId = 2,
                SystemId = 2
            }, new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity
            {
                SystemId = 13,
                ComponentId = 13,
                TargetSystemId = 2,
                TargetComponentId = 2
            }, 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();
        
        var recordGuid = new Guid();
        recordGuid.TryWriteBytes(new byte[] { 2, 4, 6, 8 });

        var requestId = 0;
        
        serverSdr.OnRecordTagRequest.Subscribe(async _ =>
        {
            if (_.Skip == 0 & _.Count == 1)
            {
                await serverSdr.SendRecordTagResponse(__ =>
                {
                    __.Result = AsvSdrRequestAck.AsvSdrRequestAckOk;
                    __.ItemsCount = 1;
                    __.RequestId = _.RequestId;
                    requestId = _.RequestId;
                });
            }
        });
        
        var recordTagList = await clientSdr.GetRecordTagList(recordGuid,0, 1);

        Assert.Equal(AsvSdrRequestAck.AsvSdrRequestAckOk ,recordTagList.Result);
        Assert.Equal(requestId, recordTagList.RequestId);
        Assert.Equal((uint)1, recordTagList.ItemsCount);
    }

    [Fact]
    public async Task Client_Record_Delete_And_Check()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity
            {
                ComponentId = 2,
                SystemId = 2
            }, new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity
            {
                SystemId = 13,
                ComponentId = 13,
                TargetSystemId = 2,
                TargetComponentId = 2
            }, 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();
        
        var recordGuid = new Guid();
        recordGuid.TryWriteBytes(new byte[] { 2, 4, 6, 8 });

        var requestId = 0;
        
        serverSdr.OnRecordDeleteRequest.Subscribe(async _ =>
        {
            if (new Guid(_.RecordGuid).Equals(recordGuid))
            {
                await serverSdr.SendRecordDeleteResponse(__ =>
                {
                    __.Result = AsvSdrRequestAck.AsvSdrRequestAckOk;
                    __.RecordGuid = recordGuid.ToByteArray();
                    __.RequestId = _.RequestId;
                    requestId = _.RequestId;
                });
            }
        });
        
        var recordDelete = await clientSdr.DeleteRecord(recordGuid);

        Assert.Equal(AsvSdrRequestAck.AsvSdrRequestAckOk ,recordDelete.Result);
        Assert.Equal(recordGuid, new Guid(recordDelete.RecordGuid));
        Assert.Equal(requestId, recordDelete.RequestId);
    }
    
    [Fact]
    public async Task Client_Record_Tag_Delete_And_Check()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity
            {
                ComponentId = 2,
                SystemId = 2
            }, new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity
            {
                SystemId = 13,
                ComponentId = 13,
                TargetSystemId = 2,
                TargetComponentId = 2
            }, 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();
        
        var recordGuid = new Guid();
        recordGuid.TryWriteBytes(new byte[] { 2, 4, 6, 8 });

        var tagGuid = new Guid();
        tagGuid.TryWriteBytes(new byte[] { 1, 3, 5, 7 });

        var requestId = 0;
        
        serverSdr.OnRecordTagDeleteRequest.Subscribe(async _ =>
        {
            if (new Guid(_.RecordGuid).Equals(recordGuid) & new Guid(_.TagGuid).Equals(tagGuid))
            {
                await serverSdr.SendRecordTagDeleteResponse(__ =>
                {
                    __.Result = AsvSdrRequestAck.AsvSdrRequestAckOk;
                    __.RecordGuid = recordGuid.ToByteArray();
                    __.RequestId = _.RequestId;
                    requestId = _.RequestId;
                });
            }
        });
        
        var recordTagDelete = await clientSdr.DeleteRecordTag(new TagId(tagGuid, recordGuid));

        Assert.Equal(AsvSdrRequestAck.AsvSdrRequestAckOk ,recordTagDelete.Result);
        Assert.Equal(recordGuid, new Guid(recordTagDelete.RecordGuid));
        Assert.Equal(tagGuid, new Guid(recordTagDelete.TagGuid));
        Assert.Equal(requestId, recordTagDelete.RequestId);
    }
    
    [Fact]
    public async Task Server_Send_Record_Data_Llz_And_Check()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity
            {
                ComponentId = 2,
                SystemId = 2
            }, new AsvSdrServerConfig(),
        new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity
            {
                SystemId = 13,
                ComponentId = 13,
                TargetSystemId = 2,
                TargetComponentId = 2
            }, 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();
        
        var payload = new AsvSdrRecordDataLlzPayload();
        
        clientSdr.OnRecordData.Subscribe(_ =>
        {
            if (_.Payload is AsvSdrRecordDataLlzPayload llzRecordDataPayload)
            {
                payload = llzRecordDataPayload;
            }
        });
        
        await serverSdr.SendRecordData(AsvSdrCustomMode.AsvSdrCustomModeLlz, payload =>
        {
            if (payload is AsvSdrRecordDataLlzPayload llzPayload)
            {
                llzPayload.TimeUnixUsec = 1;
                llzPayload.TotalFreq = 2;
                llzPayload.DataIndex = 3;
                llzPayload.GnssLat = 4;
                llzPayload.GnssLon = 5;
                llzPayload.GnssAlt = 6;
                llzPayload.GnssAltEllipsoid = 7;
                llzPayload.GnssHAcc = 8;
                llzPayload.GnssVAcc = 9;
                llzPayload.GnssVelAcc = 10;
                llzPayload.Lat = 11;
                llzPayload.Lon = 12;
                llzPayload.Alt = 13;
                llzPayload.RelativeAlt = 14;
                llzPayload.Roll = 15;
                llzPayload.Pitch = 16;
                llzPayload.Yaw = 17;
                llzPayload.CrsAm90 = 18;
                llzPayload.CrsAm150 = 19;
                llzPayload.ClrPower = 20;
                llzPayload.ClrAm90 = 21;
                llzPayload.ClrAm150 = 22;
                llzPayload.TotalPower = 23;
                llzPayload.TotalFieldStrength = 24;
                llzPayload.TotalAm90 = 25;
                llzPayload.TotalAm150 = 26;
                llzPayload.Phi90CrsVsClr = 27;
                llzPayload.Phi150CrsVsClr = 28;
                llzPayload.CodeIdAm1020 = 29;
                llzPayload.GnssEph = 30;
                llzPayload.GnssEpv = 31;
                llzPayload.GnssVel = 32;
                llzPayload.Vx = 33;
                llzPayload.Vy = 34;
                llzPayload.Vz = 35;
                llzPayload.Hdg = 36;
                llzPayload.CrsPower = 37;
                llzPayload.CrsCarrierOffset = 38;
                llzPayload.CrsFreq90 = 39;
                llzPayload.CrsFreq150 = 40;
                llzPayload.ClrCarrierOffset = 41;
                llzPayload.ClrFreq90 = 42;
                llzPayload.ClrFreq150 = 43;
                llzPayload.TotalCarrierOffset = 44;
                llzPayload.TotalFreq90 = 45;
                llzPayload.TotalFreq150 = 46;
                llzPayload.CodeIdFreq1020 = 47;
                llzPayload.MeasureTime = 48;
                llzPayload.RecordGuid = new byte[]
                {
                    49, 50, 51, 52
                };
                llzPayload.GnssFixType = GpsFixType.GpsFixTypeNoGps;
                //llzPayload.GnssSatellitesVisible = 54;
            }
        });
        
        await Task.Delay(500);

        #region Asserts
        Assert.Equal((uint)1, payload.TimeUnixUsec);
        Assert.Equal((ulong)2, payload.TotalFreq);
        Assert.Equal((uint)3, payload.DataIndex);
        Assert.Equal(4, payload.GnssLat);
        Assert.Equal(5, payload.GnssLon);
        Assert.Equal(6, payload.GnssAlt);
        Assert.Equal(7, payload.GnssAltEllipsoid);
        Assert.Equal((uint)8, payload.GnssHAcc);
        Assert.Equal((uint)9, payload.GnssVAcc);
        Assert.Equal((uint)10, payload.GnssVelAcc);
        Assert.Equal(11, payload.Lat);
        Assert.Equal(12, payload.Lon);
        Assert.Equal(13, payload.Alt);
        Assert.Equal(14, payload.RelativeAlt);
        Assert.Equal(15, payload.Roll);
        Assert.Equal(16, payload.Pitch);
        Assert.Equal(17, payload.Yaw);
        Assert.Equal(18, payload.CrsAm90);
        Assert.Equal(19, payload.CrsAm150);
        Assert.Equal(20, payload.ClrPower);
        Assert.Equal(21, payload.ClrAm90);
        Assert.Equal(22, payload.ClrAm150);
        Assert.Equal(23, payload.TotalPower);
        Assert.Equal(24, payload.TotalFieldStrength);
        Assert.Equal(25, payload.TotalAm90);
        Assert.Equal(26, payload.TotalAm150);
        Assert.Equal(27, payload.Phi90CrsVsClr);
        Assert.Equal(28, payload.Phi150CrsVsClr);
        Assert.Equal(29, payload.CodeIdAm1020);
        Assert.Equal(30, payload.GnssEph);
        Assert.Equal(31, payload.GnssEpv);
        Assert.Equal(32, payload.GnssVel);
        Assert.Equal(33, payload.Vx);
        Assert.Equal(34, payload.Vy);
        Assert.Equal(35, payload.Vz);
        Assert.Equal(36, payload.Hdg);
        Assert.Equal(37, payload.CrsPower);
        Assert.Equal(38, payload.CrsCarrierOffset);
        Assert.Equal(39, payload.CrsFreq90);
        Assert.Equal(40, payload.CrsFreq150);
        Assert.Equal(41, payload.ClrCarrierOffset);
        Assert.Equal(42, payload.ClrFreq90);
        Assert.Equal(43, payload.ClrFreq150);
        Assert.Equal(44, payload.TotalCarrierOffset);
        Assert.Equal(45, payload.TotalFreq90);
        Assert.Equal(46, payload.TotalFreq150);
        Assert.Equal(47, payload.CodeIdFreq1020);
        Assert.Equal(48, payload.MeasureTime);
        Assert.Equal(new byte[] { 49, 50, 51, 52 }, payload.RecordGuid.Take(4));
        Assert.Equal(GpsFixType.GpsFixTypeNoGps, payload.GnssFixType);
        //Assert.Equal(54, payload.GnssSatellitesVisible);
        #endregion
    }
    
    [Fact]
    public async Task Server_Send_Record_And_Check()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity
            {
                ComponentId = 2,
                SystemId = 2
            }, new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity
            {
                SystemId = 13,
                ComponentId = 13,
                TargetSystemId = 2,
                TargetComponentId = 2
            }, 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        var record = new AsvSdrRecordPayload();
        
        clientSdr.OnRecord.Subscribe(_ =>
        {
            record = _.Item2;
        });
        
        serverSdr.SendRecord( _ =>
        {
            _.DataType = AsvSdrCustomMode.AsvSdrCustomModeLlz;
            _.Frequency = 12;
            _.Size = 20;
            _.DataCount = 50;
            _.RecordName = new[] { 'T', 'E', 'S', 'T' };
            _.TagCount = 4;
            _.CreatedUnixUs = 40;
            _.DurationSec = 2;
        });

        await Task.Delay(500);
        
        Assert.Equal(AsvSdrCustomMode.AsvSdrCustomModeLlz, record.DataType);
        Assert.Equal((ulong)12, record.Frequency);
        Assert.Equal((uint)20, record.Size);
        Assert.Equal((uint)50, record.DataCount);
        Assert.Equal(new[] { 'T', 'E', 'S', 'T' }, record.RecordName.Take(4));
        Assert.Equal(4, record.TagCount);
        Assert.Equal((ulong)40, record.CreatedUnixUs);
        Assert.Equal((uint)2, record.DurationSec);
    }
    
    [Fact]
    public async Task Server_Send_Record_Tag_And_Check()
    {
        var link = new VirtualLink();
        
        var serverSdr = new AsvSdrServer(link.Server,
            new MavlinkServerIdentity
            {
                ComponentId = 2,
                SystemId = 2
            }, new AsvSdrServerConfig(),
            new PacketSequenceCalculator(), Scheduler.Default);
        
        var clientSdr = new AsvSdrClient(link.Client, new MavlinkClientIdentity
            {
                SystemId = 13,
                ComponentId = 13,
                TargetSystemId = 2,
                TargetComponentId = 2
            }, 
            new PacketSequenceCalculator(), Scheduler.Default);

        serverSdr.Start();

        var record = new AsvSdrRecordTagPayload();
        
        clientSdr.OnRecordTag.Subscribe(_ =>
        {
            record = _.Item2;
        });
        
        serverSdr.SendRecordTag(_ =>
        {
            _.RecordGuid = new byte[] { 1, 2, 3, 4};
            //_.TagType = AsvSdrRecordTagType.AsvSdrRecordTagTypeReal64;
        });

        await Task.Delay(500);
        
        Assert.Equal(new byte[] { 1, 2, 3, 4}, record.RecordGuid.Take(4));
        //Assert.Equal(AsvSdrRecordTagType.AsvSdrRecordTagTypeReal64, record.TagType);
    }
}