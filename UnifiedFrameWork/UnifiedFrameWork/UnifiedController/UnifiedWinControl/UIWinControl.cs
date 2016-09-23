using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedFrameWork.Controllerlayer
{
    public class UIWinControl
    {
        private UIChooseFiletoUploadWindow mUIChooseFiletoUploadWindow;
        private UIChooseFileSaveAsWindow mUIChooseSaveAsWindow;
        private UIChooseIEDownloadWindow mUIChooseIEDownlaod;
        internal void WinControl_Helper()
        {
        }

        public UIChooseFiletoUploadWindow UIChooseFiletoUploadWindow
        {
            get
            {
                if ((this.mUIChooseFiletoUploadWindow == null))
                {
                    this.mUIChooseFiletoUploadWindow = new UIChooseFiletoUploadWindow();
                }
                return this.mUIChooseFiletoUploadWindow;
            }
        }

        public UIChooseFileSaveAsWindow UIChooseFileSaveAsWindow
        {
            get
            {
                if ((this.mUIChooseSaveAsWindow == null))
                {
                    this.mUIChooseSaveAsWindow = new UIChooseFileSaveAsWindow();
                }
                return this.mUIChooseSaveAsWindow;
            }
        }

        public UIChooseIEDownloadWindow UIIEDownloadWindow
        {
            get
            {
                if ((this.mUIChooseIEDownlaod == null))
                {
                    this.mUIChooseIEDownlaod = new UIChooseIEDownloadWindow();
                }
                return this.mUIChooseIEDownlaod;
            }
        }
    }
    public class UIChooseFiletoUploadWindow : WinWindow
    {

        public UIChooseFiletoUploadWindow()
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.Name] = "Choose File to Upload";
            this.SearchProperties[WinWindow.PropertyNames.ClassName] = "#32770";
            this.WindowTitles.Add("Choose File to Upload");
            #endregion
        }

        public UIOpenWindow UIOpenWindow
        {
            get
            {
                if ((this.mUIOpenWindow == null))
                {
                    this.mUIOpenWindow = new UIOpenWindow(this);
                }
                return this.mUIOpenWindow;
            }
        }
        #region Fields
        //private UIShellViewClient mUIShellViewClient;
        private UIOpenWindow mUIOpenWindow;
        #endregion
    }

    public class UIOpenWindow : WinWindow
    {
        public UIOpenWindow(UITestControl searchLimitContainer) :
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.ControlId] = "1";
            this.WindowTitles.Add("Choose File to Upload");
            #endregion
        }

        #region Properties
        public WinButton UIOpenButton
        {
            get
            {
                if ((this.mUIOpenButton == null))
                {
                    this.mUIOpenButton = new WinButton(this);
                    #region Search Criteria
                    this.mUIOpenButton.SearchProperties[WinButton.PropertyNames.Name] = "Open";
                    this.mUIOpenButton.WindowTitles.Add("Choose File to Upload");
                    #endregion
                }
                return this.mUIOpenButton;
            }
        }

        public WinEdit UIFilePath
        {
            get
            {
                if ((this.mUIFilePath == null))
                {
                    this.mUIFilePath = new WinEdit(this);
                    #region Search Criteria
                    this.mUIFilePath.SearchProperties[WinButton.PropertyNames.Name] = "File name:";
                    this.mUIFilePath.WindowTitles.Add("Choose File to Upload");
                    #endregion
                }
                return this.mUIFilePath;
            }
        }
        #endregion

        #region Fields
        private WinButton mUIOpenButton;
        private WinEdit mUIFilePath;
        #endregion
    }

    public class UIChooseFileSaveAsWindow : WinWindow
    {
        public UIChooseFileSaveAsWindow()
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.Name] = "Save As";
            this.SearchProperties[WinWindow.PropertyNames.ClassName] = "#32770";
            this.WindowTitles.Add("Save As");
            #endregion
        }

        public UISaveAsWindow UISaveAsWindow
        {
            get
            {
                if ((this.mUISaveAsWindow == null))
                {
                    this.mUISaveAsWindow = new UISaveAsWindow(this);
                }
                return this.mUISaveAsWindow;
            }
        }
        #region Fields
        //private UIShellViewClient mUIShellViewClient;
        private UISaveAsWindow mUISaveAsWindow;
        #endregion
    }

    public class UISaveAsWindow : WinWindow
    {
        public UISaveAsWindow(UITestControl searchLimitContainer) :
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.ControlId] = "1";
            this.WindowTitles.Add("Save As");
            #endregion
        }

        #region Properties
        public WinButton UISaveButton
        {
            get
            {
                if ((this.mUISaveButton == null))
                {
                    this.mUISaveButton = new WinButton(this);
                    #region Search Criteria
                    this.mUISaveButton.SearchProperties[WinButton.PropertyNames.Name] = "Save";
                    this.mUISaveButton.WindowTitles.Add("Save As");
                    #endregion
                }
                return this.mUISaveButton;
            }
        }

        public WinEdit UIFilePath
        {
            get
            {
                if ((this.mUIFilePath == null))
                {
                    this.mUIFilePath = new WinEdit(this);
                    #region Search Criteria
                    this.mUIFilePath.SearchProperties[WinButton.PropertyNames.Name] = "File name:";
                    this.mUIFilePath.WindowTitles.Add("Save As");
                    #endregion
                }
                return this.mUIFilePath;
            }
        }
        #endregion

        #region Fields
        private WinButton mUISaveButton;
        private WinEdit mUIFilePath;
        #endregion
    }

    public class UIChooseIEDownloadWindow : WinWindow
    {
        public UIChooseIEDownloadWindow()
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.Name] = "Internet Explorer";
            this.SearchProperties[WinWindow.PropertyNames.ClassName] = "#32770";
            this.WindowTitles.Add("Internet Explorer");
            #endregion
        }

        public UIIEDownloadWindow UIIEDownloadWindow
        {
            get
            {
                if ((this.mUISaveAsWindow == null))
                {
                    this.mUISaveAsWindow = new UIIEDownloadWindow(this);
                }
                return this.mUISaveAsWindow;
            }
        }
        #region Fields
        //private UIShellViewClient mUIShellViewClient;
        private UIIEDownloadWindow mUISaveAsWindow;
        #endregion

    }

    public class UIIEDownloadWindow : WinWindow
    {
        public UIIEDownloadWindow(UITestControl searchLimitContainer) :
                base(searchLimitContainer)
        {
            #region Search Criteria
            this.SearchProperties[WinWindow.PropertyNames.ControlId] = "1";
            this.WindowTitles.Add("Internet Explorer");
            #endregion
        }

        public WinButton UIIEOpenButton
        {
            get
            {
                if ((this.mUIIEOpenButton == null))
                {
                    this.mUIIEOpenButton = new WinButton(this);
                    #region Search Criteria
                    this.mUIIEOpenButton.SearchProperties[WinButton.PropertyNames.Name] = "Open";
                    this.mUIIEOpenButton.WindowTitles.Add("Internet Explorer");
                    #endregion
                }
                return this.mUIIEOpenButton;
            }
        }

        public WinButton UIIESaveButton
        {
            get
            {
                if ((this.mUIIESaveButton == null))
                {
                    this.mUIIESaveButton = new WinButton(this);
                    #region Search Criteria
                    this.mUIIESaveButton.SearchProperties[WinButton.PropertyNames.Name] = "Save";
                    this.mUIIESaveButton.WindowTitles.Add("Internet Explorer");
                    #endregion
                }
                return this.mUIIESaveButton;
            }
        }

        public WinButton UIIESaveAsButton
        {
            get
            {
                if ((this.mUIIESaveAsButton == null))
                {
                    this.mUIIESaveAsButton = new WinButton(this);
                    #region Search Criteria
                    this.mUIIESaveAsButton.SearchProperties[WinButton.PropertyNames.Name] = "Save as";
                    this.mUIIESaveAsButton.WindowTitles.Add("Internet Explorer");
                    #endregion
                }
                return this.mUIIESaveAsButton;
            }
        }

        public WinButton UIIECancelButton
        {
            get
            {
                if ((this.mUIIECancelButton == null))
                {
                    this.mUIIECancelButton = new WinButton(this);
                    #region Search Criteria
                    this.mUIIECancelButton.SearchProperties[WinButton.PropertyNames.Name] = "Cancel";
                    this.mUIIECancelButton.WindowTitles.Add("Internet Explorer");
                    #endregion
                }
                return this.mUIIECancelButton;
            }
        }

        #region Fields
        private WinButton mUIIESaveButton;
        private WinButton mUIIEOpenButton;
        private WinButton mUIIESaveAsButton;
        private WinButton mUIIECancelButton;
        #endregion
    }
}

