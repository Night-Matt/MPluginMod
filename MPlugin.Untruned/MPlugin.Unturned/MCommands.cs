using MPlugin.Untruned.MPlugin.API;
using MPlugin.Untruned.MPlugin.Core;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SDG.Provider.SteamGetInventoryResponse;

namespace MPlugin.Untruned.MPlugin.Unturned
{
    public class CommandI : IMCommand
    {
        public string Name => "mi";

        public string Permission => "";

        public ECommandAllowType AllowType => ECommandAllowType.Player;

        public bool Excute(string[] parameters, MPlayer player)
        {
            ushort itemId;
            byte amount = 1;
            if (parameters.Length >= 1)
            {
                if (parameters.Length >= 2) byte.TryParse(parameters[1], out amount);
                if (ushort.TryParse(parameters[0], out itemId))
                {
                    if (!ItemTool.tryForceGiveItem(player.Player, itemId, amount))
                    {
                        MSay.Say($"Failed with an error.", player.SteamId, Color.red);
                        return false;
                    }
                    else
                    {
                        MSay.Say($"Successfully obtained item {amount}x {itemId}.", player.SteamId, Color.yellow);
                        return true;
                    }
                }
                else return false;
            }
            else return false;
        }
    }

    public class CommandV : IMCommand
    {
        public string Name => "mv";

        public string Permission => "";

        public ECommandAllowType AllowType => ECommandAllowType.Player;

        public bool Excute(string[] parameters, MPlayer player)
        {
            ushort vehicleId;
            if (parameters.Length >= 1)
            {
                if (ushort.TryParse(parameters[0], out vehicleId))
                {
                    if (!VehicleTool.giveVehicle(player.Player, vehicleId))
                    {
                        MSay.Say($"Failed with an error.", player.SteamId, Color.red);
                        return false;
                    }
                    else
                    {
                        MSay.Say($"Successfully obtained vehicle: {vehicleId}.", player.SteamId, Color.yellow);
                        return true;
                    }
                }else return false;
            }
            else
            {
                return false;
            }
        }
    }

    public class CommandDay : IMCommand
    {
        public string Name => "mday";

        public string Permission => "";

        public ECommandAllowType AllowType => ECommandAllowType.Both;

        public bool Excute(string[] parameters, MPlayer player)
        {
            LightingManager.time = (uint)(LightingManager.cycle * LevelLighting.transition);
            return true;
        }
    }
    public class CommandPermissions : IMCommand
    {
        public string Name => "mp";

        public string Permission => "";

        public ECommandAllowType AllowType => ECommandAllowType.Player;

        public bool Excute(string[] parameters, MPlayer player)
        {
            string msg = "";
            List<string> permissions = MPluginManager.Instance.GetPermissions(player);
            foreach (var v in permissions)
            {
                msg += v + ",";
            }
            msg = msg.Remove(msg.Length - 1, 1);
            MSay.Say($"List of permissions you have: {msg}", player.SteamId, Color.green);
            return true;
        }
    }
}
