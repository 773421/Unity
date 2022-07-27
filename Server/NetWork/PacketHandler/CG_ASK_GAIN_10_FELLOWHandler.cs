//This code create by CodeEngine

using UnityEngine;

namespace SPacket.SocketInstance
{
    public class CG_ASK_GAIN_10_FELLOWHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            CG_ASK_GAIN_10_FELLOW packet = (CG_ASK_GAIN_10_FELLOW)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
