using System.Threading;
using System.Threading.Tasks;
using Asv.Mavlink.Common;

using R3;

namespace Asv.Mavlink;

/// <summary>
/// Represents a client for accessing position-related data and commands for a vehicle.
/// </summary>
public interface IPositionClient: IMavlinkMicroserviceClient
{
    /// <summary>
    /// Gets the RX value for GlobalPosition of type GlobalPositionIntPayload.
    /// </summary>
    /// <remarks>
    /// This property represents the current global position.
    /// </remarks>
    ReadOnlyReactiveProperty<GlobalPositionIntPayload?> GlobalPosition { get; }

    /// <summary>
    /// Gets the home position of the property Home.
    /// </summary>
    /// <value>
    /// The home position represented by an ReadOnlyReactiveProperty of HomePositionPayload.
    /// </value>
    ReadOnlyReactiveProperty<HomePositionPayload?> Home { get; }

    /// <summary>
    /// Gets the target position value.
    /// </summary>
    /// <returns>
    /// The ReactiveProperty object that represents the target position value.
    /// </returns>
    ReadOnlyReactiveProperty<PositionTargetGlobalIntPayload?> Target { get; }

    /// <summary>
    /// Gets the observable altitude value.
    /// </summary>
    /// <value>
    /// The observable altitude value.
    /// </value>
    ReadOnlyReactiveProperty<AltitudePayload?> Altitude { get; }

    /// <summary>
    /// Gets the ReactiveProperty for VfrHud.
    /// </summary>
    /// <remarks>
    /// The VfrHud property provides access to an ReadOnlyReactiveProperty interface for VfrHudPayload, which represents the
    /// information received from a VFR (Visual Flight Rules) Heads-Up Display (HUD).
    /// </remarks>
    /// <returns>The ReadOnlyReactiveProperty interface for VfrHudPayload.</returns>
    ReadOnlyReactiveProperty<VfrHudPayload?> VfrHud { get; }

    /// <summary>
    /// Gets the reactive value representing the high resolution IMU payload.
    /// </summary>
    /// <value>
    /// The reactive value holding the high resolution IMU payload.
    /// </value>
    ReadOnlyReactiveProperty<HighresImuPayload?> Imu { get; }

    /// <summary>
    /// Gets the RX value of the current Attitude payload.
    /// </summary>
    /// <returns>The RX value of the Attitude payload.</returns>
    ReadOnlyReactiveProperty<AttitudePayload?> Attitude { get; }

    /// <summary>
    /// Sets a desired vehicle position, velocity, and/or acceleration in a global coordinate system (WGS84). Used by an external controller to command the vehicle (manual controller or
    /// other system).
    /// </summary>
    /// <param name="timeBootMs">Timestamp (time since system boot). The rationale for the timestamp in the setpoint is to allow the system to compensate for the transport delay of the setpoint. This allows the system to compensate processing latency.</param>
    /// <param name="coordinateFrame">Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11</param>
    /// <param name="latInt">X Position in WGS84 frame</param>
    /// <param name="lonInt">Y Position in WGS84 frame</param>
    /// <param name="alt">Altitude (MSL, Relative to home, or AGL - depending on frame)</param>
    /// <param name="vx">X velocity in NED frame</param>
    /// <param name="vy">Y velocity in NED frame</param>
    /// <param name="vz">Z velocity in NED frame</param>
    /// <param name="afx">X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N</param>
    /// <param name="afy">Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N</param>
    /// <param name="afz">Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N</param>
    /// <param name="yaw">yaw setpoint</param>
    /// <param name="yawRate">yaw rate setpoint</param>
    /// <param name="typeMask">Bitmap to indicate which dimensions should be ignored by the vehicle.</param>
    /// <param name="cancel">Cancellation token</param>
    /// <param name="coordFrame">Valid options are: MAV_FRAME_GLOBAL_INT = 5, MAV_FRAME_GLOBAL_RELATIVE_ALT_INT = 6, MAV_FRAME_GLOBAL_TERRAIN_ALT_INT = 11</param>
    /// <returns>Returns a Task representing the asynchronous operation</returns>
    ValueTask SetTargetGlobalInt(
        uint timeBootMs,
        MavFrame coordinateFrame,
        int latInt,
        int lonInt,
        float alt,
        float vx,
        float vy,
        float vz,
        float afx,
        float afy,
        float afz,
        float yaw,
        float yawRate,
        PositionTargetTypemask typeMask,
        CancellationToken cancel = default
    );

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeBootMs">Timestamp (time since system boot).</param>
    /// <param name="coordinateFrame"> Valid options are: MAV_FRAME_LOCAL_NED = 1, MAV_FRAME_LOCAL_OFFSET_NED = 7, MAV_FRAME_BODY_NED = 8, MAV_FRAME_BODY_OFFSET_NED = 9</param>
    /// <param name="typeMask">Bitmap to indicate which dimensions should be ignored by the vehicle.</param>
    /// <param name="x">X Position in NED frame</param>
    /// <param name="y">Y Position in NED frame</param>
    /// <param name="z">Z Position in NED frame (note, altitude is negative in NED)</param>
    /// <param name="vx">X velocity in NED frame</param>
    /// <param name="vy">Y velocity in NED frame</param>
    /// <param name="vz">Z velocity in NED frame</param>
    /// <param name="afx">X acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N</param>
    /// <param name="afy">Y acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N</param>
    /// <param name="afz">Z acceleration or force (if bit 10 of type_mask is set) in NED frame in meter / s^2 or N</param>
    /// <param name="yaw">yaw setpoint</param>
    /// <param name="yawRate">yaw rate setpoint</param>
    /// <param name="cancel"></param>
    /// <returns></returns>
    ValueTask SetPositionTargetLocalNed(
        uint timeBootMs, 
        MavFrame coordinateFrame, 
        PositionTargetTypemask typeMask,
        float x,
        float y, 
        float z, 
        float vx, 
        float vy, 
        float vz, 
        float afx, 
        float afy, 
        float afz, 
        float yaw, 
        float yawRate,
        CancellationToken cancel
    );
}