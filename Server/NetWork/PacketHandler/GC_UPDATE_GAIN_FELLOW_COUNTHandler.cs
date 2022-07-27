//This code create by CodeEngine

namespace SPacket.SocketInstance
{
    public class GC_UPDATE_GAIN_FELLOW_COUNTHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_UPDATE_GAIN_FELLOW_COUNT packet = (GC_UPDATE_GAIN_FELLOW_COUNT)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;

            int fellowGainCount_Coin = packet.NormalCount;
            int fellowGainCount_YuanBao = packet.YuanbaoCount;
            int fellowGainCD_Coin = packet.NormalCD;
            int fellowGainCount_Free = packet.FreeCount;

            if (fellowGainCount_Coin >= 0 && fellowGainCount_YuanBao >= 0)
            {
                //GameManager.gameManager.PlayerDataPool.FellowGainCount_Free = fellowGainCount_Free;
                //GameManager.gameManager.PlayerDataPool.FellowGainCount_Coin = fellowGainCount_Coin;
                //GameManager.gameManager.PlayerDataPool.FellowGainCount_YuanBao = fellowGainCount_YuanBao;
                //GameManager.gameManager.PlayerDataPool.FellowGainCD_Coin = fellowGainCD_Coin;
            }

            //enter your logic
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
