using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Plugin.DeviceInfo;

namespace EasyChat
{
    public class Message
    {
        public string Text { get; set; }
        public DateTime MessagDateTime { get; set; }

        public bool IsIncoming => UserId != CrossDeviceInfo.Current.Id;

        public string Name { get; set; }
        public string UserId { get; set; }
    }
}

