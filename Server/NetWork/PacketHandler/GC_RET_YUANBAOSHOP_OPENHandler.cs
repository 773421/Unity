//This code create by CodeEngine

namespace SPacket.SocketInstance
{
    public class GC_RET_YUANBAOSHOP_OPENHandler : Ipacket
    {
        public uint Execute(PacketDistributed ipacket)
        {
            GC_RET_YUANBAOSHOP_OPEN packet = (GC_RET_YUANBAOSHOP_OPEN)ipacket;
            if (null == packet) return (uint)PACKET_EXE.PACKET_EXE_ERROR;
            //enter your logic

            if (packet.IsOpen == 1)
            {
                //UIManager.ShowUI(UIInfo.YuanBaoShop, YuanBaoShopLogic.OnYuanBaoShopLoad, packet.IsShowBlackMarket == 1 ? true : false);
            }
            //else
            //{
            //    if (Singleton<ObjManager>.Instance.MainPlayer)
            //    {
            //        Singleton<ObjManager>.Instance.MainPlayer.SendNoticMsg(false, "#{2167}");
            //    }
            //}
            return (uint)PACKET_EXE.PACKET_EXE_CONTINUE;
        }
    }
}
