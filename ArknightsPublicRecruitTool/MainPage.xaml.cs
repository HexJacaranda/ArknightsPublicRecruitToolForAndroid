using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using ArknightsRecruitCore;

namespace ArknightsPublicRecruitTool
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Initialize();
        }
        private RecruitAPI API;
        private Category[] Categories;
        private KeyValuePair<string[], Operator[]>[] QueryResult = null;
        private readonly object SyncRoot = new object();
        private readonly object QuerySync = new object();
        private bool View = false;
        private Assembly Assembly;
        string ReadJsonFromEmbeddedResource(string Path)
        {
            using (var stream = Assembly.GetManifestResourceStream($"{Assembly.GetName().Name}.{Path}"))
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
        void Initialize()
        {
            Assembly = typeof(MainPage).Assembly;
            AppBase.BackgroundColor = (Color)Application.Current.Resources["BackgroundColor"];
            ResultView.ItemTemplate = new DataTemplate(typeof(ResultViewCell));
            ConvertButton.Clicked += (sender, e) =>
            {
                View = !View;
                RefreshView();
            };
            ButtonLay.BackgroundColor = Color.Accent;
            ConvertButton.BackgroundColor = Color.FromRgba(255, 255, 255, 0.4);
            ClearButton.Clicked += ClearTags;
            ClearButton.BackgroundColor = Color.FromRgba(255, 255, 255, 0.4);
            API = new RecruitAPI()
            {
                API = new Recruit(ReadJsonFromEmbeddedResource(@"OperatorSet.json"),
                                  ReadJsonFromEmbeddedResource(@"TagCategories.json"),
                                  one => one.Way.Contains("公开招募"))
            };
            API.Notify += UpdateNotify;

            Categories = (from category in API.Categories
                          select new Category(category.Key, category.Value, API.Tags)
                          { Sync = API.Sync }).ToArray();
            Table.Root = new TableRoot();
            foreach (var cat in Categories)
                Table.Root.Add(cat.Section);
        }
        private void ClearTags(object sender,EventArgs e)
        {
            lock (API.Sync)
                API.Tags.Clear();
            lock (QuerySync)
            {
                foreach (var cat in Categories)
                    cat.ClearStates();
            }
        }
        private void UpdateNotify(KeyValuePair<string[], Operator[]>[] Update)
        {
            lock (QuerySync) QueryResult = Update;
            RefreshView();
        }
        private void RefreshView()
        {
            if (View)
                GenerateTagView();
            else
                GenerateOperatorView();
        }
        private void GenerateOperatorView()
        {
            var table = new Dictionary<Operator, List<string[]>>();
            lock (QuerySync)
            {
                foreach (var query in QueryResult)
                {
                    foreach (var op in query.Value)
                    {
                        if (!table.ContainsKey(op))
                            table.Add(op, new List<string[]>());
                        table[op].Add(query.Key);
                    }
                }
            }
            var items = from pair in table
                        select new ResultViewModel()
                        {
                            Title = string.Format("星级: {0} 代号: {1}", pair.Key.Rank, pair.Key.CodeName),
                            Content = from set in pair.Value select ContactWith(set, " "),
                            TargetColor = ResultViewModel.RankColors[pair.Key.Rank - 1]
                        };
            lock (SyncRoot)
            {
                ResultView.ItemsSource = items;
            }
        }
        private void GenerateTagView()
        {
            lock (QuerySync)
            {
                if (QueryResult == null)
                    return;
                var items = from query in QueryResult
                            select new ResultViewModel()
                            {
                                Title = ContactWith(query.Key, " "),
                                Content = from op in query.Value select string.Format("星级: {0} 代号: {1}", op.Rank, op.CodeName),
                                TargetColor = ResultViewModel.RankColors[query.Value.FirstOrDefault() == null ? 0 : query.Value.First().Rank - 1]
                            };
                lock (SyncRoot)
                    ResultView.ItemsSource = items;
            }
        }
        static string ContactWith(IEnumerable<string> Sequence, string Seperator)
        {
            var builder = new StringBuilder();
            string first = Sequence.FirstOrDefault();
            if (first == default)
                return default;
            Sequence = Sequence.Skip(1);
            builder.Append(first);
            foreach (var each in Sequence)
            {
                builder.Append(Seperator);
                builder.Append(each);
            }
            return builder.ToString();
        }
    }
}
