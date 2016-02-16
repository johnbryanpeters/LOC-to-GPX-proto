using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HelloWorld
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            greetingOutput.Text = "Hello, " + nameInput.Text + "!";
        }

        private async void bOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Let user choose a file.
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.SuggestedStartLocation =
                    Windows.Storage.Pickers.PickerLocationId.Desktop;
                // We only want to look for files with .loc extension.
                picker.FileTypeFilter.Add(".loc");
                // Your turn, user...
                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    // Open the file.
                    var stream = await file.OpenStreamForReadAsync();
                    // Application now has read/write access to the picked file.
                    // Stuff the whole file into a byte array.
                    var _ContentArray = new byte[(int)stream.Length];
                    stream.Read(_ContentArray, 0, (int)stream.Length);
                    // Instantiate a LocFile01 object, feeding it the array so 
                    // created.
                    LocFile01 _LocFile = new LocFile01(_ContentArray);
                    // Display information extracted by instantiation of object.
                    openedFile.Text = 
                        "Opened file: " + file.Name + "\n" + 
                        "Header: " + _LocFile.Header + "\n" +
                        "Waypoints:" + "\n" +
                            "Name on GPS\t" + 
                            "Label on map\t" + 
                            "Latitude\t" + 
                            "Longitude\t\n";
                    foreach(LocFile01.Waypoint w in _LocFile.Waypoints)
                    {
                        openedFile.Text = openedFile.Text + 
                            w.NameOnGPS  + "\t" + 
                            w.LabelOnMap + "\t" + 
                            w.Latitude   + "\t" + 
                            w.Longitude  + "\n";
                    }
                    int foo = _LocFile.Tracks.Count;
                    if(_LocFile.Tracks.Count > 0)
                    {
                        openedFile.Text = openedFile.Text +
                            "Track(s):" + "\n";
                        foreach(var t in _LocFile.Tracks)
                        {
                            openedFile.Text = openedFile.Text +
                                "Trackpoints in track " + t.LabelOnMap + ":" + "\n" +
                                "Latitude\t" +
                                "Longitude\t\n";
                            //foreach (var tp in t)
                            //{
                            //    openedFile.Text = openedFile.Text +
                            //}
                        }
                    }
                }
                else
                {
                    openedFile.Text = "File selection cancelled.";
                }
            }
            catch(InvalidOperationException ioe)
            {
                openedFile.Text = "Exception encountered in .LOC file.\n" +
                                  ioe.Message;
            }
            catch (Exception exc)
            {
                openedFile.Text = "Exception: " + exc.Message;
            }
            finally { }
        }
    }
}