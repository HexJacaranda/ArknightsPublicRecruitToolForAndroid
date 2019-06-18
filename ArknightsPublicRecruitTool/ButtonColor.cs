using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ArknightsPublicRecruitTool
{
    class ButtonColorProperty : BindableObject
    {
        public static Color GetTargetColor(BindableObject obj)
        {       
            return (Color)obj.GetValue(TargetColorProperty);
        }
        public static void SetTargetColor(BindableObject obj, Color value)
        {
            obj.SetValue(TargetColorProperty, value);
        }
        public static readonly BindableProperty TargetColorProperty =
            BindableProperty.CreateAttached("TargetColor", typeof(Color), typeof(ButtonColorProperty), Color.Default);
    }
}
