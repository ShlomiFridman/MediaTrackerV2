using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Windows;

namespace MediaTracker
{
    class AppSettings
    {
        // static instance
        private static AppSettings Instance;

        private readonly string saveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MediaTracker", "settings.json");

        #region Main Window properties
        // the instance of the mainWindow
        public MainWindow mainWindow;

        /// <summary>
        /// the setting for if to auto advance after opening a file
        /// </summary>
        public bool mainAutoAdvance
        {
            get {
                return (bool) this.settings["mainAutoAdvance"];
            }
            set
            {
                this.settings["mainAutoAdvance"] = (bool) value;
            }
        }

        /// <summary>
        /// the setting for if the random file section should be visible
        /// </summary>
        public bool mainRandomExpanded
        {
            get{
                return (bool) this.settings["mainRandomExpanded"];
            }
            set
            {
                this.settings["mainRandomExpanded"] = (bool) value;
            }
        }

        /// <summary>
        /// the setting for if the settings section should be visible
        /// </summary>
        public bool mainSettingsExpander
        {
            get
            {
                return (bool)this.settings["mainSettingsExpanded"];
            }
            set
            {
                this.settings["mainSettingsExpanded"] = (bool) value;
            }
        }

        /// <summary>
        /// the setting for what to do on opening a file
        /// </summary>
        public int mainOnOpenFile
        {
            get
            {
                return (int) this.settings["mainOnOpenFile"];
            }
            set
            {
                if (value < 0 || value > 2)
                    throw new ArgumentException("The value can only be from 0 to 2");
                this.settings["mainOnOpenFile"] = (int) value;
            }
        }
        #endregion

        // dictionary that were loaded from save file, if the file is corrupted will contain the default values
        private Dictionary<string, dynamic> settings;
        private bool loadedFile;

        /// <summary>
        /// private constructor of the class, initialize the settings with the default values,
        /// and tries to load the settings json file,
        /// if succeed will update the values of the settings, else will keep the default values
        /// </summary>
        private AppSettings()
        {
            this.loadedFile = false;
            this.settings = new Dictionary<string, dynamic>()
            {
                // MainWindow settings
                // default location
                {"mainLeft",(double) 50 },
                {"mainTop",(double) 50 },
                // default size
                {"mainWidth",(double) 600 },
                {"mainHeight",(double) 290 },
                // autoadvance setting for the open track file func
                {"mainAutoAdvance", (bool) true },
                // what to do after opening a file
                {"mainOnOpenFile", (int) 0 },
                // bool value if the random section is expanded
                {"mainRandomExpanded", (bool) false },
                // bool value if the setting section is expanded
                {"mainSettingsExpanded", (bool) false },
                // default root value
                {"mainRoot", (string) "Root" },
                // the positions of the grid
                { "mainGridCols", new double[] {1.0, 1.0 } }
        };
            // check if the save file exist
            if (File.Exists(saveFilePath))
            {
                // init stream and json with null values
                StreamReader streamReader = null;
                string json = string.Empty;
                // check if file exists
                try
                {
                    // try to open stream to file
                    streamReader = new StreamReader(saveFilePath);
                    // read the content
                    json = streamReader.ReadToEnd();
                    // content read successfully, flag set as true
                    this.loadedFile = true;
                    // initialize the loaded settings dictionary for comparison
                    Dictionary<string, dynamic> loadedSettings = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
                    // compare the loaded and default dictionaries
                    foreach (var key in loadedSettings.Keys)
                        if (this.settings.ContainsKey(key))
                            this.settings[key] = loadedSettings[key];
                }
                catch (Exception readerEx)
                {
                    // in case of an error, alert user
                    Utilties.errorMessage("Load setting error", readerEx.Message);
                    this.loadedFile = false;
                }
                finally
                {
                    // if steam is initialized, close it at the end
                    if (streamReader != null)
                        streamReader.Close();
                }
            }
        }

        /// <summary>
        /// load the current settings into the main window,
        /// will also save the given instance of the mainWindow for future use
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <returns>returns true if the loaded settings were from the save file, else return false (the default values)</returns>
        public bool loadSettings(MainWindow mainWindow)
        {
            // save the latest mainWindow instance in this.mainWindow
            this.mainWindow = mainWindow;
            // load the settings to the window
            mainWindow.Left = (double) this.settings["mainLeft"];
            mainWindow.Top = (double) this.settings["mainTop"];
            mainWindow.Width = (double) this.settings["mainWidth"];
            mainWindow.Height = (double) this.settings["mainHeight"];
            mainWindow.autoAdvanceSetting.IsChecked = (bool) this.settings["mainAutoAdvance"];
            mainWindow.OnOpenComboBox.SelectedIndex= (int) this.settings["mainOnOpenFile"];
            mainWindow.randomExpander.IsExpanded = (bool) this.settings["mainRandomExpanded"];
            mainWindow.settingsExpander.IsExpanded = (bool) this.settings["mainSettingsExpanded"];
            mainWindow.treeCol.Width = new GridLength((double) this.settings["mainGridCols"][0], GridUnitType.Star);
            mainWindow.trackerCol.Width = new GridLength((double) this.settings["mainGridCols"][1], GridUnitType.Star);
            if (!((string) this.settings["mainRoot"]).Equals("Root"))
                mainWindow.setRoot((string)this.settings["mainRoot"]);
            // return true if the settings were from the save file, else return false (the default settings)
            return loadedFile;
        }

        /// <summary>
        /// saves the current settings into the save json file
        /// </summary>
        /// <returns>returns true if saved successfully, else false</returns>
        public bool save()
        {
            bool saved = true;
            if (this.mainWindow != null)
            {
                // get the latest main window settings from the latest instance
                this.settings["mainLeft"] = (double)this.mainWindow.Left;
                this.settings["mainTop"] = (double)this.mainWindow.Top;
                this.settings["mainWidth"] = (double)this.mainWindow.Width;
                this.settings["mainHeight"] = (double) this.mainWindow.Height;
                this.settings["mainRoot"] = (string)this.mainWindow.rootTextBox.Text;
                this.settings["mainGridCols"][0] = (double)this.mainWindow.treeCol.Width.Value;
                this.settings["mainGridCols"][1] = (double)this.mainWindow.trackerCol.Width.Value;
            }
            // initialize the json object of the settings
            string json = JsonConvert.SerializeObject(this.settings, Formatting.Indented);
            // init the writer as null
            StreamWriter writer = null;
            try
            {
                // try to open stream to file
                writer = File.CreateText(saveFilePath);
                // writing the json object
                writer.Write(json);
            }
            catch (Exception writerEx)
            {
                // in case of exception alert user
                Utilties.errorMessage("Save Settings Exception", writerEx.Message);
                // exception occurred, save failed
                saved = false;
            }
            finally
            {
                // in any case, flush and close stream
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                }
            }
            // return true if all went well, else false
            return saved;
        }

        public static AppSettings getInstance()
        {
            if (Instance == null)
                Instance = new AppSettings();
            return Instance;
        }
    }
}
