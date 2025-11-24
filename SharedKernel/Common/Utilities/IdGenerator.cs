using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Common.Utilities
{
    public static class IdGenerator
    {
        private static readonly long _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        private static readonly Random _random = new Random();
        private static long _lastTimestamp = -1L;
        private static long _sequence = 0L;

        public static Guid Generate()
        {
            var timestamp = DateTime.UtcNow.Ticks - _epoch;

            if (timestamp < _lastTimestamp)
            {
                timestamp = _lastTimestamp;
            }

            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & 4095;
                if (_sequence == 0)
                {
                    timestamp = WaitNextMillis(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;

            var time = ((timestamp << 22) >> 22);
            var nodeId = (ushort)_random.Next(0, 1024);
            var seq = (ushort)_sequence;

            var bytes = new byte[16];
            Array.Copy(BitConverter.GetBytes(time), 0, bytes, 0, 6);
            Array.Copy(BitConverter.GetBytes(nodeId), 0, bytes, 6, 2);
            Array.Copy(BitConverter.GetBytes(seq), 0, bytes, 8, 2);

            return new Guid(bytes);
        }

        private static long WaitNextMillis(long lastTimestamp)
        {
            var timestamp = DateTime.UtcNow.Ticks - _epoch;
            while (timestamp <= lastTimestamp)
            {
                timestamp = DateTime.UtcNow.Ticks - _epoch;
            }
            return timestamp;
        }
    }
}
