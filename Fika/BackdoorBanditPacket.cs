using System;
using System.IO;
using System.Linq;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using Fika.Core.Coop.Components;
using Fika.Core.Coop.GameMode;
using Fika.Core.Coop.Utils;
using Fika.Core.Networking;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace BackdoorBandit.Fika
{
    public class BackdoorBanditPacket: INetSerializable
    {
        public static NetDataWriter netwriter = null;
        

  
        public string Mode { get; set;  }
        public string DoorID { get; set;  }
        
        public int PlayerID { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(PlayerID);
            writer.Put(Mode);
            writer.Put(DoorID);
        }

        public void Deserialize(NetDataReader reader)
        {
            PlayerID = reader.GetInt();
            Mode = reader.GetString();
            DoorID = reader.GetString();
        }


        public static void Send( BackdoorBanditPacket packet)
        {
            if (netwriter == null)
            {
                netwriter = new NetDataWriter();
            }
            
            netwriter.Reset();
            if (FikaBackendUtils.IsServer)
            {
                FikaLogger.Write($"{nameof(BackdoorBanditPacket)}: sending packet out via server");
                Singleton<FikaServer>.Instance.SendDataToAll(netwriter, ref packet, LiteNetLib.DeliveryMethod.ReliableUnordered);
            }
            else
            {
                FikaLogger.Write($"{nameof(BackdoorBanditPacket)}: sending packet out via client");
                Singleton<FikaClient>.Instance.SendData(netwriter, ref packet, LiteNetLib.DeliveryMethod.ReliableUnordered);
            }
        }
        
        public static void Send( BackdoorBanditPacket packet, NetPeer peer)
        {
            if (netwriter == null)
            {
                netwriter = new NetDataWriter();
            }
            
            netwriter.Reset();
            Singleton<FikaServer>.Instance.SendDataToAll(netwriter, ref packet, LiteNetLib.DeliveryMethod.ReliableUnordered, peer);

        }

        public static  void Process(INetSerializable data)
        {
            var packet = data as BackdoorBanditPacket;
            
            switch (packet.Mode)
            {
                case "C4":
                    var coopHandler = CoopHandler.GetCoopHandler();
                    FikaLogger.Write($"{nameof(BackdoorBanditPacket)}: Finding door {packet.DoorID}");

                    var door = Singleton<GameWorld>.Instance.World_0.FindDoor(packet.DoorID) as Door;
                 //   var door =   coopHandler.ListOfInteractiveObjects.First(x => x.Value.Id == packet.DoorID).Value as Door;
                    var player = coopHandler.Players.First(x => x.Value.NetId == packet.PlayerID).Value;
                    FikaLogger.Write($"{nameof(BackdoorBanditPacket)}: Having Player {player.Profile.Nickname} place C4");
                    BackdoorBandit.ExplosiveBreachComponent.StartExplosiveBreach(door, player);
                    break;
                default:
                    FikaLogger.Write($"{nameof(BackdoorBanditPacket)}: Unsupported mode \"{packet.Mode}");
                    break;
            }
            
        }

        public static void ProcessServer(INetSerializable data, NetPeer peer)
        {
            BackdoorBanditPacket.Process(data);
            BackdoorBanditPacket.Send(data as BackdoorBanditPacket, peer);
        }
    }
}