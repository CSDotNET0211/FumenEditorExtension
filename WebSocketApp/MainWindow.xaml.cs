using MisaMinoNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace WebSocketApp
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<byte, TetrisData> _tetrisData = new Dictionary<byte, TetrisData>();

        WebSocket _wcFumenEditor = null;
        WebSocket _wcTetrio = null;
        System.Windows.Forms.NotifyIcon NotifyIcon;

        bool alwaysRefresh = false;
        int sendPlayerIndex = 0;
        int refreshTime = 500;

        enum Command
        {
            AIRequest,
            AIResponse,
        }

        enum AIKind
        {
            MisaMino,
            Zetris,
            ColdClear,
            CSBotNET,

        }



        const byte VERSION_INFO = 1;

        enum ConnectionID : byte
        {
            FumenEditor,
            Tetrio
        }
        public MainWindow()
        {
            InitializeComponent();

            NotifyIcon = new NotifyIcon();
            var menu = new ContextMenuStrip();

            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = "TETR.IO 〇 Not Connected";
            menu.Items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Text = "FumenEditor 〇 Not Connected";
            menu.Items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Text = "Show";
            menu.Items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Text = "Launch Image-Recognition";
            menu.Items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Text = "Exit";
            menu.Items.Add(menuItem);

            NotifyIcon.Text = "FieldEditor Extension";
            NotifyIcon.Visible = false;
            NotifyIcon.ContextMenuStrip = menu;

            Init();
        }

        async void Init()
        {
            //Httpリスナーを立ち上げ、クライアントからの接続を待つ
            HttpListener s = new HttpListener();
            s.Prefixes.Add("http://localhost:8000/ws/");
            s.Start();

            HttpListenerContext hc;
            while (true)
            {
                Debug.WriteLine("Start Listening...");
                hc = await s.GetContextAsync();
                Debug.WriteLine("Client Found.");
                if (!hc.Request.IsWebSocketRequest)
                {
                    Debug.WriteLine("connected but Closed by not WebSocket request");
                    //クライアント側にエラー(400)を返却し接続を閉じる
                    hc.Response.StatusCode = 400;
                    hc.Response.Close();
                    return;
                }

                Debug.WriteLine("Connected with WebSocket");
                //クライアントからのリクエストがWebSocketでない場合は処理を中断

                //WebSocketでレスポンスを返却
                var wsc = await hc.AcceptWebSocketAsync(null);
                WebSocket ws = wsc.WebSocket;

                var buff = new ArraySegment<byte>(new byte[10]);
                var result = await ws.ReceiveAsync(buff, CancellationToken.None);

                for (int i = 0; i < result.Count; i++)
                    Debug.Write(buff[i] + " ");
                Debug.WriteLine("Received the Identification Value:" + (buff[0] - 48));

                if (buff[0] - 48 == (byte)ConnectionID.FumenEditor)
                {
                    _wcFumenEditor = ws;
                    StatusLightFumenEditor.Fill = Brushes.Green;
                    StatusTextFumenEditor.Text = "Connected";

                    //文字列をByte型に変換  
                    var templist = new List<byte>();
                    templist.Add(VERSION_INFO);
                    templist.AddRange(Encoding.UTF8.GetBytes("v0.1"));
                    var segment = new ArraySegment<byte>(templist.ToArray());

                    //クライアント側に文字列を送信
                    await ws.SendAsync(segment, WebSocketMessageType.Binary,
                      true, CancellationToken.None);

                    Task.Run(FumenEditorReceiver);
                }
                else if (buff[0] - 48 == (byte)ConnectionID.Tetrio)
                {
                    _wcTetrio = ws;
                    StatusLightTetrio.Fill = Brushes.Green;
                    StatusTextTetrio.Text = "Connected";
                    Task.Run(TetrioReceiver);
                }

            }
        }

        async void TetrioReceiver()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                var textForOutput = "";
                var buff = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result;
                try
                {
                    result = await _wcTetrio.ReceiveAsync(buff, CancellationToken.None);
                }
                catch
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        StatusLightTetrio.Fill = Brushes.Red;
                        StatusTextTetrio.Text = "Not Connected";
                    }));
                    return;
                }

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var str = Encoding.UTF8.GetString(buff.Take(result.Count).ToArray());
                    textForOutput = str;
                    // Debug.WriteLine("Received the Message From TETRIO: " + str);
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    //   Debug.WriteLine("Received the Binary From TETRIO");
                    for (int i = 0; i < result.Count; i++)
                        textForOutput += buff[i] + " ";
                }

                if (_wcFumenEditor != null && _wcFumenEditor.State == WebSocketState.Open && stopwatch.ElapsedMilliseconds > refreshTime)
                {

                    var segment = new ArraySegment<byte>(buff.Take(result.Count).ToArray());
                    segment[0]=0;
                    //クライアント側に文字列を送信
                    await _wcFumenEditor.SendAsync(segment, WebSocketMessageType.Binary,
                      true, CancellationToken.None);

                    stopwatch.Restart();
                }

                Dispatcher.Invoke((Action)(() =>
                {
                    RecentDataFromTetrio.Text = textForOutput;
                }));
            }
        }

        async void FumenEditorReceiver()
        {
            while (true)
            {
                var textForOutput = "";
                var buff = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result;

                try
                {
                    result = await _wcFumenEditor.ReceiveAsync(buff, CancellationToken.None);
                }
                catch
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        StatusLightFumenEditor.Fill = Brushes.Red;
                        StatusTextFumenEditor.Text = "Not Connected";
                    }));
                    return;
                }




                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var str = Encoding.UTF8.GetString(buff.Take(result.Count).ToArray());
                    textForOutput = str;
                    // Debug.WriteLine("Received the Message From TETRIO: " + str);
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {

                    //   Debug.WriteLine("Received the Binary From TETRIO");
                    for (int i = 0; i < result.Count; i++)
                        textForOutput += buff[i] + " ";

                    if (buff.Count == 0)
                        return;

                    switch ((Command)buff[0])
                    {
                        case Command.AIRequest:
                            switch ((AIKind)buff[1])
                            {
                                case AIKind.MisaMino:
                                    //   MisaMino.Finished += MisaMino_Finished;
                                    MisaMino misaMino = new MisaMino();
                                    var aimove = misaMino.GetBestResult(ConvertField(buff, 2), buff.Skip(2 + 200).Take(5).ToArray(), buff[207], buff[208]);

                                    aimove[0]=(byte)Command.AIResponse;
                                    var segment = new ArraySegment<byte>(aimove.ToArray());
                                   
                                    await _wcFumenEditor.SendAsync(segment, WebSocketMessageType.Binary,
                                      true, CancellationToken.None);
                                    break;

                                    case AIKind.Zetris:

                                    break;
                            }
                            break;
                    }


                }
                this.Dispatcher.Invoke((Action)(() =>
                {
                    RecentDataFromFumenEditor.Text = textForOutput;
                }));

            }

            byte[,] ConvertField(ArraySegment<byte> buffer, int offset)
            {
                var result = new byte[10, 20];

                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 20; y++)
                    {
                        result[x, y] = buffer[offset + x + y * 10];
                    }
                }

                return result;
            }
            
        }
    }
}
