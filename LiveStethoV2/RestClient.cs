using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft;

namespace LiveStethoV2
{
    class RestClient
    {

        class SoundMetaData
        {
            public int id;
            public string name;
            public string file_uri;
            public int length;
            public string date; 
        }

        private int GetFileList()
        {
            return 0;
        }

        //Test Something
        try
        {	        
		
	    }
	    catch (RestSharp.DataFormat e)
	    {

		    
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
}
