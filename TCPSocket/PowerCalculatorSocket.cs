using DataTypes.Data;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Domain.PowerMeter;

namespace Power.Calculator.UdpSocket
{
    using MeanPowerTimestamped = IReadOnlyMeanPowerCollection<TimestampedCollection<double>>;
    public class PowerCalculatorSocket : IDisposable
    {
        private readonly IPEndPoint endPoint;
        private readonly Socket listenSocket;
        private readonly IMeterEntity meterEntity;
        private readonly MeanPowerTimestamped calculator;
        private bool disposedValue;

        public PowerCalculatorSocket(
            IPEndPoint endPoint,
            IMeterEntity meterEntity,
            MeanPowerTimestamped calculator
            )
        {
            this.endPoint = endPoint;
            this.meterEntity = meterEntity;
            this.calculator = calculator;
            this.listenSocket = new(
                this.endPoint.AddressFamily,
                SocketType.Dgram,
                ProtocolType.Udp
            );
            this.listenSocket.Bind(this.endPoint);
        }

        public void StartServer()
        {
            this.meterEntity.NewMeterCollectionAvailable += this.StartBroadcast;
        }

        /* public async Task StartServer(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                this.listenSocket.Listen(20);
                var handler = await this.listenSocket.AcceptAsync();
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
            }
        } */

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.listenSocket.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void StartBroadcast(object sender, EventArgs e)
        {
            var tmp = this.MakePacket();
            string cnslstring = JsonConvert.SerializeObject(tmp);
            Console.WriteLine(cnslstring);
        }

        private ServerPacketPowerMeter MakePacket()
        {
            return this.calculator.WrapServerPacket();
        }
    }
}