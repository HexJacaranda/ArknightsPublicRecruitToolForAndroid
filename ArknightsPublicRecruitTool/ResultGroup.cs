using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ArknightsPublicRecruitTool
{
    public class ResultViewModel
    {
        public static readonly Color[] RankColors = new Color[]
        {
            (Color)Application.Current.Resources["LowRank"],
            (Color)Application.Current.Resources["LowRank"],
            (Color)Application.Current.Resources["LowRank"],
            (Color)Application.Current.Resources["RankFour"],
            (Color)Application.Current.Resources["RankFive"],
            (Color)Application.Current.Resources["RankSix"]
        };
        public string Title { get; set; }
        public IEnumerable<string> Content { get; set; }
        public Color TargetColor { get; set; }
    }
    public class ResultViewCell : ViewCell
    {
        private StackLayout m_layout;
        public static readonly BindableProperty ContentProperty =
            BindableProperty.Create("Content", typeof(IEnumerable<string>), typeof(ResultViewCell), propertyChanging: ContentChanging);
        private static void ContentChanging(BindableObject bindable, object old, object value)
        {
            var cell = bindable as ResultViewCell;
            if (value == null)
                return;
            foreach (var single in value as IEnumerable<string>)
            {
                var lable = new Label()
                {
                    BackgroundColor = Color.Transparent,
                    Text = single,
                    HeightRequest = 35,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };
                cell.m_layout.Children.Add(lable);
            }
        }
        public IEnumerable<string> Content
        {
            get
            {
                return (IEnumerable<string>)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }
        public ResultViewCell()
        {
            m_layout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical
            };
            var title = new Label()
            {
                BackgroundColor = Color.FromRgba(255, 255, 255, 0.4),
                HeightRequest = 35,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            title.SetBinding(Label.TextProperty, new Binding("Title"));
            m_layout.SetBinding(VisualElement.BackgroundColorProperty, new Binding("TargetColor"));
            SetBinding(ContentProperty, new Binding("Content"));
            m_layout.Children.Add(title);
            View = m_layout;
        }
    }
}
