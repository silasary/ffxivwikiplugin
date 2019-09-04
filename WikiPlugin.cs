using Dalamud.Game.Internal.Network;
using Dalamud.Plugin;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Silasary.WikiPlugin
{
    public class WikiPlugin : IDalamudPlugin
    {
        private DalamudPluginInterface dalamud;

        public string Name => "Wiki Plugin";

        public void Dispose()
        {
            dalamud.Framework.Network.OnZonePacket -= processPacket;
        }

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.dalamud = pluginInterface ?? throw new ArgumentNullException(nameof(pluginInterface));
            pluginInterface.Framework.Network.OnZonePacket += processPacket;
            dalamud.Framework.Gui.Chat.Print($"Registered WikiPlugin");
        }

        private void processPacket(IntPtr dataPtr)
        {
            var ptype = Marshal.ReadInt16(dataPtr, 2);
            if (ptype == 120)
            {
                byte[] packet = new byte[64];
                Marshal.Copy(dataPtr, packet, 0, 64);
                byte notifyType = packet[16];
                short contentFinderConditionId = BitConverter.ToInt16(packet, 38);
                Task.Run(async delegate
                {
                    if (notifyType == 4)
                    {
                        var jObject = await XivApi.GetContentFinderCondition(contentFinderConditionId);
                        var name = ((string)jObject["Name"]).Replace(' ', '_');
                        var url = $"https://ffxiv.consolegameswiki.com/wiki/{name}#Bosses";
                        dalamud.Framework.Gui.Chat.Print($"Launching {url}");
                        Process.Start(new ProcessStartInfo(url)
                        {
                            UseShellExecute = true
                        });
                    }
                });
            }
        }
    }
}
