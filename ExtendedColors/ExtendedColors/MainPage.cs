using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ColorExtensions;
using Xamarin.Forms;

namespace ExtendedColors
{
    public class ColorsPage : ContentPage
    {
        // Code segment from Mark Smith found on this thread:
        //http://forums.xamarin.com/discussion/17392/how-to-create-complex-bindings
        public ColorsPage()
        {
            var fieldList = typeof(PaletteEx).GetTypeInfo().DeclaredFields
                .Where(p => p.FieldType == typeof(Color) && p.IsStatic && !p.Name.Contains("<"))
                .Select(p => new
                {
                    Name = p.Name,
                    Color = (Color)p.GetValue(null)
                }).ToList();


            Padding = new Thickness(5, Device.OnPlatform(20, 0, 5));

            Content = new ListView()
            {
                ItemsSource = fieldList,
                ItemTemplate = new DataTemplate(typeof(DynamicTemplateLayout))
            };
        }

        public class DynamicTemplateLayout : ViewCell
        {
            protected override void OnBindingContextChanged()
            {
                base.OnBindingContextChanged();

                dynamic c = BindingContext;
               
                View = new Label
                {
                    TextColor = GetTextColor(c.Color),
                    BackgroundColor = c.Color,
                    Text = c.Name,
                    Font = Font.SystemFontOfSize(NamedSize.Medium),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start
                };
            }

            // Formula for computing Luminance out of R G B, which is something close to luminance = (red * 0.3) + (green * 0.6) + (blue * 0.1).
            // Original Source: http://stackoverflow.com/questions/20978198/how-to-match-uilabels-textcolor-to-its-background
            private static Color GetTextColor(Color backgroundColor)
            {
                var backgroundColorDelta = ((backgroundColor.R * 0.3) + (backgroundColor.G * 0.6) + (backgroundColor.B * 0.1));

                return (backgroundColorDelta > 0.4f) ? Color.Black : Color.White;
            }
        }
    }
}
