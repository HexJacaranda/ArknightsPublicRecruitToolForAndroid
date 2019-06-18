using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xamarin.Forms;

namespace ArknightsPublicRecruitTool
{
    public class UnitCell : ViewCell
    {
        private Label m_lable;
        public Color BackgroundColor
        {
            get
            {
                return m_lable.BackgroundColor;
            }
            set
            {
                m_lable.BackgroundColor = value;
            }
        }
        public string Text
        {
            get
            {
                return m_lable.Text;
            }
            set
            {
                m_lable.Text = value;
            }
        }
        public bool Selected { get; set; }
        public UnitCell(string Content)
        {
            Selected = false;
            m_lable = new Label()
            {
                Text = Content,
                HeightRequest = 35,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            View = m_lable;
        }
        public void GoSelected()
        {
            Selected = true;
            BackgroundColor = Color.LightBlue;
        }
        public void GoUnselected()
        {
            Selected = false;
            BackgroundColor = Color.White;
        }
    }
    public class Category
    {
        public IList<string> Reference { get; private set; }
        public TableSection Section { get; private set; }
        public object Sync { get; set; }
        public Category(string Title,IEnumerable<string> Content,IList<string> Tags)
        {
            Reference = Tags;
            Section = new TableSection(Title);
            foreach (var single in Content)
            {
                UnitCell cell = new UnitCell(single);
                cell.Tapped += (sender, e) =>
                {
                    UnitCell target = sender as UnitCell;
                    if (target.Selected)
                    {
                        target.GoUnselected();
                        lock (Sync)
                            Reference.Remove(target.Text);
                    }
                    else
                    {
                        target.GoSelected();
                        lock (Sync)
                            Reference.Add(target.Text);
                    }
                };
                Section.Add(cell);
            }
        }       
        public void ClearStates()
        {
            foreach (UnitCell cell in Section)
                cell.GoUnselected();
        }
    }
}
