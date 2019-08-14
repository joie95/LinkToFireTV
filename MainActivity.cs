using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Java.Lang;
using Android.Content;
using Android.Net;
using Sockets.Plugin;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Android.Support.Design.Widget;
using Xamarin;

namespace LinkToFireTV
{
    [Activity(Name = "linktofiretv.MainActivity", Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //UI Elemente suchen
            Button button1 = FindViewById<Button>(Resource.Id.button1);
            TextInputEditText textInputEditText1 = FindViewById<TextInputEditText>(Resource.Id.textInputEditText1);

            //IP-Adresse laden
            retrieveset();

            //Übergeben Uri überprüfen
            Uri uri = Intent.Data;
            string uriStr = "";

            //Falls Uri nicht leer --> Sende Uri und schließe danach
            if (uri != null)
            {
                uriStr = uri.ToString();
                //UDP Senden
                var port = 15000;
                var address = textInputEditText1.Text;
                var client = new UdpSocketClient();
                var msgBytes = Encoding.UTF8.GetBytes(uriStr);

                await client.SendToAsync(msgBytes, address, port);

                RunOnUiThread(() => Toast.MakeText(this, "Link gesendet!", ToastLength.Long).Show());

                Finish();
            }
            //Ansonsten --> bleibe offen
            else
            {
                //Button Klick --> Acitivity schließen
                button1.Click += delegate {

                    saveset();
                    Finish();
                };

            }
    
        }


        protected void saveset()
        {
            //IP speichern
            var prefs = Application.Context.GetSharedPreferences("LinkToFireTV", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("ip-adress", FindViewById<TextInputEditText>(Resource.Id.textInputEditText1).Text);
            prefEditor.Commit();

            Toast.MakeText(this, "Gespeichert!", ToastLength.Long).Show();

        }

        protected void retrieveset()
        {
            //IP-Adresse laden 
            var prefs = Application.Context.GetSharedPreferences("LinkToFireTV", FileCreationMode.Private);
            var ipStr = prefs.GetString("ip-adress", null);

            if (string.IsNullOrEmpty(ipStr) == false)
            {
                RunOnUiThread(() => {
                    FindViewById<TextInputEditText>(Resource.Id.textInputEditText1).Text = ipStr;
                }); 
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}

