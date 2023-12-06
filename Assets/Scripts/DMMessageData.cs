using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMGameData
{
    public enum MsgType
    {
        聊天,
        进入
    }
    public class DMMessageData
    {
        /// <summary>
        /// B站或者抖音
        /// </summary>
        public string Source { get; }
        public long UserId;
        public string UserName;
        public string HeadUrl;
        public string CommentText;
        public MsgType MsgType = MsgType.聊天;
        public DMMessageData(string src)
        {
            Source = src;
        }

        public DMMessageData(){}

        override public string ToString()
        {
            return $"[{Source}] UserId:{UserId} {UserName}:{CommentText} HeadUrl:{HeadUrl} MsgType:{MsgType}";
        }
    }
}
