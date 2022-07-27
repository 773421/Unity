//This code create by CodeEngine

namespace SPacket.SocketInstance
{
    public class GC_FELLOW_ENCHANCE_RETHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_FELLOW_ENCHANCE_RET packet = (GC_FELLOW_ENCHANCE_RET)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
