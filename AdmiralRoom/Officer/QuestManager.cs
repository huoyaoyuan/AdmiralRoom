using System;
using System.Collections.Generic;
using System.Linq;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class QuestManager : NotifyBase
    {
        public QuestManager()
        {
            Staff.API("api_get_member/questlist").Subscribe(x => CheckQuestPage(x.ParseQuest().Data));
        }
        public IDTable<Quest> AvilableQuests = new IDTable<Quest>();
        public IList<Quest> QuestInProgress
        {
            get
            {
                List<Quest> list = AvilableQuests.Where(x => x.State == QuestState.InProgress || x.State == QuestState.Complete).ToList();
                while (list.Count < InProgressCount)
                    list.Add(new Quest(new API.api_quest()));
                list.Sort();
                return list;
            }
        }
        public int InProgressCount { get; private set; }
        public int AvilableCount { get; private set; }
        private int lastcheckedpage;
        private int lastcheckedfrom;
        private int lastcheckedto;
        private DateTime lastcheckedtime;
        void CheckQuestPage(getmember_questlist api)
        {
            int checkfrom, checkto;
            if(api.api_list == null)
            {
                if (api.api_disp_page == 1) AvilableQuests.Clear();
                return;
            }
            checkfrom = api.api_list.First().api_no;
            checkto = api.api_list.Last().api_no;
            if (api.api_disp_page == 1) checkfrom = int.MinValue;
            else if (lastcheckedpage == api.api_disp_page - 1) checkfrom = lastcheckedto + 1;
            if (api.api_disp_page == api.api_page_count) checkto = int.MaxValue;
            else if (lastcheckedpage == api.api_disp_page + 1) checkto = lastcheckedfrom - 1;
            foreach (var item in AvilableQuests.Where(x => x.Id >= checkfrom && x.Id <= checkto).ToList())
                AvilableQuests.Remove(item);
            DateTime checktime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, QuestPeriodTime);
            if (checktime.Date != lastcheckedtime.Date)
                foreach (var item in AvilableQuests.Where(x => x.IsDaily).ToList())
                    AvilableQuests.Remove(item);
            if (checktime.WeekStart() != lastcheckedtime.WeekStart()) 
                foreach (var item in AvilableQuests.Where(x => x.Period == QuestPeriod.Weekly).ToList())
                    AvilableQuests.Remove(item);
            AvilableQuests.UpdateWithoutRemove(api.api_list, x => x.api_no);
            if (checktime.Month != lastcheckedtime.Month)
                foreach (var item in AvilableQuests.Where(x => x.Period == QuestPeriod.Monthly).ToList())
                    AvilableQuests.Remove(item);
            AvilableQuests.UpdateWithoutRemove(api.api_list, x => x.api_no);
            AvilableCount = api.api_count;
            InProgressCount = api.api_exec_count;
            lastcheckedpage = api.api_disp_page;
            lastcheckedfrom = checkfrom;
            lastcheckedto = checkto;
            lastcheckedtime = checktime;
            OnAllPropertyChanged();
        }
        public static readonly TimeZoneInfo QuestPeriodTime = TimeZoneInfo.CreateCustomTimeZone("KancolleQuest", TimeSpan.FromHours(4), "", "");
    }
}
