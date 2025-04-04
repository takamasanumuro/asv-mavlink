using Asv.Mavlink.Common;

using R3;

namespace Asv.Mavlink;

/// <summary>
/// Represents a client that can display status text.
/// </summary>
public interface IStatusTextClient: IMavlinkMicroserviceClient
{
    /// <summary>
    /// Gets the editable value for Name.
    /// </summary>
    /// <value>
    /// The editable value for Name.
    /// </value>
    ReactiveProperty<string> DeviceName { get; }

    /// <summary>
    /// Gets the value of the OnMessage property.
    /// </summary>
    /// <remarks>
    /// The OnMessage property is used to obtain a reference to an ReadOnlyReactiveProperty object that emits StatusMessage values.
    /// </remarks>
    /// <returns>
    /// An ReadOnlyReactiveProperty object that emits StatusMessage values.
    /// </returns>
    Observable<StatusMessage> OnMessage { get; }
}

/// <summary>
/// Represents a status message.
/// </summary>
public class StatusMessage
{
    /// <summary>
    /// Gets or sets the sender of the message.
    /// </summary>
    /// <value>
    /// A string representing the sender of the message.
    /// </value>
    public string Sender { get; set; }

    /// <summary>
    /// Gets or sets the text value.
    /// </summary>
    /// <remarks>
    /// This property represents the text value.
    /// </remarks>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the severity type.
    /// </summary>
    /// <value>
    /// A value of type MavSeverity that represents the severity type.
    /// </value>
    public MavSeverity Type { get; set; }
}