using DataTypes.Data;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Domain.PowerMeter;
using Serilog;

namespace Power.Calculator.UdpSocket
{
    using MeanPowerTimestamped = IReadOnlyMeanPowerCollection<TimestampedCollection<double>>;
    public class PowerCalculatorSocket : IDisposable
    {
        private readonly IPEndPoint endPoint;
        private readonly Socket serverSocket;
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
            this.serverSocket = new(
                this.endPoint.AddressFamily,
                SocketType.Dgram,
                ProtocolType.Udp
            );
        }

        public void StartServer()
        {
            Log.Information($"Server running at {this.endPoint.Address}:{this.endPoint.Port}");
            this.meterEntity.NewMeterCollectionAvailable += this.StartBroadcast;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.serverSocket.Dispose();
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
            byte[] buffer = Encoding.ASCII.GetBytes(cnslstring);
            this.serverSocket.SendTo(buffer, this.endPoint);
            Log.Debug($"UDP packet on {this.endPoint.Address}:{this.endPoint.Port} sent.");
        }

        private ServerPacketPowerMeter MakePacket()
        {
            return this.calculator.WrapServerPacket();
        }
    }
}