//This code create by CodeEngine

using System;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
 {
     public class GC_ASK_GAIN_FELLOW_RETHandler : Ipacket
     {
         public uint Execute(PacketDistributed ipacket)
         {
             GC_ASK_GAIN_FELLOW_RET packet = (GC_ASK_GAIN_FELLOW_RET)ipacket;
             if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

             int fellowId = packet.Fellowid;
             int fellowSatrLevel = packet.Fellowstarlevel;
             UInt64 fellowGuid = packet.Fellowguid;
            UnityEngine.Debug.Log("抽取成功");
             //if (PartnerFrameLogic_Gamble.Instance())
             //{
             //    //播放特效
             //    if (BackCamerControll.Instance() != null)
             //    {
             //        BackCamerControll.Instance().PlaySceneEffect(137);
             //    }

             //    GameManager.gameManager.SoundManager.PlaySoundEffect(117);  //box

             //    PartnerFrameLogic_Gamble.Instance().UpdateMainInfo();
             //    PartnerFrameLogic_Gamble.Instance().UpdateGainPartner(fellowId, fellowSatrLevel, fellowGuid);
             //}

             //enter your logic
             return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
         }
     }
 }
