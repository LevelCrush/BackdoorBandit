using System;
using System.Reflection;
using BackdoorBandit.Fika;
using SPT.Reflection.Patching;
using EFT;
using EFT.Interactive;
using Fika.Core.Coop.Components;

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