using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TN.TNM.DataAccess.Models.TinhHuong;

namespace TN.TNM.DataAccess.Helper
{
    public static class SendRequestHelper
    {
        public static ResponseKichHoatModel KichHoatTinhHuong(KichHoatRequestModel item)
        {
            var response = new ResponseKichHoatModel();
            var uri = "https://autocall.telebot.vn/autocall/api/fastTts";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization",
                    "Rbt7qYcZkM5sBz43brDZGg86rD5t8BJoLVOf1MUHChxE2JaiAXeC9i0UdFv4OzeQFWpkfjL3ITXHhjV0Kg7a9mycNolNquSY1mWx");

                var json = JsonConvert.SerializeObject(item).ToString();
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var _response = client.PostAsync(uri, content).Result;
                    var contents = _response.Content.ReadAsStringAsync().Result;
                    dynamic result = JsonConvert.DeserializeObject(contents);
                    response.error = result.error;
                    response.message = new MessageKichHoatModel {session = result.message.session};
                }
                catch (Exception e)
                {
                    
                }
            }

            return response;
        }

        public static string GetKichHoatTinhHuongResult(string session)
        {
            string result = null;
            var uri = "https://autocall.telebot.vn/autocall/api/result" + "?session=" + session;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization",
                    "Rbt7qYcZkM5sBz43brDZGg86rD5t8BJoLVOf1MUHChxE2JaiAXeC9i0UdFv4OzeQFWpkfjL3ITXHhjV0Kg7a9mycNolNquSY1mWx");

                try
                {
                    var _response = client.GetAsync(uri).Result;
                    var contents = _response.Content.ReadAsStringAsync().Result;
                    dynamic response = JsonConvert.DeserializeObject(contents);

                    if (response.error == 1)
                    {
                        result = response.message;
                    }
                    else
                    {
                        result = response.data[0].digit;
                    }
                }
                catch (Exception e)
                {

                }
            }

            return result;
        }
    }
}
