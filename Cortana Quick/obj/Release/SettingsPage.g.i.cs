﻿#pragma checksum "C:\Users\Brian\documents\visual studio 2013\Projects\CortanaQuick\Cortana Quick\SettingsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "90C9D2C1D1DBF88937DE66A3895E5FDE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Cortana_Quick {
    
    
    public partial class SettingsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.DataTemplate ListPickerItemTemplate;
        
        internal System.Windows.DataTemplate ListPickerFullModeItemTemplate;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.ListPicker listPicker;
        
        internal System.Windows.Controls.CheckBox CheckDelete;
        
        internal System.Windows.Controls.TextBlock TextDelete;
        
        internal System.Windows.Controls.CheckBox CheckVerify;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Cortana%20Quick;component/SettingsPage.xaml", System.UriKind.Relative));
            this.ListPickerItemTemplate = ((System.Windows.DataTemplate)(this.FindName("ListPickerItemTemplate")));
            this.ListPickerFullModeItemTemplate = ((System.Windows.DataTemplate)(this.FindName("ListPickerFullModeItemTemplate")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.listPicker = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("listPicker")));
            this.CheckDelete = ((System.Windows.Controls.CheckBox)(this.FindName("CheckDelete")));
            this.TextDelete = ((System.Windows.Controls.TextBlock)(this.FindName("TextDelete")));
            this.CheckVerify = ((System.Windows.Controls.CheckBox)(this.FindName("CheckVerify")));
        }
    }
}

