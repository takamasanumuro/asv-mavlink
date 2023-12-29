using System.Reactive.Linq;
using Asv.Common;

namespace Asv.Mavlink;

/// <summary>
/// Represents a client device that communicates over a Mavlink connection.
/// </summary>
public interface IClientDevice
{
    /// <summary>
    /// Gets the full ID of the property.
    /// </summary>
    /// <remarks>
    /// The full ID is a unique identifier that represents the property.
    /// </remarks>
    /// <returns>
    /// The full ID of the property as a <see cref="ushort"/> value.
    /// </returns>
    ushort FullId { get; }

    /// <summary>
    /// Gets the instance of the IMavlinkV2Connection interface.
    /// </summary>
    /// <value>
    /// An instance of the IMavlinkV2Connection interface.
    /// </value>
    IMavlinkV2Connection Connection { get; }

    /// <summary>
    /// Gets the property Seq which represents an IPacketSequenceCalculator object.
    /// </summary>
    /// <value>
    /// An IPacketSequenceCalculator object.
    /// </value>
    IPacketSequenceCalculator Seq { get; }

    /// <summary>
    /// Gets the identity of the Mavlink client.
    /// </summary>
    /// <returns>The identity of the Mavlink client.</returns>
    MavlinkClientIdentity Identity { get; }

    /// <summary>
    /// Gets the device class.
    /// </summary>
    /// <value>
    /// The device class.
    /// </value>
    DeviceClass Class { get; }

    /// <summary>
    /// Gets the value of the Name property.
    /// </summary>
    /// <returns>The value of the Name property.</returns>
    IRxValue<string> Name { get; }

    /// <summary>
    /// Gets the interface for the heartbeat client.
    /// </summary>
    /// <value>
    /// The heartbeat client.
    /// </value>
    IHeartbeatClient Heartbeat { get; }

    /// <summary>
    /// Gets the client for retrieving the status text.
    /// </summary>
    /// <value>
    /// The client for retrieving the status text.
    /// </value>
    IStatusTextClient StatusText { get; }

    /// <summary>
    /// Gets the initial value of the property.
    /// </summary>
    /// <value>
    /// An instance of <see cref="IRxValue{T}"/> representing the initial value
    /// of the property.
    /// </value>
    IRxValue<InitState> OnInit { get; }
}

/// <summary>
/// Provides helper methods for working with client devices.
/// </summary>
public static class ClientDeviceHelper
{
    /// <summary>
    /// Waits until the client device is connected.
    /// </summary>
    /// <param name="client">The client device.</param>
    public static void WaitUntilConnect(this IClientDevice client)
    {
        client.Heartbeat.Link.Where(_ => _ == LinkState.Connected).FirstAsync().Wait();
    }

    /// <summary>
    /// Waits until the client is connected and initialized. </summary> <param name="client">The IVehicleClient object.</param>
    /// /
    public static void WaitUntilConnectAndInit(this IVehicleClient client)
    {
        client.WaitUntilConnect();
        client.OnInit.Where(_ => _ == InitState.Complete).FirstAsync().Wait();
    }
}

/// <summary>
/// Represents the initialization state of a process.
/// </summary>
public enum InitState
{
    /// <summary>
    /// Represents the initialization state of a process, when waiting for a connection.
    /// </summary>
    WaitConnection,

    /// <summary>
    /// Represents the state of an initialization process where the initialization has failed.
    /// </summary>
    Failed,

    /// <summary>
    /// Represents the current initialization state as being in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// Represents the initialization state of a process.
    /// </summary>
    Complete
}

/// Represents the class of a device.
/// /
public enum DeviceClass
{
    /// <summary>
    /// Represents the unknown device class.
    /// </summary>
    /// <remarks>
    /// This member is used to indicate a device class that is unknown or not defined.
    /// </remarks>
    /// <seealso cref="DeviceClass.Plane"/>
    /// <seealso cref="DeviceClass.Copter"/>
    /// <seealso cref="DeviceClass.GbsRtk"/>
    /// <seealso cref="DeviceClass.SdrPayload"/>
    /// <seealso cref="DeviceClass.Adsb"/>
    Unknown,

    /// <summary>
    /// Represents a device class category of Plane.
    /// </summary>
    Plane,

    /// <summary>
    /// Represents the Copter device class.
    /// </summary>
    Copter,

    /// <summary>
    /// Represents a device of GBS RTK class.
    /// </summary>
    GbsRtk,

    /// <summary>
    /// Represents the class of a device.
    /// </summary>
    SdrPayload,

    /// <summary>
    /// Represents the device class for ADS-B devices.
    /// </summary>
    Adsb,
}