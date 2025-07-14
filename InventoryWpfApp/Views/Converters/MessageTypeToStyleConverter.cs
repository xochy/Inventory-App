using InventoryWpfApp.ViewModels.Base.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace InventoryWpfApp.Views.Converters
{
    public class MessageTypeToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MessageType messageType)
            {
                switch (messageType)
                {
                    case MessageType.Success:
                        return Application.Current.FindResource("SuccessMessageStyle") as Style;
                    case MessageType.Error:
                        return Application.Current.FindResource("ErrorMessageStyle") as Style;
                    case MessageType.Info:
                        return Application.Current.FindResource("InfoMessageStyle") as Style;
                    case MessageType.None:
                    default:
                        return Application.Current.FindResource("MessageTextBlockStyle") as Style;
                }
            }
            return Application.Current.FindResource("MessageTextBlockStyle") as Style; // Default
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
