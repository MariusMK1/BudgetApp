using BudgetApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BudgetApp.Helpers;

public class DatabaseHelper
{
    private static string _dbPath = "https://budgetapp-72993-default-rtdb.europe-west1.firebasedatabase.app/";
    public static async Task<bool> Insert<T>(T item) where T : IHasId
    {
        var jsonBody = JsonConvert.SerializeObject(item);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        using (var client = new HttpClient())
        {
            var result = await client.PostAsync($"{_dbPath}{item.GetType().Name.ToLower()}.json", content);
            if (!result.IsSuccessStatusCode)
                return false;
            var responseContent = await result.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
            if (data is not null)
            {
                if (data.TryGetValue("name", out var id))
                    item.Id = id;
            }
            return true;
        }
    }
    public static async Task<List<T>> Read<T>() where T : IHasId
    {
        using (var client = new HttpClient())
        {
            var result = await client.GetAsync($"{_dbPath}{typeof(T).Name.ToLower()}.json");

            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
                if (items is null)
                    return new();
                var list = items.Select(item =>
                {
                    item.Value.Id = item.Key;
                    return item.Value;
                }).ToList();
                return list;
            }
            else
            {
                return new();
            }
        }
    }
    public static async Task<bool> Update<T>(T item) where T : IHasId
    {
        var jsonBody = JsonConvert.SerializeObject(item);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        using (var client = new HttpClient())
        {
            var result = await client.PatchAsync($"{_dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json", content);
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public static async Task<bool> Delete<T>(T item) where T : IHasId
    {
        using (var client = new HttpClient())
        {
            var result = await client.DeleteAsync($"{_dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json");
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
