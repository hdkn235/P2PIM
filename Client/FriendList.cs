using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Client
{
    class FriendList
    {
        public FriendList(String name)
        {
            this.ListName = name;
        }
        public String ListName { set; get; }

        private ObservableCollection<Friend> friends = new ObservableCollection<Friend>();

        public ObservableCollection<Friend> Friends
        {
            private set { }
            get
            {
                return friends;
            }
        }

        public void AddFriend(Friend newFriend)
        {
            friends.Add(newFriend);
        }

        public void RemoveFriend(Friend friend)
        {
            friends.Remove(friend);
        }
    }
}
