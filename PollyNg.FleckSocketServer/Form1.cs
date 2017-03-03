using Fleck;
using Jil;
using PollyNg.API;
using PollyNg.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PollyNg.FleckSocketServer
{
    public partial class Form1 : Form
    {
        private chatContainer db = new chatContainer();
        private List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        WebSocketServer server = new WebSocketServer(BaseConfig.SocketAddress);
        RedisHelper helper = null;
        public Form1()
        {
            InitializeComponent();
            this.btnStop.Enabled = false;
            FleckLog.Level = LogLevel.Debug;
            //注册缓存信息
            RedisManager.ConfigurationOption = BaseConfig.RedisPath;
            helper = new RedisHelper();
        }
        private delegate void SetTextCallback(string text, Color color);
        //默认输出文本
        private void show(string content)
        {
            show(content, DefaultForeColor);
        }
        //带颜色输出文本
        private void show(string content, Color color)
        {
            if (this.rtbContent.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(show);
                this.Invoke(d, new object[] { content, color });
            }
            else
            {
                var lenLo = this.rtbContent.TextLength ;
                this.rtbContent.AppendText(content);
                var lenHi = this.rtbContent.TextLength - 1;
                this.rtbContent.Select(lenLo, lenHi);
                this.rtbContent.SelectionColor = color;
            }
        }
        private string responseText(string content)
        {
            var response = new MessageModel();
            response.content = content;
            response.type = Type.message;
            return JSON.Serialize(response);
        }
        private void saveMessage(MessageModel mess)
        {
            var messToSave = new message();
            messToSave.type = mess.type.ToString();
            messToSave.content = mess.content;
            messToSave.url = mess.url;
            db.message.Add(messToSave);
            db.SaveChanges();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            server = new WebSocketServer(BaseConfig.SocketAddress);
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    allSockets.Add(socket);
                    show(string.Format("{3}:有新人加入!IpAddress:{0} ClientPort:{1} 当前{2}人\n",
                        socket.ConnectionInfo.ClientIpAddress,
                        socket.ConnectionInfo.ClientPort,
                        allSockets.Count,
                        DateTime.Now.ToString()), Color.Blue);
                    allSockets.ToList().ForEach(s => s.Send(responseText(string.Format("有新人加入，当前{0}人", allSockets.Count))));
                };
                socket.OnClose = () =>
                {
                    allSockets.Remove(socket);
                    show(string.Format("{3}:有人离开!IpAddress:{0} ClientPort:{1} 当前{2}人\n",
                        socket.ConnectionInfo.ClientIpAddress,
                        socket.ConnectionInfo.ClientPort,
                        allSockets.Count,
                        DateTime.Now.ToString()), Color.Blue);
                    allSockets.ToList().ForEach(s => s.Send(responseText(string.Format("有人离开，当前{0}人", allSockets.Count))));
                };
                socket.OnMessage = message =>
                {
                    var response = "偷偷告诉你:你已经发送成功";
                    var mess = JSON.Deserialize<MessageModel>(message);
                    var socketId = socket.ConnectionInfo.Id.ToString();
                    show(string.Format("{0}:消息!IpAddress:{1} ClientPort:{2} 消息:{3}-{4}-{5}-{6}\n",
                        DateTime.Now.ToString(),
                         socket.ConnectionInfo.ClientIpAddress,
                         socket.ConnectionInfo.ClientPort,
                         mess.type,
                         mess.content,
                         mess.url,
                         mess.nickname
                         ));
                    try {
                        saveMessage(mess);
                    }
                    catch
                    {
                    }
                    finally
                    {
                    }
                    if (mess.type != Type.login) {
                        mess.nickname = helper.HashGet<MessageModel>("login", socketId).nickname;
                        allSockets.ToList().ForEach(s => s.Send(JSON.Serialize(mess)));
                    }
                    //登录信息
                    else
                    {
                        //在缓存中存储登录信息
                        response = "偷偷告诉你:你已经改名成功";
                        helper.HashSet("login", socketId, mess);
                    }
                    socket.Send(responseText(response));
                };
            });
            show(string.Format("{0}:成功启动\n", DateTime.Now.ToString()), Color.Green);
            this.btnStart.Enabled = false;
            this.btnStop.Enabled = true;
            this.btnGroupSend.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            foreach (var socket in allSockets.ToList())
            {
                socket.Close();
            }
            server.Dispose();

            show(string.Format("{0}:成功停用\n", DateTime.Now.ToString()), Color.HotPink);

            this.btnGroupSend.Enabled = false;
            this.btnStop.Enabled = false;
            this.btnStart.Enabled = true;
        }

        private void btnGroupSend_Click(object sender, EventArgs e)
        {
            var input = this.tbContent.Text;

            if (input != "exit" && input != "")
            {
                show(string.Format("{0}:准备群发 {1}\n", DateTime.Now.ToString(), input), Color.Gray);
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(responseText(input));
                }
                show(string.Format("{0}:群发成功\n", DateTime.Now.ToString()), Color.Green);
            }
        }

        private void rtbContent_TextChanged(object sender, EventArgs e)
        {
            this.rtbContent.SelectionStart = this.rtbContent.TextLength;
            this.rtbContent.ScrollToCaret();
        }
    }
}
