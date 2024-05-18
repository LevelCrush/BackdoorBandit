﻿using System;
using System.Reflection;
using Aki.Reflection.Patching;
using BackdoorBandit.SIT;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using StayInTarkov.Networking;

namespace BackdoorBandit.Patches
{
    internal class ActionMenuDoorPatch : ModulePatch
    {

        protected override MethodBase GetTargetMethod() => typeof(GetActionsClass).GetMethod(nameof(GetActionsClass.smethod_10));


        [PatchPostfix]
        public static void Postfix(ref ActionsReturnClass __result, GamePlayerOwner owner, Door door)
        {
            // Add an additional action after the original method executes
            if (__result != null && __result.Actions != null)
            {
                __result.Actions.Add(new ActionsTypesClass
                {
                    Name = "Plant Explosive",
                    Action = new Action(() =>
                    {
                        BackdoorBandit.ExplosiveBreachComponent.StartExplosiveBreach(door, owner.Player);
                        BackdoorBanditPacket packet = new BackdoorBanditPacket(owner.Player.ProfileId);
                        packet.Mode = "C4";
                        packet.DoorID = door.Id;
                        GameClient.SendData(packet.Serialize());
                    }),
                    Disabled = (!door.IsBreachAngle(owner.Player.Position) || !BackdoorBandit.ExplosiveBreachComponent.IsValidDoorState(door) ||
                        !BackdoorBandit.ExplosiveBreachComponent.hasC4Explosives(owner.Player))
                });
            }
        }
    }
}