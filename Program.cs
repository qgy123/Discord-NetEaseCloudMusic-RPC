using System;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace NetEaseDcNpPlugin
{
    public class Program
    {
        private static DiscordRpc.RichPresence _presence = new DiscordRpc.RichPresence();
        private static DiscordRpc.EventHandlers _handlers = new DiscordRpc.EventHandlers();

        private static string _lastPlaying = "";

        static void Main(string[] args)
        {
            //RegCallBack();

            DiscordRpc.Initialize(Const.ApplicationId, ref _handlers, false, null);

            Timer timer = new Timer(1000);
            timer.Elapsed += Tick;
            timer.Start();

            Console.ReadLine();
        }

        private static void Tick(object sender, ElapsedEventArgs e)
        {
            Update();
        }

        private static void RegCallBack()
        {
            _handlers.readyCallback += ReadyCallback;
            _handlers.disconnectedCallback += DisconnectedCallback;
            _handlers.errorCallback += ErrorCallback;
        }

        private static void Update()
        {
            string now = GetNetEaseNowPlaying();

            if (!string.IsNullOrWhiteSpace(now))
            {
                _presence.details = "Now Listening:";
                _presence.state = now;
                _presence.largeImageKey = "icon";
                _presence.largeImageText = "NetEaseMusic";
                _presence.startTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                if (_lastPlaying != now)
                {
                    _lastPlaying = now;
                    DiscordRpc.UpdatePresence(_presence);
                    Console.WriteLine(now);
                }
            }

        }

        private static string GetNetEaseNowPlaying()
        {
            string text = "";

            Native.EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                StringBuilder str = new StringBuilder(256);
                Native.GetClassName(hWnd, str, 256);

                if (str.ToString() == "OrpheusBrowserHost")
                {
                    int length = Native.GetWindowTextLength(hWnd);
                    StringBuilder builder = new StringBuilder(length);
                    Native.GetWindowText(hWnd, builder, length + 1);
                    text = builder.ToString();
                }

                return true;

            }, IntPtr.Zero);

            return text;
        }


        private static void ErrorCallback(int errorCode, string message)
        {
            Console.WriteLine("ErrorCallback : " + errorCode);
        }

        private static void DisconnectedCallback(int errorCode, string message)
        {
            Console.WriteLine("Disconnected Callback Error Code : " + errorCode);
        }

        private static void ReadyCallback(ref DiscordRpc.DiscordUser connectedUser)
        {
            Console.WriteLine("Connected!");
            Console.WriteLine($"User + {connectedUser.username}");
            Console.WriteLine($"userId + {connectedUser.userId}");
            Console.WriteLine($"discriminator + {connectedUser.discriminator}");
        }
    }
}
