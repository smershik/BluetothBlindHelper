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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlindHelper
{
    [Activity(Label = "BlindHelper", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        BluetoothSocket _socket = null;
        BluetoothDevice _device = null;
        byte[] buffer = Encoding.ASCII.GetBytes("e");
        byte[] result = new byte[4];
        

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

            Button buttonStart = FindViewById<Button>(Resource.Id.Start);
            buttonStart.Enabled = false;

            TextView connected = FindViewById<TextView>(Resource.Id.textView1);
            TextView Distance = FindViewById<TextView>(Resource.Id.Distance);




            buttonConnect.Click += async delegate
            {
                buttonConnect.Enabled = false;
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
                buttonDisconnect.Enabled = true;
                buttonStart.Enabled = true;

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
                    buttonDisconnect.Enabled = false;
                    buttonStart.Enabled = false;
                    connected.Text = "Disconnected!";
                }
                catch { throw new Exception("not disconnected"); }
            };

            var sender = new Thread(() =>
            {
                while (true)
                {
                    _socket.OutputStream.Write(buffer, 0, buffer.Length);
                    Thread.Sleep(1000);
                }
            }
            );

            buttonStart.Click += delegate
            {

                sender.Start();
                System.Threading.Timer timer = new System.Threading.Timer((object o) =>
                {
                    _socket.InputStream.Read(result, 0, result.Length);
                    Distance.Text = Encoding.ASCII.GetString(result);
                },null,0,1000);
                
                //_socket.OutputStream.Write(buffer, 0, buffer.Length);
                //await _socket.InputStream.ReadAsync(result, 0, result.Length);
                //Distance.Text = Encoding.ASCII.GetString(result);
            }; 

                

            }

        public void StartSend()
        {
            while (true)
            {
                _socket.OutputStream.Write(buffer, 0, buffer.Length);
                System.Threading.Thread.Sleep(500);
            }
        }

        


    }
}

