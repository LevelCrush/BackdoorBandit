using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BackdoorBandit.SIT;
using StayInTarkov;
using StayInTarkov.Coop.SITGameModes;

namespace BackdoorBandit.Patches
{
    public class BackdoorBanditNetworkPacketPatch: ModulePatch
    { protected override MethodBase GetTargetMethod()
        {
            return typeof(CoopSITGame).GetMethod("CreateExfiltrationPointAndInitDeathHandler", BindingFlags.Public | BindingFlags.Instance);
        }
    
       

        [PatchPostfix]
        public static  void PatchPostfix()
        { 
            
            StayInTarkovHelperConstants.Logger.LogInfo("Trying to patch in BackdoorBanditPacket");
            var sit_types =
                typeof(StayInTarkovHelperConstants).GetField("_sitTypes", BindingFlags.Static | BindingFlags.NonPublic);
           
            StayInTarkovHelperConstants.Logger.LogInfo($"Backdoor bandit is patching in BackdoorBanditPacket");
            var new_types = new List<Type>();
            new_types.Add(typeof(BackdoorBanditPacket));
            var merged = StayInTarkovHelperConstants.SITTypes.Union(new_types).ToArray();
            sit_types.SetValue(null, merged);
            StayInTarkovHelperConstants.Logger.LogInfo("Backdoor bandit is finished patching BackdoorBanditPacket");
                  
           
        }
    }
}