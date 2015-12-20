using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Admiral : NotificationObject
    {
        public Admiral()
        {
            Staff.API("api_get_member/record").Subscribe<getmember_record>(RecordHandler);
            Staff.API("api_get_member/basic").Subscribe<getmember_basic>(BasicHandler);
        }

        #region Level
        private int _level;
        public int Level
        {
            get { return _level; }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region MemberID
        private int _memberid;
        public int MemberID
        {
            get { return _memberid; }
            set
            {
                if (_memberid != value)
                {
                    _memberid = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Nickname
        private string _nickname;
        public string Nickname
        {
            get { return _nickname; }
            set
            {
                if (_nickname != value)
                {
                    _nickname = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Comment
        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    _comment = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Exp
        private Exp _exp;
        public Exp Exp
        {
            get { return _exp; }
            set
            {
                if (_exp != value)
                {
                    _exp = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Rank
        private string _rank;
        public string Rank
        {
            get { return _rank; }
            set
            {
                if (_rank != value)
                {
                    _rank = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Fleet
        private int _fleet;
        public int Fleet
        {
            get { return _fleet; }
            set
            {
                if (_fleet != value)
                {
                    _fleet = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region ShipCount
        private int _shipcount;
        public int ShipCount
        {
            get { return _shipcount; }
            set
            {
                if (_shipcount != value)
                {
                    _shipcount = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region MaxShip
        private int _maxship;
        public int MaxShip
        {
            get { return _maxship; }
            set
            {
                if (_maxship != value)
                {
                    _maxship = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region EquipCount
        private int _euqipcount;
        public int EquipCount
        {
            get { return _euqipcount; }
            set
            {
                if (_euqipcount != value)
                {
                    _euqipcount = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region MaxEquip
        private int _maxequip;
        public int MaxEquip
        {
            get { return _maxequip; }
            set
            {
                if (_maxequip != value)
                {
                    _maxequip = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Furniture
        private int _furniture;
        public int Furniture
        {
            get { return _furniture; }
            set
            {
                if (_furniture != value)
                {
                    _furniture = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region FurnitureCoin
        private int _coin;
        public int FurnitureCoin
        {
            get { return _coin; }
            set
            {
                if (_coin != value)
                {
                    _coin = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public void RecordHandler(getmember_record data)
        {
            if (data == null) return;
            Level = data.api_level;
            MemberID = data.api_member_id;
            Nickname = data.api_nickname;
            Comment = data.api_cmt;
            Exp = new Exp(data.api_experience, Level, false);
            Rank = ConstData.AdmiralRanks[data.api_rank];
            Fleet = data.api_deck;
            ShipCount = data.api_ship[0];
            MaxShip = data.api_ship[1];
            EquipCount = data.api_slotitem[0];
            MaxEquip = data.api_slotitem[1];
            Furniture = data.api_furniture;
        }
        public void BasicHandler(getmember_basic data)
        {
            if (data == null) return;
            Level = data.api_level;
            MemberID = data.api_member_id;
            Nickname = data.api_nickname;
            Comment = data.api_comment;
            Exp = new Exp()
            {
                Current = data.api_experience,
                NextLevel = ConstData.GetAdmiralExp(Level + 1),
                PrevLevel = ConstData.GetAdmiralExp(Level),
            };
            Exp.Next = Exp.NextLevel - Exp.Current;
            Rank = ConstData.AdmiralRanks[data.api_rank];
            Fleet = data.api_count_deck;
            MaxShip = data.api_max_chara;
            MaxEquip = data.api_max_slotitem;
            FurnitureCoin = data.api_fcoin;
        }
    }
}
