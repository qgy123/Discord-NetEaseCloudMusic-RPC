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

            Timer timer = new Timer(1000);
            timer.Elapsed += tick;
            timer.Start();

            Console.ReadLine();
        }

        private static void tick(object sender, ElapsedEventArgs e)
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
            DiscordRpc.Initialize(Const.ApplicationId, ref _handlers, false, null);

            string now = GetNetEaseNowPlaying(GetNetEaseProcessId());

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

        private static int GetNetEaseProcessId()
        {
            Process[] processes = Process.GetProcessesByName("cloudmusic");
            int pid = 0;

            foreach (var process in processes)
            {
                if (string.IsNullOrWhiteSpace(process.StartInfo.Arguments))
                {
                    pid = process.Id;
                }
            }

            return pid;
        }

        private static string GetNetEaseNowPlaying(int pid)
        {
            StringBuilder text = new StringBuilder(256);

            if (pid != 0)
            {
                IntPtr hwnd = Native.GetNetEaseWindowText(pid);
                if (hwnd != IntPtr.Zero)
                {
                    Native.GetWindowTextW(hwnd, text, Native.GetWindowTextLength(hwnd) + 1);
                }
            }

            return text.ToString();
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
