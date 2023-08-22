using Avalonia.Controls;
using BudgetApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BudgetApp.Helpers;

public class FirebaseAuthHelper
{
    private static string api_key = "AIzaSyAJXYZqczAWJxZzyCqYXnaQtPDbnK - AXhk";
    public static async Task<LoginStatus> Register(User user)
    {
        using (HttpClient client = new HttpClient())
        {
            var body = new
            {
                email = user.Email,
                password = user.Password,
                returnSecureToken = true
            };
            var json = JsonConvert.SerializeObject(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={api_key}", data);
            if (response.IsSuccessStatusCode)
            {
                string resultJson =  await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<FirebaseResult>(resultJson);
                App.UserId = result.localId;
                var loginStatus = new LoginStatus
                {
                    Success = true,
                    Message = "Registration Successful"
                };
                return loginStatus;
            }
            else
            {
                string errorJson = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<FirebaseError>(errorJson);
                var loginStatus = new LoginStatus
                {
                    Success = false,
                    Message = error.error.message
                };
                return loginStatus;
            }
        }
    }
    public static async Task<LoginStatus> Login(User user)
    {
        using (HttpClient client = new HttpClient())
        {
            var body = new
            {
                email = user.Email,
                password = user.Password,
                returnSecureToken = true
            };
            var json = JsonConvert.SerializeObject(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={api_key}", data);
            if (response.IsSuccessStatusCode)
            {
                string resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<FirebaseResult>(resultJson);
                App.UserId = result.localId;
                var loginStatus = new LoginStatus
                {
                    Success = true,
                    Message = "Login Successful"
                };
                return loginStatus;
            }
            else
            {
                string errorJson = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<FirebaseError>(errorJson);
                var loginStatus = new LoginStatus
                {
                    Success = false,
                    Message = error.error.message
                };
                return loginStatus;
            }
        }
    }
}
