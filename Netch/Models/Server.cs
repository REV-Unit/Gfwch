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
            var delay = -1;
            try
            {
                var addresses = await Dns.GetHostAddressesAsync(Hostname);
                var ip = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                if (ip == null)
                {
                    delay = -2;
                }

                using var ping = new Ping();
                var pingReply = await ping.SendPingAsync(ip);
                if (pingReply.Status == IPStatus.Success)
                {
                    delay = (int) pingReply.RoundtripTime;
                }
            }
            catch (Exception)
            {
                delay = -666;
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