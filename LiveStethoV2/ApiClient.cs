using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Threading.Tasks;

namespace LiveStethoV2
{

    class ApiClient
    {
        private IRestClient _client = null;

        public ApiClient()
        {
            this._client = new RestClient("http://127.0.0.1:5000/api/v1_0");
        }
        public IObservable<IRestResponse<SoundDataModel>> GetFileList()
        {
            var req = new RestRequest("/soundmetadatas", Method.GET);
            var resp = _client.ExecuteTaskAsync<SoundDataModel>(req).ToObservable();
            return resp;
        }
    }

    /*
    //Test Function
    private void RestCommGet()
    {
        //Call Rest Api
        RestClient TestBe = new RestClient("http://127.0.0.1:5000/");
        RestRequest req = new RestRequest("get/{name}", Method.GET);
        req.AddUrlSegment("name", "bear");
        req.AddFile("file", "file path");
        IRestResponse resp = TestBe.Execute(req);
        JsonData respobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonData>(resp.Content);
        MessageBox.Show(respobj.data);
    }

    private void RestCommGetFile()
    {
        //Call Rest Api
        RestClient TestBe = new RestClient("http://127.0.0.1:5000/");
        RestRequest req = new RestRequest("get/{name}", Method.GET);

        req.AddUrlSegment("name", "bear");
        IRestResponse resp = TestBe.Execute(req);
        Console.WriteLine(resp.ContentLength);
        tmpFile = File.OpenWrite(@"D:\Test Data S\CDLData\StethoStream.bin");
        tmpFile.Write(resp.RawBytes, 0, (int)resp.ContentLength);
        tmpFile.Close();
    }
    */
}
