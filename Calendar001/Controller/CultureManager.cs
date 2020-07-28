using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Calendar001.Controller
{
    class CultureManager
    {
        public void SelectCulture(string culture)
        {
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in System.Windows.Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = string.Format("/Resources/StringResources.{0}.xaml", culture);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(
                d => d.Source.OriginalString == requestedCulture);

            if (resourceDictionary == null)
            {
                requestedCulture = "/Resources/StringResources.ko-kr.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(
                    d => d.Source.OriginalString == requestedCulture);
            }
            else
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                System.Windows.Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            //지역화 설정
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }
    }
}
