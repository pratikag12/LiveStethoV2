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
using System.IO;

namespace LiveStethoV2
{
    class ApiClient
    {
        private IRestClient _client = null;

        public ApiClient()
        {
            this._client = new RestClient("http://127.0.0.1:5000/api/v1_0");
        }

        //Acquire all metadata
        public Task<IRestResponse<SoundDataModel>> GetAllMetaData()
        {
            var req = new RestRequest("/soundmetadatas", Method.GET);
            var resp = _client.ExecuteTaskAsync<SoundDataModel>(req);
            return resp;
        }

        //Send Sound MetaData
        public Task<IRestResponse<SoundDataModel.SoundData>> PostMetaData(string name, int filelength)
        {
            var req = new RestRequest("/soundmetadatas", Method.POST);
            req.AddJsonBody(
                new
                {
                    name = name,  //Name of Test
                    length = filelength  //Name of File
                }); // AddJsonBody serializes the object automatically

            //req.AddHeader("Content-type", "application/json");
            req.RequestFormat = DataFormat.Json;
            var resp = _client.ExecuteTaskAsync<SoundDataModel.SoundData>(req);
            return resp;
        }

        //Upload Sound Data File
        public Task<IRestResponse> PostSoundFile(string file, long len, int id)
        {
            var req = new RestRequest("/sounddata/{id}", Method.POST);
            req.AddUrlSegment("id", id);
            BinaryReader br = new BinaryReader(File.Open(file, FileMode.Open));
            byte[] documentBytes = br.ReadBytes((int)len);
            req.AddParameter("application/dat", documentBytes, ParameterType.RequestBody);
            var resp = _client.ExecuteTaskAsync(req);
            return resp;
        }

        //Retrieve sound metadata
        public Task<IRestResponse<SoundDataModel.SoundData>> GetMetaData(int id)
        {
            var req = new RestRequest("/soundmetadata/{id}", Method.GET);
            req.AddUrlSegment("id", id); //Add URL request data id
            var resp = _client.ExecuteTaskAsync<SoundDataModel.SoundData>(req);
            return resp;
        }

        //Retrieve sounddata

        //Retrieve analysis results

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
}
