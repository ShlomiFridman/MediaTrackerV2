﻿#pragma checksum "..\..\..\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "32AABF5EEBA94013A02B3FFC9CB07C109722999B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MediaTracker;
using MediaTracker.MyClasses;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MediaTracker {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 45 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ColumnDefinition treeCol;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ColumnDefinition trackerCol;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox rootTextBox;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox searchTextBox;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button clearSearchBoxBtn;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button refreshBtn;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView TreeView;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GridSplitter GridSplitter;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel mainUI;
        
        #line default
        #line hidden
        
        
        #line 125 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander settingsExpander;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox autoAdvanceSetting;
        
        #line default
        #line hidden
        
        
        #line 140 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox OnOpenComboBox;
        
        #line default
        #line hidden
        
        
        #line 157 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox selectedFolderText;
        
        #line default
        #line hidden
        
        
        #line 167 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox selectedFileText;
        
        #line default
        #line hidden
        
        
        #line 176 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox selectedFileFolderText;
        
        #line default
        #line hidden
        
        
        #line 181 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PreviousFolderBtn;
        
        #line default
        #line hidden
        
        
        #line 183 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OpenFolderBtn;
        
        #line default
        #line hidden
        
        
        #line 185 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button NextFolderBtn;
        
        #line default
        #line hidden
        
        
        #line 191 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Previous5FileBtn;
        
        #line default
        #line hidden
        
        
        #line 193 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PreviousFileBtn;
        
        #line default
        #line hidden
        
        
        #line 195 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OpenFileBtn;
        
        #line default
        #line hidden
        
        
        #line 197 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button NextFileBtn;
        
        #line default
        #line hidden
        
        
        #line 199 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Next5FileBtn;
        
        #line default
        #line hidden
        
        
        #line 203 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander randomExpander;
        
        #line default
        #line hidden
        
        
        #line 206 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox RandomText;
        
        #line default
        #line hidden
        
        
        #line 217 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OpenRandomBtn;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.8.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MediaTracker;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.8.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\MainWindow.xaml"
            ((MediaTracker.MainWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\MainWindow.xaml"
            ((MediaTracker.MainWindow)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.treeCol = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 3:
            this.trackerCol = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 4:
            
            #line 63 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.selectRootBtn_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.rootTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.searchTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 77 "..\..\..\MainWindow.xaml"
            this.searchTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.clearSearchBoxBtn = ((System.Windows.Controls.Button)(target));
            
            #line 84 "..\..\..\MainWindow.xaml"
            this.clearSearchBoxBtn.Click += new System.Windows.RoutedEventHandler(this.clearSearchBoxBtn_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.refreshBtn = ((System.Windows.Controls.Button)(target));
            
            #line 88 "..\..\..\MainWindow.xaml"
            this.refreshBtn.Click += new System.Windows.RoutedEventHandler(this.refreshTreeView_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.TreeView = ((System.Windows.Controls.TreeView)(target));
            return;
            case 10:
            this.GridSplitter = ((System.Windows.Controls.GridSplitter)(target));
            return;
            case 11:
            this.mainUI = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 12:
            this.settingsExpander = ((System.Windows.Controls.Expander)(target));
            
            #line 126 "..\..\..\MainWindow.xaml"
            this.settingsExpander.Expanded += new System.Windows.RoutedEventHandler(this.SettingsControl_StateChange);
            
            #line default
            #line hidden
            
            #line 127 "..\..\..\MainWindow.xaml"
            this.settingsExpander.Collapsed += new System.Windows.RoutedEventHandler(this.SettingsControl_StateChange);
            
            #line default
            #line hidden
            return;
            case 13:
            this.autoAdvanceSetting = ((System.Windows.Controls.CheckBox)(target));
            
            #line 134 "..\..\..\MainWindow.xaml"
            this.autoAdvanceSetting.Checked += new System.Windows.RoutedEventHandler(this.autoAdvanceSetting_Checked);
            
            #line default
            #line hidden
            
            #line 135 "..\..\..\MainWindow.xaml"
            this.autoAdvanceSetting.Unchecked += new System.Windows.RoutedEventHandler(this.autoAdvanceSetting_Checked);
            
            #line default
            #line hidden
            return;
            case 14:
            this.OnOpenComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 140 "..\..\..\MainWindow.xaml"
            this.OnOpenComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.OnOpenComboBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 15:
            this.selectedFolderText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 16:
            this.selectedFileText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 17:
            this.selectedFileFolderText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 18:
            this.PreviousFolderBtn = ((System.Windows.Controls.Button)(target));
            
            #line 181 "..\..\..\MainWindow.xaml"
            this.PreviousFolderBtn.Click += new System.Windows.RoutedEventHandler(this.ButtonClick_ChangeFolder);
            
            #line default
            #line hidden
            return;
            case 19:
            this.OpenFolderBtn = ((System.Windows.Controls.Button)(target));
            
            #line 183 "..\..\..\MainWindow.xaml"
            this.OpenFolderBtn.Click += new System.Windows.RoutedEventHandler(this.ButtonClick_open);
            
            #line default
            #line hidden
            return;
            case 20:
            this.NextFolderBtn = ((System.Windows.Controls.Button)(target));
            
            #line 185 "..\..\..\MainWindow.xaml"
            this.NextFolderBtn.Click += new System.Windows.RoutedEventHandler(this.ButtonClick_ChangeFolder);
            
            #line default
            #line hidden
            return;
            case 21:
            this.Previous5FileBtn = ((System.Windows.Controls.Button)(target));
            
            #line 191 "..\..\..\MainWindow.xaml"
            this.Previous5FileBtn.Click += new System.Windows.RoutedEventHandler(this.ButtonClick_ChangeFile);
            
            #line default
            #line hidden
            return;
            case 22:
            this.PreviousFileBtn = ((System.Windows.Controls.Button)(target));
            
            #line 193 "..\..\..\MainWindow.xaml"
            this.PreviousFileBtn.Click += new System.Windows.RoutedEventHandler(this.ButtonClick_ChangeFile);
            
            #line default
            #line hidden
            return;
            case 23:
            this.OpenFileBtn = ((System.Windows.Controls.Button)(target));
            
            #line 195 "..\..\..\MainWindow.xaml"
            this.OpenFileBtn.Click += new System.Windows.RoutedEventHandler(this.ButtonClick_open);
            
            #line default
            #line hidden
            return;
            case 24:
            this.NextFileBtn = ((System.Windows.Controls.Button)(target));
            
            #line 197 "..\..\..\MainWindow.xaml"
            this.NextFileBtn.Click += new System.Windows.RoutedEventHandler(this.ButtonClick_ChangeFile);
            
            #line default
            #line hidden
            return;
            case 25:
            this.Next5FileBtn = ((System.Windows.Controls.Button)(target));
            
            #line 199 "..\..\..\MainWindow.xaml"
            this.Next5FileBtn.Click += new System.Windows.RoutedEventHandler(this.ButtonClick_ChangeFile);
            
            #line default
            #line hidden
            return;
            case 26:
            this.randomExpander = ((System.Windows.Controls.Expander)(target));
            
            #line 204 "..\..\..\MainWindow.xaml"
            this.randomExpander.Expanded += new System.Windows.RoutedEventHandler(this.randomExpander_Toggled);
            
            #line default
            #line hidden
            
            #line 204 "..\..\..\MainWindow.xaml"
            this.randomExpander.Collapsed += new System.Windows.RoutedEventHandler(this.randomExpander_Toggled);
            
            #line default
            #line hidden
            return;
            case 27:
            this.RandomText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 28:
            
            #line 216 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.GenRandomBtn_Click);
            
            #line default
            #line hidden
            return;
            case 29:
            this.OpenRandomBtn = ((System.Windows.Controls.Button)(target));
            
            #line 218 "..\..\..\MainWindow.xaml"
            this.OpenRandomBtn.Click += new System.Windows.RoutedEventHandler(this.ButtonClick_open);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

