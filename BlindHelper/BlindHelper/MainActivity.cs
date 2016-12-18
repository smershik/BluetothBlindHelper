using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;


using System.Linq;
using Java.Util;

namespace BlindHelper
{
    [Activity(Label = "BlindHelper", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button buttonConnect = FindViewById<Button>(Resource.Id.button1);
            Button buttonDisconnect = FindViewById<Button>(Resource.Id.button2);

            buttonDisconnect.Enabled = false;

            Button button1On = FindViewById<Button>(Resource.Id.button3);
            Button button2On = FindViewById<Button>(Resource.Id.button4);

            Button button3On = FindViewById<Button>(Resource.Id.button5);
            Button buttonOff = FindViewById<Button>(Resource.Id.button6);

            SeekBar brightness = FindViewById<SeekBar>(Resource.Id.seekBar1);

            SeekBar rot = FindViewById<SeekBar>(Resource.Id.seekBar2);

            SeekBar blau = FindViewById<SeekBar>(Resource.Id.seekBar4);

            SeekBar gelb = FindViewById<SeekBar>(Resource.Id.seekBar3);

            TextView connected = FindViewById<TextView>(Resource.Id.textView1);

            BluetoothSocket _socket = null;
            BluetoothDevice _device = null;


            buttonConnect.Click += async delegate
            {

                BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
                if (adapter == null)
                    throw new Exception("No Bluetooth adapter found.");

                if (!adapter.IsEnabled)
                    throw new Exception("Bluetooth adapter is not enabled.");

                adapter.StartDiscovery();


                _device = (from bd in adapter.BondedDevices
                                          where bd.Name == "BLINDHELPER"
                                          select bd).FirstOrDefault();

                if (_device == null)
                    throw new Exception("Named device not found.");

                _socket = _device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                await _socket.ConnectAsync();
                buttonConnect.Enabled = false;
                buttonDisconnect.Enabled = true;

            };

            buttonDisconnect.Click += delegate {

                try
                {
                    //  buttonDisconnect.Enabled = false;
                    buttonConnect.Enabled = true;


                    _device.Dispose();

                    //myConnection.thisSocket.OutputStream.WriteByte(187);
                    //myConnection.thisSocket.OutputStream.Close();

                    _socket.Close();

                    connected.Text = "Disconnected!";
                }
                catch { throw new Exception("not disconnected"); }
            };

        }

        


    }
}

