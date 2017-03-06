using System;
using Google.Protobuf;

namespace EventStore.RPC.Server
{
    public static class ByteStringExtensions
    {
        public static Guid ToGuid(this ByteString byteString)
        {
            return new Guid(byteString.ToByteArray());
        }
    }
}