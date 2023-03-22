using System;
using Asv.IO;

namespace Asv.Mavlink
{
    public interface IPayload:ISpanSerializable
    {
        /// <summary>
        /// Maximum size of payload
        /// </summary>
        byte GetMaxByteSize();
        // <summary>
        /// Minimum size of payload
        /// </summary>
        byte GetMinByteSize();
    }
}
