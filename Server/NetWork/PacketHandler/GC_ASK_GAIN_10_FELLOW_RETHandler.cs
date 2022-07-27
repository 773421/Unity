//This code create by CodeEngine

using System;
namespace SPacket.SocketInstance
{
    public class GC_ASK_GAIN_10_FELLOW_RETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_ASK_GAIN_10_FELLOW_RET packet = (GC_ASK_GAIN_10_FELLOW_RET)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            //if (PartnerFrameLogic_Gamble.Instance())
            //{
            //    //≤•∑≈Ãÿ–ß
            //    if (BackCamerControll.Instance() != null)
            //    {
            //        BackCamerControll.Instance().PlaySceneEffect(137);
            //    }

            //    GameManager.gameManager.SoundManager.PlaySoundEffect(117); //box

            //    PartnerFrameLogic_Gamble.Instance().UpdateMainInfo();
            //    int nCount = packet.fellowidCount;
            //    for (int i = 0; i < nCount; i++)
            //    {
            //        int fellowId = packet.GetFellowid(i);
            //        int fellowSatrLevel = packet.GetFellowstarlevel(i);
            //        UInt64 fellowGuid = packet.GetFellowguid(i);
            //        PartnerFrameLogic_Gamble.Instance().UpdateGainPartner(fellowId, fellowSatrLevel, fellowGuid);
            //    }
            //}

            //enter your logic
            MessageCenter.Ins.Broadcast(MsgID.RefreshPartnerList);
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
