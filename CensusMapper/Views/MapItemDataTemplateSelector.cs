﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CensusMapper.Views
{
    public class MapItemDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is PopulatedState)
            {
                return (DataTemplate) Application.Current.Resources["StateTemplate"];
            }
            else
            {
                return (DataTemplate)Application.Current.Resources["ZipCodeTemplate"];
            }
        }
    }
}
