using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChatPage : Page
    {
        private static readonly HttpClient client = new HttpClient();

        Dictionary<string, string> conversations = new();
        Dictionary<string, List<string>> messages = new();

        private static string selectedUser;

        public ChatPage()
        {
            client.DefaultRequestHeaders.Add("Authorization", Jwt.token);
            getMessagesTask();
            //getConversationsTask();
            this.InitializeComponent();
        }

        public static class UserToAdd
        {
            public static string username { get; set; }
        }
        private class MessageToSend
        {
            public string conversation_id { get; set; }
            public string message { get; set; }
        }
        private class UserAddReq
        {
            public string username { get; set; }
        }
        private class Conversation
        {
            public string username { get; set; }
            public string id { get; set; }
        }

        private class MessageResp
        {
            public string message { get; set; }
            public string sender { get; set; }
        }

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            
            if(txtMessage.Text != "" )
            {
                var reqCls = new MessageToSend
                {
                    conversation_id = conversations[UsersComboBox.SelectedItem.ToString()],
                    message = txtMessage.Text
                    
                };
                string jsonString = JsonSerializer.Serialize(reqCls);
                var content = new StringContent(jsonString, encoding: System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://134.122.51.174:8888/conversations/messages/send", content);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine(response.StatusCode);
                }

                
                
                
            }
        }
        private async void addUserButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogContent = new AddUserPage();
            ContentDialog addUserDialog = new ContentDialog();
            addUserDialog.XamlRoot = addUserButton.XamlRoot;
            addUserDialog.Title = "Add user";
            addUserDialog.CloseButtonText = "Close";
            addUserDialog.PrimaryButtonText = "Add";
            addUserDialog.Content = dialogContent;

            var result = await addUserDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var reqCls = new UserAddReq
                {
                    username = UserToAdd.username
                };

                string jsonString = JsonSerializer.Serialize(reqCls);
                var content = new StringContent(jsonString, encoding: System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://134.122.51.174:8888/conversations/create", content);
                var responseString = await response.Content.ReadAsStringAsync();

                Debug.WriteLine(responseString);
                GetConversations();
            }
        }


        async void GetConversations()
        {

            var response = await client.GetAsync("http://134.122.51.174:8888/conversations/get");
                var responseString = await response.Content.ReadAsStringAsync();

                var conversations_list = JsonSerializer.Deserialize<List<Conversation>>(responseString);
                if (conversations_list.Count != UsersComboBox.Items.Count())
                {
                    foreach (Conversation conversation in conversations_list)
                    {
                        if (!conversations.ContainsKey(conversation.username))
                        {
                            conversations[conversation.username] = conversation.id;
                            messages[conversation.username] = new List<string>();
                            UsersComboBox.Items.Add(conversation.username);
                        }
                    }
                }
                
        }
        async Task getMessagesTask()
        {
            while (true)
            {
                GetConversations();
                await Task.Delay(100);
                getMessages();
                await Task.Delay(2 * 1000);
            }
        }

        async void getMessages()
        {
            Debug.WriteLine("dupa");
            foreach (KeyValuePair<string,string> conversation in conversations)
            {
                
                var response = await client.GetAsync("http://134.122.51.174:8888/conversations/" + conversation.Value);
                var responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                var messagesL = JsonSerializer.Deserialize<List<MessageResp>>(responseString);
                try
                {
                    if(messages.ContainsKey(conversation.Key))
                    {
                        messages[conversation.Key].Clear();
                    }
                    foreach (MessageResp messageResp in messagesL)
                    {
                        Debug.WriteLine($"{messageResp.sender}: {messageResp.message}");
                        messages[conversation.Key].Add($"{messageResp.sender}: {messageResp.message}");
                        Debug.WriteLine(messageResp.message);
                        
                        

                    }
                    messagesList.ItemsSource = null;
                    messagesList.ItemsSource = messages[selectedUser];
                    

                } catch { continue; }
            }
            
        }

        private async void UsersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(messages.Count);
            if(messages.Count != 0)
            {
                string selected = UsersComboBox.SelectedValue.ToString();
                Debug.WriteLine(selected.ToString());
                messagesList.ItemsSource = messages[selected];
                selectedUser = UsersComboBox.SelectedValue.ToString();
                
            }
        }
        

       
    }
}
