using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LiveStethoV2
{
    class FlaskCommunication
    {

        public async Task<Tuple<HttpStatusCode, SoundDataModel>> GetAllMetaData()
        {
            ApiClient cli = new ApiClient();
            Task<IRestResponse<SoundDataModel>> apidata = cli.GetAllMetaData();
            IRestResponse<SoundDataModel> data = await apidata;
            return Tuple.Create<HttpStatusCode, SoundDataModel>(data.StatusCode, data.Data);
        }

        public async Task<Tuple<HttpStatusCode, SoundDataModel.SoundData>> PostMetaData(string name, long filelength)
        {
            ApiClient cli = new ApiClient();
            Task<IRestResponse<SoundDataModel.SoundData>> apidata = cli.PostMetaData(name, (int)filelength);
            IRestResponse<SoundDataModel.SoundData> data = await apidata;
            return Tuple.Create<HttpStatusCode, SoundDataModel.SoundData>(data.StatusCode, data.Data); 
        }

        public async Task<HttpStatusCode> PostFile(string file, long len, int id)
        {
            ApiClient cli = new ApiClient();
            Task<IRestResponse> apidata = cli.PostSoundFile(file, len, id);
            IRestResponse data = await apidata;
            return data.StatusCode;
        }

        public async Task<Tuple<HttpStatusCode, SoundDataModel.SoundData>> GetMetaData(int id)
        {
            ApiClient cli = new ApiClient();
            Task<IRestResponse<SoundDataModel.SoundData>> apidata = cli.GetMetaData(id);
            IRestResponse<SoundDataModel.SoundData> data = await apidata;
            return Tuple.Create<HttpStatusCode, SoundDataModel.SoundData>(data.StatusCode, data.Data); //Return Get Meta Data Content With
        }

        
        public async Task<Tuple<HttpStatusCode, byte[]>> GetSoundFile(int id)
        {
            ApiClient cli = new ApiClient();
            Task<IRestResponse> apidata = cli.GetSoundFile(id);
            IRestResponse data = await apidata;
            return Tuple.Create<HttpStatusCode, byte[]>(data.StatusCode, data.RawBytes);
        }

        public async Task<Tuple<HttpStatusCode, SoundDataModel.AnalysisResult>> GetAnalysisResult(int id)
        {
            ApiClient cli = new ApiClient();
            Task<IRestResponse<SoundDataModel.AnalysisResult>> apidata = cli.GetAnalysisResult(id);
            IRestResponse<SoundDataModel.AnalysisResult> data = await apidata;
            return Tuple.Create<HttpStatusCode, SoundDataModel.AnalysisResult>(data.StatusCode, data.Data);
        }
    }
}
