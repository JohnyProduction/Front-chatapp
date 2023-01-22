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
    public class UserRegister 
    {
        public string username { get; set; }
        public string password1 { get; set; }
        public string password2 {get; set; }
    
    }

    public sealed partial class RegisterPage : Page
    {
        private static readonly HttpClient client = new HttpClient();
        public RegisterPage()
        {
            this.InitializeComponent();
        }

        async void checkRegister()
        {
            try
            {
                var registerUser = new UserRegister
                {
                    username = regusernameTextBox.Text.ToString(),
                    password1 = regpasswordBox.Password.ToString(),
                    password2 = reregpasswordBox.Password.ToString()
                };
                string jsonString = JsonSerializer.Serialize(registerUser);
                var content = new StringContent(jsonString, encoding: System.Text.Encoding.UTF8, "application/json");


                var response = await client.PostAsync("http://134.122.51.174:8888/user/register", content);

                var responseString = await response.Content.ReadAsStringAsync();
                this.Frame.Navigate(typeof(LoginPage));
            }
            catch(Exception ex)
            {
                errorTextBlock.Text = ex.ToString();
            }
        }
        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            checkRegister();
            
        }
    }
}
