using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace User.PluginSdkDemo
{
    internal sealed class GoveeUdpSender : IDisposable
    {
        private readonly UdpClient client;
        private readonly IPEndPoint endpoint;

        public GoveeUdpSender(string ipAddress, int port = 4003)
        {
            endpoint = new IPEndPoint(
                IPAddress.Parse(ipAddress), port);

            client = new UdpClient();
        }

        public async Task RunBlinkTestAsync(
            CancellationToken cancellationToken)
        {
            // Enable the Govee Razer-compatible control mode.
            await SendPayloadAsync(
                new byte[] { 0xBB, 0x00, 0x01, 0xB1, 0x01, 0x0A });

            await Task.Delay(1000, cancellationToken);

            // Eight alternating ON/OFF phases.
            for (int phase = 0; phase < 8; phase++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                byte[] colors = new byte[18];
                bool on = phase % 2 == 0;

                for (int zone = 0; zone < 6; zone++)
                {
                    int offset = zone * 3;

                    colors[offset] = 0;                  // Red
                    colors[offset + 1] = on ? (byte)255 : (byte)0;
                    colors[offset + 2] = 0;              // Blue
                }

                await SendPayloadAsync(BuildColorPacket(colors));
                await Task.Delay(500, cancellationToken);
            }

            // Leave all six zones blue after the test.
            byte[] blue = new byte[18];

            for (int zone = 0; zone < 6; zone++)
            {
                blue[zone * 3 + 2] = 255;
            }

            await SendPayloadAsync(BuildColorPacket(blue));
        }
        // ALTERNATING FLASHING YELLOW
        // Activates during an active caution and alternates the left and right
        // groups of three LED zones using a 110 ms flash interval.
        // The effect must stop when yellow clears or a higher-priority flag
        // (such as green or checkered) takes control.
        // When cancelled, the final block sends an all-black frame so the
        // last yellow frame is not intentionally left illuminated.
        public async Task RunAlternatingFlashingYellowAsync(
     CancellationToken cancellationToken)
        {
            try
            {
                // Enable the Govee control mode.
                await SendPayloadAsync(
                    new byte[] { 0xBB, 0x00, 0x01, 0xB1, 0x01, 0x0A });

                await Task.Delay(1000, cancellationToken);

                var timer = System.Diagnostics.Stopwatch.StartNew();

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int phase = (int)(timer.ElapsedMilliseconds % 1320);

                    bool flashing = (phase % 220) < 110;
                    bool leftOn = phase < 660 && flashing;
                    bool rightOn = phase >= 660 && flashing;

                    byte[] colors = new byte[18];

                    for (int zone = 0; zone < 6; zone++)
                    {
                        bool on = zone < 3 ? leftOn : rightOn;

                        if (on)
                        {
                            int offset = zone * 3;
                            colors[offset] = 255;
                            colors[offset + 1] = 255;
                            colors[offset + 2] = 0;
                        }
                    }

                    await SendPayloadAsync(BuildColorPacket(colors));
                    await Task.Delay(50, cancellationToken);
                }
            }
            finally
            {
                // Clear the last yellow frame when this routine stops.
                await SendPayloadAsync(BuildColorPacket(new byte[18]));
            }
        }

          // ALTERNATING CHECKERED CHASE
         // Slides a white-black-white pattern from left to right
         // across all six zones. Each step lasts 150 ms.
         // Cancellation sends an all-black frame so the chase
         // does not intentionally leave the last frame illuminated.
        public async Task RunAlternatingCheckeredChaseAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                // Activate the Govee local control mode.
                await SendPayloadAsync(
                    new byte[] { 0xBB, 0x00, 0x01, 0xB1, 0x01, 0x0A });

                await Task.Delay(1000, cancellationToken);

                int position = 0;

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Start with all six zones black.
                    byte[] colors = new byte[18];

                    // Position 1: white.
                    int firstZone = position;
                    colors[firstZone * 3] = 255;
                    colors[firstZone * 3 + 1] = 255;
                    colors[firstZone * 3 + 2] = 255;

                    // Position 2 stays black.
                    // Position 3: white.
                    int thirdZone = (position + 2) % 6;
                    colors[thirdZone * 3] = 255;
                    colors[thirdZone * 3 + 1] = 255;
                    colors[thirdZone * 3 + 2] = 255;

                    await SendPayloadAsync(BuildColorPacket(colors));

                    // Slide one zone to the right every 150 ms.
                    await Task.Delay(150, cancellationToken);

                    position = (position + 1) % 6;
                }
            }
            finally
            {
                // Turn off the chase frame when the effect stops.
                await SendPayloadAsync(BuildColorPacket(new byte[18]));
            }
        }
        private static byte[] BuildColorPacket(byte[] colors)
        {
            if (colors == null || colors.Length != 18)
                throw new ArgumentException(
                    "Exactly six RGB zones are required.", nameof(colors));

            byte[] packet = new byte[25];

            packet[0] = 0xBB;
            packet[1] = 0x00;
            packet[2] = 0x14;
            packet[3] = 0xB0;
            packet[4] = 0x00;
            packet[5] = 0x06;

            Array.Copy(colors, 0, packet, 6, colors.Length);

            byte checksum = 0;

            for (int i = 0; i < packet.Length - 1; i++)
                checksum ^= packet[i];

            packet[packet.Length - 1] = checksum;

            return packet;
        }

        private async Task SendPayloadAsync(byte[] payload)
        {
            string json =
                "{\"msg\":{\"cmd\":\"razer\",\"data\":{\"pt\":\"" +
                Convert.ToBase64String(payload) +
                "\"}}}";

            byte[] data = Encoding.UTF8.GetBytes(json);

            await client.SendAsync(data, data.Length, endpoint);
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}