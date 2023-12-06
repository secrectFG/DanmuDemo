using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMDataDefine
{
    public class BatchComboSend
    {
        public string action { get; set; }
        public string batch_combo_id { get; set; }
        public int batch_combo_num { get; set; }
        public object blind_gift { get; set; }
        public int gift_id { get; set; }
        public string gift_name { get; set; }
        public int gift_num { get; set; }
        public object send_master { get; set; }
        public int uid { get; set; }
        public string uname { get; set; }
    }

    public class ComboSend
    {
        public string action { get; set; }
        public string combo_id { get; set; }
        public int combo_num { get; set; }
        public int gift_id { get; set; }
        public string gift_name { get; set; }
        public int gift_num { get; set; }
        public object send_master { get; set; }
        public int uid { get; set; }
        public string uname { get; set; }
    }

    public class Data
    {
        public string action { get; set; }
        public string batch_combo_id { get; set; }
        public BatchComboSend batch_combo_send { get; set; }
        public string beatId { get; set; }
        public string biz_source { get; set; }
        public object blind_gift { get; set; }
        public int broadcast_id { get; set; }
        public string coin_type { get; set; }
        public int combo_resources_id { get; set; }
        public ComboSend combo_send { get; set; }
        public int combo_stay_time { get; set; }
        public int combo_total_coin { get; set; }
        public int crit_prob { get; set; }
        public int demarcation { get; set; }
        public int discount_price { get; set; }
        public int dmscore { get; set; }
        public int draw { get; set; }
        public int effect { get; set; }
        public int effect_block { get; set; }
        public string face { get; set; }
        public int face_effect_id { get; set; }
        public int face_effect_type { get; set; }
        public int float_sc_resource_id { get; set; }
        public long giftId { get; set; }
        public string giftName { get; set; }
        public int giftType { get; set; }
        public int gold { get; set; }
        public int guard_level { get; set; }
        public bool is_first { get; set; }
        public int is_special_batch { get; set; }
        public int magnification { get; set; }
        public MedalInfo medal_info { get; set; }
        public string name_color { get; set; }
        public int num { get; set; }
        public string original_gift_name { get; set; }
        public int price { get; set; }
        public int rcost { get; set; }
        public int remain { get; set; }
        public string rnd { get; set; }
        public object send_master { get; set; }
        public int silver { get; set; }
        public int super { get; set; }
        public int super_batch_gift_num { get; set; }
        public int super_gift_num { get; set; }
        public int svga_block { get; set; }
        public bool @switch { get; set; }
        public string tag_image { get; set; }
        public string tid { get; set; }
        public int timestamp { get; set; }
        public object top_list { get; set; }
        public int total_coin { get; set; }
        public long uid { get; set; }
        public string uname { get; set; }

        public override string ToString()
        {
            return $"giftId:{giftId} giftName:{giftName} price:{price} num:{num}";
        }
    }

    public class MedalInfo
    {
        public int anchor_roomid { get; set; }
        public string anchor_uname { get; set; }
        public int guard_level { get; set; }
        public int icon_id { get; set; }
        public int is_lighted { get; set; }
        public int medal_color { get; set; }
        public int medal_color_border { get; set; }
        public int medal_color_end { get; set; }
        public int medal_color_start { get; set; }
        public int medal_level { get; set; }
        public string medal_name { get; set; }
        public string special { get; set; }
        public int target_id { get; set; }
    }

    public class Gift
    {
        public string cmd { get; set; }
        public Data data { get; set; }
    }

    public enum MsgTypeEnum
    {
        /// <summary>
        /// 彈幕
        /// </summary>
        Comment,

        /// <summary>
        /// 禮物
        /// </summary>
        GiftSend,

        /// <summary>
        /// 禮物排名
        /// </summary>
        GiftTop,

        /// <summary>
        /// 歡迎老爷
        /// </summary>
        Welcome,

        /// <summary>
        /// 直播開始
        /// </summary>
        LiveStart,

        /// <summary>
        /// 直播結束
        /// </summary>
        LiveEnd,

        /// <summary>
        /// 其他
        /// </summary>
        Unknown,

        /// <summary>
        /// 欢迎船员
        /// </summary>
        WelcomeGuard,

        /// <summary>
        /// 购买船票（上船）
        /// </summary>
        GuardBuy,

        /// <summary>
        /// SC
        /// </summary>
        SuperChat,

        /// <summary>
        /// 观众互动信息
        /// </summary>
        Interact,

        /// <summary>
        /// 超管警告
        /// </summary>
        Warning,
        /// <summary>
        /// 观看人数, 可能是人次? 
        /// </summary>
        WatchedChange
    }

    class WrapData
    {
        public string JsonData { get; set; }
        public MsgTypeEnum MsgType { get; set; }
        public override string ToString()
        {
            return $"MsgType:{MsgType} JsonData:{JsonData}";
        }
    }

    /// <summary>
    /// 观众互动内容
    /// </summary>
    public enum InteractTypeEnum
    {
        /// <summary>
        /// 进入
        /// </summary>
        Enter = 1,

        /// <summary>
        /// 关注
        /// </summary>
        Follow = 2,

        /// <summary>
        /// 分享直播间
        /// </summary>
        Share = 3,

        /// <summary>
        /// 特别关注
        /// </summary>
        SpecialFollow = 4,

        /// <summary>
        /// 互相关注
        /// </summary>
        MutualFollow = 5,

    }

    

    class PreHandleData
    {
        public string JsonData { get; set; }
        public string MsgType { get; set; }
        public override string ToString()
        {
            return $"MsgType:{MsgType} JsonData:{JsonData}";
        }
    }
}
