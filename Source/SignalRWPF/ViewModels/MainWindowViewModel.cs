using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using SignalRWPF.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading;
using System;
using System.Windows;
using System.Diagnostics;

namespace SignalRWPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        private ObservableCollection<ChatMessage> _messages = new();
        public ObservableCollection<ChatMessage> Messages
        {
            get { return _messages; }
            set { SetProperty(ref _messages, value); }
        }

        private readonly HubConnection _hubConnection;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public MainWindowViewModel(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
            _hubConnection.StartAsync();
        }

        private DelegateCommand _sendMessage;
        public DelegateCommand SendMessage =>
            _sendMessage ?? (_sendMessage = new DelegateCommand(ExecuteSendMessage));
        async void ExecuteSendMessage()
        {
            _messages.Clear();
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                var stream = _hubConnection.StreamAsync<string>("GenerateAnswerStream", _content, _cancellationTokenSource.Token);
                await foreach (var content in stream)
                {
                    var message = new ChatMessage(content);
                    _messages.Add(message);
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        private DelegateCommand _stopCommand;


        public DelegateCommand StopCommand =>
            _stopCommand ?? (_stopCommand = new DelegateCommand(ExecuteStopCommand));
        void ExecuteStopCommand()
        {
            _cancellationTokenSource.Cancel();
        }


    }
}
