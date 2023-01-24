// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Net.Http;
using System.Text.Json;
using System.Text.Encodings;
using Windows.UI.Popups;
namespace App1
{
    public class UserLogin
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }
        private static readonly HttpClient client = new HttpClient();

        async void fields()
        {
            ContentDialog fieldDialog = new ContentDialog
            {
                Title = "Some fields are empty",
                Content = "Fill in all fields",
                CloseButtonText = "Ok"
            };
            fieldDialog.XamlRoot = loginPanel.XamlRoot;

            ContentDialogResult result = await fieldDialog.ShowAsync();
        }
        async void checkLogin()
        {
            try
            {
                var loginUser = new UserLogin
                {
                    username = usernameTextBox.Text.ToString(),
                    password = passwordBox.Password.ToString()
                };
                string jsonString = JsonSerializer.Serialize(loginUser);
                var content = new StringContent(jsonString, encoding:System.Text.Encoding.UTF8, "application/json");
             

                var response = await client.PostAsync("http://134.122.51.174:8888/user/login",content);
                
                var responseString = await response.Content.ReadAsStringAsync();

                if(response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    errorTextBlock.Text = responseString;
                }
                else
                {
                    Jwt.token = responseString;
                    this.Frame.Navigate(typeof(ChatPage));
                }
            }
            catch (Exception ex)
            {
                errorTextBlock.Text = ex.ToString();
            }
        }
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(usernameTextBox.Text))
            {
                fields();
            }
            else if (string.IsNullOrEmpty(passwordBox.Password))
            {
                fields();
            }
            else
            {
                checkLogin();
            }
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterPage));
        }
    }
}
