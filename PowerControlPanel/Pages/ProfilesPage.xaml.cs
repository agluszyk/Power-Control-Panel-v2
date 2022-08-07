﻿using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media;

namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for ProfilesPage.xaml
    /// </summary>
    public partial class ProfilesPage : Page
    {
        private Classes.ManageXML.ManageXML_Profiles xmlP;
        private Classes.ManageXML.ManageXML_Apps xmlA;
        private string ProfileName = "";
        public ProfilesPage()
        {
            InitializeComponent();

            //initialize xml management class
            xmlP = new Classes.ManageXML.ManageXML_Profiles();
            xmlA = new Classes.ManageXML.ManageXML_Apps();
            //populate profile list
            loadProfileListView();
            loadAppListView();
            //change theme to match general theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);

            //hide AMD specific stuff if cpu is intel
            if (GlobalVariables.cpuType =="Intel")
            {
                online_GPUCLK_DP.Visibility = Visibility.Collapsed;
                offline_GPUCLK_DP.Visibility = Visibility.Collapsed;
            }


            //set max tdp on sliders
            offline_sliderTDP1.Maximum = Properties.Settings.Default.maxTDP;
            offline_sliderTDP2.Maximum = Properties.Settings.Default.maxTDP;
            online_sliderTDP1.Maximum = Properties.Settings.Default.maxTDP;
            online_sliderTDP2.Maximum = Properties.Settings.Default.maxTDP;
        }

        private void loadProfileListView()
        {
            
            DataTable dt = xmlP.profileList();
            profileDataGrid.DataContext = dt.DefaultView;
           
        }
        private void loadAppListView()
        {

            DataTable dt = xmlA.appList();
            appDataGrid.DataContext = dt.DefaultView;

        }

        private void btnAddProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            xmlP.createProfile();
            loadProfileListView();
        }

        private void btnDeleteProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            bool deleteProfile = true;



            if (ProfileName == "Default")
            {
                if (System.Windows.Forms.MessageBox.Show("Deleting the Default profile will disable having a default. Do you still want to delete it?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    deleteProfile = false;
                }


            }
            
            if (deleteProfile == true)
            {
                xmlP.deleteProfile(ProfileName);
                loadProfileListView();
                clearProfile();
                if (GlobalVariables.ActiveProfile == ProfileName)
                {
                    GlobalVariables.ActiveProfile = "None";

                }
            }

        }
        private void saveProfile()
        {
            if (ProfileName != "")
            {


              
                string[] result = new string[6];


                if (toggle_Online_TDP1.IsOn == true)
                {
                    result[0] = online_sliderTDP1.Value.ToString();

                }
                else { result[0] = ""; }

                if (toggle_Online_TDP2.IsOn == true)
                {
                    result[1] = online_sliderTDP2.Value.ToString();

                }
                else { result[1] = ""; }


                if (toggle_Online_GPUCLK.IsOn == true)
                {
                    result[2] = online_sliderGPUCLK.Value.ToString();

                }
                else { result[2] = ""; }




                if (toggle_Offline_TDP1.IsOn == true)
                {
                    result[3] = offline_sliderTDP1.Value.ToString();

                }
                else { result[3] = ""; }

                if (toggle_Offline_TDP2.IsOn == true)
                {
                    result[4] = offline_sliderTDP2.Value.ToString();

                }
                else { result[4] = ""; }

                if (toggle_Offline_GPUCLK.IsOn == true)
                {
                    result[5] = offline_sliderGPUCLK.Value.ToString();

                }
                else { result[5] = ""; }

                xmlP.saveProfileArray(result,ProfileName);

                //check if profile name has changed! if yes, update any applications or active profiles with new name
                if (ProfileName != txtbxProfileName.Text)
                {
                    //if not match, then name was changed. Update profile name in profiles  section of XML. Update all apps with profilename
                    xmlP.changeProfileName(ProfileName, txtbxProfileName.Text);
                    xmlA.changeProfileNameInApps(ProfileName, txtbxProfileName.Text);

                    loadProfileListView();

                    //if active profile name is the one changed, then update profile
                    if (GlobalVariables.ActiveProfile == ProfileName) 
                    { 
                        GlobalVariables.ActiveProfile = txtbxProfileName.Text;
                        System.Windows.MessageBox.Show("MAKE CODE TO RUN PROFILE AFTER PROFILE UPDATE");
                    }
                }

                System.Windows.MessageBox.Show("Profile Saved");
            }

        }
        private void loadProfile()
        {
            if (ProfileName != "")
            {

                txtbxProfileName.Text = ProfileName;
                string[] result = xmlP.loadProfileArray(ProfileName);



                DataTable dt = xmlA.appListByProfile(ProfileName);
                profileAppDataGrid.DataContext = dt.DefaultView;

                if (result[0] != string.Empty)
                {
                    toggle_Online_TDP1.IsOn= true;
                    online_sliderTDP1.Value = Int32.Parse(result[0]);
                }
                else
                {
                    toggle_Online_TDP1.IsOn = false;
                    online_sliderTDP1.Value = online_sliderTDP1.Minimum;
                }



                if (result[1] != string.Empty)
                {
                    toggle_Online_TDP2.IsOn = true;
                    online_sliderTDP2.Value = Int32.Parse(result[1]);
                }
                else
                {
                    toggle_Online_TDP2.IsOn = false;
                    online_sliderTDP2.Value = online_sliderTDP2.Minimum;
                }

                if (result[2] != string.Empty)
                {
                    toggle_Online_GPUCLK.IsOn = true;
                    online_sliderGPUCLK.Value = Int32.Parse(result[2]);
                }
                else
                {
                    toggle_Online_GPUCLK.IsOn = false;
                    online_sliderGPUCLK.Value = online_sliderTDP2.Minimum;
                }



                if (result[3] != string.Empty)
                {
                    toggle_Offline_TDP1.IsOn = true;
                    offline_sliderTDP1.Value = Int32.Parse(result[3]);
                }
                else
                {
                    toggle_Offline_TDP1.IsOn = false;
                    offline_sliderTDP1.Value = offline_sliderTDP1.Minimum;
                }


                if (result[4] != string.Empty)
                {
                    toggle_Offline_TDP2.IsOn = true;
                    offline_sliderTDP2.Value = Int32.Parse(result[4]);
                }
                else
                {
                    toggle_Offline_TDP2.IsOn = false;
                    offline_sliderTDP2.Value = offline_sliderTDP1.Minimum;
                }

                if (result[5] != string.Empty)
                {
                    toggle_Offline_GPUCLK.IsOn = true;
                    offline_sliderGPUCLK.Value = Int32.Parse(result[5]);
                }
                else
                {
                    toggle_Offline_GPUCLK.IsOn = false;
                    offline_sliderGPUCLK.Value = offline_sliderTDP2.Minimum;
                }


            }

        }
        private void clearProfile()
        {
            txtbxProfileName.Text = string.Empty;
            offline_sliderTDP1.Value = offline_sliderTDP1.Minimum;
            offline_sliderTDP2.Value = offline_sliderTDP2.Minimum;
            online_sliderTDP1.Value = online_sliderTDP1.Minimum;
            online_sliderTDP2.Value = online_sliderTDP2.Minimum;
            offline_sliderGPUCLK.Value = offline_sliderGPUCLK.Minimum;
            online_sliderGPUCLK.Value = online_sliderGPUCLK.Minimum;

            toggle_Offline_TDP1.IsOn = false;
            toggle_Offline_TDP2.IsOn = false;
            toggle_Online_TDP1.IsOn = false;
            toggle_Online_TDP2.IsOn=false;
            toggle_Offline_GPUCLK.IsOn = false;
            toggle_Online_GPUCLK.IsOn=false;

        }

        private void ToggleSwitch_Toggled(object sender, System.Windows.RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                string command = toggleSwitch.CommandParameter.ToString();
                string charger = command.Substring(0, command.IndexOf("_"));
                string parameter = command.Substring(command.IndexOf("_") + 1, command.Length - command.IndexOf("_") - 1);
                if (ProfileName != "")
                {



                    if (!toggleSwitch.IsOn)
                    {
                        DockPanel parentDP = getParentDockPanel(toggleSwitch);
                      

                        foreach (System.Windows.Controls.Control child in parentDP.Children)
                        {
                            if (child is Slider)
                            {
                                child.IsEnabled = false;
                                child.Opacity = 0.3;
                            }
                        }



                    }
                    else
                    {
                        DockPanel parentDP = getParentDockPanel(toggleSwitch);
        
                        xmlP.changeProfileParameter(charger, parameter, ProfileName, "");
                        foreach (System.Windows.Controls.Control child in parentDP.Children)
                        {
                            if (child is Slider)
                            {
                                child.IsEnabled = true;
                                child.Opacity = 1;
                            }
                        }

                    }
                }
            }
        }


        private DockPanel getParentDockPanel(DependencyObject toggle)
        {

            DockPanel returnDP = null;
            DependencyObject parent;
            if (toggle != null)
            {
                parent = VisualTreeHelper.GetParent(toggle);

                if (parent is not DockPanel)
                {
                    parent = VisualTreeHelper.GetParent(parent);
                    if (parent is not DockPanel)
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }
                }

                returnDP = (DockPanel)parent;



            }

            return returnDP;


        }

        private void profileDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
     
            object item = profileDataGrid.SelectedItem;
            if (item != null)
            {
                string profileName = (profileDataGrid.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;

                if (profileName != null)
                { ProfileSP.IsEnabled = true;
                    ProfileName = profileName;
                    loadProfile();
                }
                else { ProfileSP.IsEnabled = false;
                    ProfileName = "";
                    clearProfile();
                }

            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            saveProfile();
        }

        private void ButtonSwitchViews_Click(object sender, RoutedEventArgs e)
        {
            if (GB_Apps.Visibility== Visibility.Collapsed)
            { 
                GB_Apps.Visibility = Visibility.Visible;
                GB_Profiles.Visibility= Visibility.Collapsed;
            }
            else
            {
                GB_Apps.Visibility = Visibility.Collapsed;
                GB_Profiles.Visibility = Visibility.Visible;
            }
        }
    }
}
