using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ArknightsRecruitCore;

namespace ArknightsPublicRecruitTool
{
    public class RecruitAPI
    {
        private ObservableCollection<string> m_tags;
        public IList<string> Tags => m_tags;
        public object Sync { get; } = new object();
        public Recruit API { get; set; }
        public Dictionary<string, string[]> Categories => API.TagCategories;

        public delegate void ResultDoneNotify(KeyValuePair<string[], Operator[]>[] Update);
        public event ResultDoneNotify Notify;
        public RecruitAPI()
        {
            m_tags = new ObservableCollection<string>();
            m_tags.CollectionChanged += TagsChanged;
        }
        private void TagsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Notify(API.BestOf(m_tags.ToArray(), target => target.Rank >= 3).ToArray());
        }
    }
}
