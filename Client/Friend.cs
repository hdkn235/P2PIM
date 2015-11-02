using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    class Friend
    {
        public Friend(FriendList list)
        {
            this.List = list;
        }
        //头像的路径
        public String HeadPath { set; get; }
        //好友名字
        public String Name { set; get; }
        //签名
        public String Autograph { set; get; }

        public FriendList List { set; get; }
    }
}
