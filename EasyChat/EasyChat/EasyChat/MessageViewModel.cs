using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace EasyChat
{
    public class MessageViewModel : INotifyPropertyChanged
	{
		private string text;

		public string Text
		{
			get { return text; }
			set { text = value; RaisePropertyChanged(); }
		}

		private DateTime messageDateTime;

		public DateTime MessagDateTime
		{
			get { return messageDateTime; }
			set { messageDateTime = value; RaisePropertyChanged(); }
		}

		private bool isIncoming;

		public bool IsIncoming
		{
			get { return isIncoming; }
			set { isIncoming = value; RaisePropertyChanged(); }
		}

		public bool HasAttachement => !string.IsNullOrEmpty(attachementUrl);

		private string attachementUrl;

        public event PropertyChangedEventHandler PropertyChanged;

        public string AttachementUrl
		{
			get { return attachementUrl; }
			set { attachementUrl = value; RaisePropertyChanged(); }
		}

		void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

