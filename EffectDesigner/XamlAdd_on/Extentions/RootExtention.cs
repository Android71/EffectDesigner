//http://www.codeproject.com/Tips/612994/Binding-with-Properties-defined-in-Code-Behind
//
//<DataGridTemplateColumn Width="{Binding Source={local:Root}, Path=ColumnOneWidth}"
// CellTemplate="{Binding Source={ local:Root }, Path=ColumnOneTemplate}"/>


using System;
using System.Xaml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace XamlAdd_on.Extentions
{
    public class RootExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IRootObjectProvider provider = serviceProvider.GetService
            (typeof(IRootObjectProvider)) as IRootObjectProvider;
            if (provider != null)
            {
                return provider.RootObject;
            }

            return null;
        }
    }
}
