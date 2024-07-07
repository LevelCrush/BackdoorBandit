﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aki.Reflection.Patching;
using BackdoorBandit.Fika;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using Fika.Core.Coop.Components;
using Fika.Core.Coop.Matchmaker;
using Fika.Core.Networking;


namespace BackdoorBandit.Patches
{
    internal class ActionMenuKeyCardPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() => typeof(GetActionsClass).GetMethod(nameof(GetActionsClass.smethod_9));


        // Check if an action is already added. Hopefully door's action takes precedence
        public static bool IsActionAdded(List<ActionsTypesClass> actions, string actionName)
        {
            return actions.Any(action => action.Name.ToLower() == actionName.ToLower());
        }

        [PatchPostfix]
        public static void Postfix(ref ActionsReturnClass __result, GamePlayerOwner owner, Door door)
        {
            if (__result != null && __result.Actions != null && !IsActionAdded(__result.Actions, "Plant Explosive"))
            {
                __result.Actions.Add(new ActionsTypesClass
                {
                    Name = "Plant Explosive",
                    Action = new Action(() =>
                    {
                        var coopHandler = CoopHandler.GetCoopHandler();
                        BackdoorBandit.ExplosiveBreachComponent.StartExplosiveBreach(door, owner.Player);
                        BackdoorBanditPacket packet = new BackdoorBanditPacket();
                        packet.PlayerID = coopHandler.MyPlayer.NetId;
                        packet.Mode = "C4";
                        packet.DoorID = door.Id;
                        BackdoorBanditPacket.Send(packet);
                    }),
                    Disabled = (!door.IsBreachAngle(owner.Player.Position) || !BackdoorBandit.ExplosiveBreachComponent.IsValidDoorState(door) ||
                                !BackdoorBandit.ExplosiveBreachComponent.hasC4Explosives(owner.Player))
                });
            }
        }
    }
}
