using System.IO;
using System.Linq;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using StayInTarkov;
using StayInTarkov.Coop.Components.CoopGameComponents;
using StayInTarkov.Coop.NetworkPacket;
using StayInTarkov.Coop.NetworkPacket.Player;
using StayInTarkov.Coop.Players;
using StayInTarkov.Coop.SITGameModes;
using StayInTarkov.Networking;
using UnityEngine;

namespace BackdoorBandit.SIT
{
    public class BackdoorBanditPacket: BasePlayerPacket
    {
        
        public string Mode { get; set;  }
        public string DoorID { get; set;  }
        public BackdoorBanditPacket()
        {
            
        }


        public BackdoorBanditPacket(string profileId) : base(new string(profileId.ToCharArray()), nameof(BackdoorBanditPacket))
        {
            
        }
        
        public override byte[] Serialize()
        {
            StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}:Trying to serialize");
            var ms = new MemoryStream();

            StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}:Creating Binary Writer");
            using var writer = new BinaryWriter(ms);

            StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}:Writing Header");
            WriteHeaderAndProfileId(writer);

            StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}:{Mode}");
            writer.Write(Mode);
            StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}:{DoorID}");
            writer.Write(DoorID);

            StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}:Done setting");

            return ms.ToArray();
        }

        public override ISITPacket Deserialize(byte[] bytes)
        {
            StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}:Creating Binary Reader");
            using var reader = new BinaryReader(new MemoryStream(bytes));
            ReadHeaderAndProfileId(reader);

            Mode = reader.ReadString();
            StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}:{Mode} is set");
            
            DoorID = reader.ReadString();
            StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}:{DoorID} is set");

            return this;
        }

        protected override async void Process(CoopPlayerClient client)
        {
            StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}: Processing Fire Support Packet");

            if (client.GetPlayer.IsYourPlayer)
            {
                StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}: Ignoring own packet");
                return;
            }
            switch (Mode)
            {
                case "C4":
                    StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}: Finding door {DoorID}");
                    var door = SITGameComponent.GetCoopGameComponent().ListOfInteractiveObjects
                        .FirstOrDefault(x => x.Id == DoorID) as Door;
                    
                    var player = SITGameComponent.GetCoopGameComponent().Players[ProfileId];
                    
                    StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}: Having Player {player.Profile.Nickname} place C4");
                    BackdoorBandit.ExplosiveBreachComponent.StartExplosiveBreach(door, player as Player);
                    break;
                default:
                    StayInTarkovHelperConstants.Logger.LogInfo($"{nameof(BackdoorBanditPacket)}: Unsupported Mode: {Mode}");
                    break;
            }
        }
    }
}