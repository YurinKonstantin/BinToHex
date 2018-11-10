using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BinToHex
{
    class ClassSetUpUser
    {
        static Windows.Storage.ApplicationDataContainer localSettings =
      Windows.Storage.ApplicationData.Current.LocalSettings;
        static Windows.Storage.StorageFolder localFolder =
               Windows.Storage.ApplicationData.Current.LocalFolder;

        static  public bool start { get; set; }
        Visibility? isShowASCII;
        public Visibility? IsShowASCII
        {
            get
            {
                return isShowASCII;
            }
            set
            {
                isShowASCII = value;
            }
        }
       static string applicationTheme;
       static public string Application
        {
            get
            {
                return applicationTheme;
            }
            set
            {
                applicationTheme = value;
            }
        }
        static bool shovH=true;
        static public bool ShovH
        {
            get
            {
                return shovH;
            }
            set
            {
                shovH = value;
            }
        }
        static public void saveUseSet()
        {
            // Composite setting

            Windows.Storage.ApplicationDataCompositeValue composite =
                new Windows.Storage.ApplicationDataCompositeValue();
            composite["strApplicationTheme"] = Application;
            composite["shovH"] = ShovH;
            // composite["intPorogS"] = PorogS;
            localSettings.Values["CompositeSetting"] = composite;
        }
        static public void SetPush()
        {
                Windows.Storage.ApplicationDataCompositeValue composite =
                   (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["CompositeSetting"];

                if (composite == null)
                {
                    // No data
                }
                else
                {
                ShovH = Convert.ToBoolean(composite["shovH"]);
                if (composite["strApplicationTheme"].ToString() != String.Empty)
                {
                    Application = Convert.ToString(composite["strApplicationTheme"]);
                   
                }
                   
                else
                    Application = "Light";

                }
            
          
        }
    }
}
