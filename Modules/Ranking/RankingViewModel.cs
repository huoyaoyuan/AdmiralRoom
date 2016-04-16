using System;
using System.ComponentModel;
using System.IO;
using Huoyaoyuan.AdmiralRoom.API;
using Huoyaoyuan.AdmiralRoom.Officer;

namespace Huoyaoyuan.AdmiralRoom.Modules.Ranking
{
    class RankingViewModel : NotificationObject
    {
        public RankingViewModel()
        {
            Load();
            Staff.API("api_req_ranking/getlist").Subscribe<ranking_getlist>(RankingListHandler);
            Staff.Current.Admiral.PropertyChanged += OnExpChanged;
        }
        public static RankingViewModel Instance { get; set; }
        private void RankingListHandler(ranking_getlist api)
        {
            foreach (var player in api.api_list)
            {
                switch (player.api_no)
                {
                    case 1:
                        if (Number1.Point < player.api_rate)
                            Number1 = new RankRecord
                            {
                                Point = player.api_rate,
                                Diff = player.api_rate - Number1.Point
                            };
                        else if (Number1.Point > player.api_rate)
                            Number1 = new RankRecord
                            {
                                Point = player.api_rate,
                                Diff = 0
                            };
                        break;
                    case 5:
                        if (Number5.Point < player.api_rate)
                            Number5 = new RankRecord
                            {
                                Point = player.api_rate,
                                Diff = player.api_rate - Number5.Point
                            };
                        else if (Number5.Point > player.api_rate)
                            Number5 = new RankRecord
                            {
                                Point = player.api_rate,
                                Diff = 0
                            };
                        break;
                    case 20:
                        if (Number20.Point < player.api_rate)
                            Number20 = new RankRecord
                            {
                                Point = player.api_rate,
                                Diff = player.api_rate - Number20.Point
                            };
                        else if (Number20.Point > player.api_rate)
                            Number20 = new RankRecord
                            {
                                Point = player.api_rate,
                                Diff = 0
                            };
                        break;
                    case 100:
                        if (Number100.Point < player.api_rate)
                            Number100 = new RankRecord
                            {
                                Point = player.api_rate,
                                Diff = player.api_rate - Number100.Point
                            };
                        else if (Number100.Point > player.api_rate)
                            Number100 = new RankRecord
                            {
                                Point = player.api_rate,
                                Diff = 0
                            };
                        break;
                    case 500:
                        if (Number500.Point < player.api_rate)
                            Number500 = new RankRecord
                            {
                                Point = player.api_rate,
                                Diff = player.api_rate - Number500.Point
                            };
                        else if (Number500.Point > player.api_rate)
                            Number500 = new RankRecord
                            {
                                Point = player.api_rate,
                                Diff = 0
                            };
                        break;
                }
                if (player.api_member_id == Staff.Current.Admiral.MemberID)
                {
                    if (MyLastPoint != player.api_rate || MyRank != player.api_no)
                    {
                        MyRank = player.api_no;
                        MyLastPoint = player.api_rate;
                        myLastExp = myLastExpStore;
                        OnPropertyChanged(nameof(MyPoint));
                    }
                }
            }
        }

        private void OnExpChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Admiral.Exp))
            {
                var time = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7));
                if (time.Date != lastExpUpdateTime.Date || time.Hour / 12 != lastExpUpdateTime.Hour / 12)
                {
                    myLastExpStore = myExp;
                    lastExpUpdateTime = time;
                    Save();
                }
                myExp = (sender as Admiral).Exp.Current;
                if (myLastExp == 0) myLastExp = myLastExpStore = myExp;
                lastExpUpdateTime = time;
                OnPropertyChanged(nameof(MyPoint));
            }
        }

        public void Load()
        {
            try
            {
                using (var reader = File.OpenText(@"logs\ranking.txt"))
                {
                    string[] line;
                    lastExpUpdateTime = new DateTimeOffset(DateTime.Parse(reader.ReadLine()), TimeSpan.FromHours(7));
                    line = reader.ReadLine().Split(',');
                    Number1 = new RankRecord { Point = int.Parse(line[0]), Diff = int.Parse(line[1]) };
                    line = reader.ReadLine().Split(',');
                    Number5 = new RankRecord { Point = int.Parse(line[0]), Diff = int.Parse(line[1]) };
                    line = reader.ReadLine().Split(',');
                    Number20 = new RankRecord { Point = int.Parse(line[0]), Diff = int.Parse(line[1]) };
                    line = reader.ReadLine().Split(',');
                    Number100 = new RankRecord { Point = int.Parse(line[0]), Diff = int.Parse(line[1]) };
                    line = reader.ReadLine().Split(',');
                    Number500 = new RankRecord { Point = int.Parse(line[0]), Diff = int.Parse(line[1]) };
                    line = reader.ReadLine().Split(',');
                    MyRank = int.Parse(line[0]);
                    MyLastPoint = int.Parse(line[1]);
                    myLastExp = int.Parse(line[2]);
                    myLastExpStore = int.Parse(line[3]);
                    myExp = int.Parse(line[4]);
                }
            }
            catch { }
        }

        public void Save()
        {
            try
            {
                using (var writer = File.CreateText(@"logs\ranking.txt"))
                {
                    writer.WriteLine(lastExpUpdateTime.DateTime);
                    writer.WriteLine($"{Number1.Point},{Number1.Diff}");
                    writer.WriteLine($"{Number5.Point},{Number5.Diff}");
                    writer.WriteLine($"{Number20.Point},{Number20.Diff}");
                    writer.WriteLine($"{Number100.Point},{Number100.Diff}");
                    writer.WriteLine($"{Number500.Point},{Number500.Diff}");
                    writer.WriteLine($"{MyRank},{MyLastPoint},{myLastExp},{myLastExpStore},{myExp}");
                    writer.Flush();
                }
            }
            catch { }
        }

        public struct RankRecord
        {
            public int Point { get; set; }
            public int Diff { get; set; }
        }

        #region Number1
        private RankRecord _number1;
        public RankRecord Number1
        {
            get { return _number1; }
            set
            {
                _number1 = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Number5
        private RankRecord _number5;
        public RankRecord Number5
        {
            get { return _number5; }
            set
            {
                _number5 = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Number20
        private RankRecord _number20;
        public RankRecord Number20
        {
            get { return _number20; }
            set
            {
                _number20 = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Number100
        private RankRecord _number100;
        public RankRecord Number100
        {
            get { return _number100; }
            set
            {
                _number100 = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Number500
        private RankRecord _number500;
        public RankRecord Number500
        {
            get { return _number500; }
            set
            {
                _number500 = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region MyRank
        private int _myrank;
        public int MyRank
        {
            get { return _myrank; }
            set
            {
                if (_myrank != value)
                {
                    _myrank = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region MyLastPoint
        private int _mylastpoint;
        public int MyLastPoint
        {
            get { return _mylastpoint; }
            set
            {
                if (_mylastpoint != value)
                {
                    _mylastpoint = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        public double MyPoint => MyLastPoint + (myExp - myLastExp) / 1428.0;

        private int myLastExp;
        private int myLastExpStore;
        private int myExp;
        private DateTimeOffset lastExpUpdateTime;
    }
}
