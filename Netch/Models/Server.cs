using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Netch.Utils;

namespace Netch.Models
{
    public class Server:ICloneable
    {
        /// <summary>
        ///     备注
        /// </summary>
        public string Remark;

        /// <summary>
        ///     组
        /// </summary>
        public string Group = "None";

        /// <summary>
        ///     代理类型
        /// </summary>
        public string Type;

        /// <summary>
        ///     倍率
        /// </summary>
        public double Rate = 1.0;

        /// <summary>
        ///     地址
        /// </summary>
        public string Hostname;

        /// <summary>
        ///     端口
        /// </summary>
        public ushort Port;

        /// <summary>
        ///     延迟
        /// </summary>
        public int Delay = -1;

        /// <summary>
        ///		获取备注
        /// </summary>
        /// <returns>备注</returns>
        public override string ToString()
        {
            var remark = string.IsNullOrWhiteSpace(Remark) ? $"{Hostname}:{Port}" : Remark;

            if (Group.Equals("None") || Group.Equals(""))
                Group = "NONE";

            return $"[{ServerHelper.GetUtilByTypeName(Type)?.ShortName ?? "WTF"}][{Group}] {remark}";
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        ///		测试延迟
        /// </summary>
        /// <returns>延迟</returns>
        public async Task Test()
        {
            static async Task<int> GetDelay(IPAddress ip)
            {
                if (ip == null)
                {
                    return -2;
                }

                var tasks = new Task<PingReply>[3];
                for (var i = 0; i < 3; i++)
                {
                    using var ping = new Ping();
                    tasks[i] = ping.SendPingAsync(ip);
                }

                var replies = await Task.WhenAll(tasks);

                if (replies.Any(reply => reply.Status != IPStatus.Success))
                {
                    return -1;
                }

                return replies.Select(reply => (int)reply.RoundtripTime).Sum() / 3;
            }

            var delay = 0;
            try
            {
                var addresses = await Dns.GetHostAddressesAsync(Hostname);
                delay = await GetDelay(addresses.FirstOrDefault());
            }
            finally
            {
                Delay = delay;
            }
        }
    }

    public static class ServerExtension
    {
        public static string AutoResolveHostname(this Server server)
        {
            return Global.Settings.ResolveServerHostname ? DNS.Lookup(server.Hostname).ToString() : server.Hostname;
        }
    }
}