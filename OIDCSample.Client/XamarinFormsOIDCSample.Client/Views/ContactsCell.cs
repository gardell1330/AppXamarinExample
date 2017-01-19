using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsOIDCSample.Client.Views
{
    class ContactsCell : ViewCell
    {
        public ContactsCell()
        {
            var Name = new Label {
                Font = Font.BoldSystemFontOfSize(NamedSize.Large)
            };
            Name.SetBinding(Label.TextProperty, new Binding("nickname"));
            var img = new Image
            {
                Aspect = Aspect.AspectFill,                
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            img.Source = ImageSource.FromResource("on.png");

            var status = new Label
            {
                Font = Font.BoldSystemFontOfSize(NamedSize.Small),
                XAlign= TextAlignment.End,
                HorizontalOptions= LayoutOptions.FillAndExpand
            };
            status.SetBinding(Label.TextProperty, new Binding("status"));

            View = new StackLayout {
                Children = { Name, status },
                Orientation = StackOrientation.Horizontal
            };
        }   
    }
}
