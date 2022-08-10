//This code create by CodeEngine

using System;
 using Google.ProtocolBuffers;
 using System.Collections;
namespace SPacket.SocketInstance
{
    public class GC_UPDATE_FELLOWHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UPDATE_FELLOW packet = (GC_UPDATE_FELLOW)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //ȡ�û���������е�����
            int index = packet.Packindex;
            FellowContainer container = FellowContainer.Ins;
            if (container != null)
            {
                Fellow fellow = new Fellow();
                if (fellow != null)
                {
                    //GUID
                    fellow.Guid = packet.Guid;
                    //ID
                    fellow.DataId = packet.Dataid;
                    //�ǳ�
                    fellow.Name = packet.Name;
                    //����
                    if (packet.HasExp)
                    {
                        fellow.Exp = packet.Exp;
                    }
                    else
                    {
                        fellow.Exp = 0;
                    }
                    //Ʒ��
                    fellow.Quality = packet.Quality;
                    //����
                    if (packet.HasLevel)
                    {
                        fellow.Level = packet.Level;
                    }
                    else
                    {
                        fellow.Level = 1;
                    }
                    //����
                    if (packet.HasIslock)
                    {
                        fellow.Locked = (packet.Islock == 1) ? true : false;
                    }
                    else
                    {
                        fellow.Locked = false;
                    }
                    //���ǵȼ�
                    if (packet.HasStarlevel)
                    {
                        fellow.StarLevel = packet.Starlevel;
                    }
                    else
                    {
                        fellow.StarLevel = 0;
                    }
                    //���ʵ�
                    if (packet.HasZzpoint)
                    {
                        fellow.ZzPoint = packet.Zzpoint;
                    }
                    else
                    {
                        fellow.ZzPoint = 0;
                    }
                    fellow.Zizhi_Attack = (float)(packet.Zizhi_attack) / 100.0f;
                    fellow.Zizhi_Hit = (float)(packet.Zizhi_hit) / 100.00f;
                    fellow.Zizhi_Critical = (float)(packet.Zizhi_critical) / 100.00f;
                    fellow.Zizhi_Guard = (float)(packet.Zizhi_guard) / 100.00f;
                    fellow.Zizhi_Bless = (float)(packet.Zizhi_bless) / 100.00f;
                    int count = packet.skillIdCount;
                    for (int nIndex = 0; nIndex < count && nIndex < Fellow.FELLOW_MAXOWNSKILL; nIndex++)
                    {
                        fellow.SetOwnSkillId(packet.GetSkillId(nIndex), nIndex);
                    }
                    fellow.CombatAttr_Attack = packet.Attr_attack;
                    fellow.CombatAttr_Hit = packet.Attr_hit;
                    fellow.CombatAttr_Critical = packet.Attr_critical;
                    fellow.CombatAttr_Guard = packet.Attr_guard;
                    fellow.CombatAttr_Bless = packet.Attr_bless;
                    FellowContainer.Ins.list.Add(fellow);
                    //if (PartnerFrameLogic.Instance() != null)
                    //{
                    //    PartnerFrameLogic.Instance().SetUpdateUI();
                    //}
                    
				}
            }
            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
