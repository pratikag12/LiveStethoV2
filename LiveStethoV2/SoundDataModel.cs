using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveStethoV2
{
    public class SoundDataModel
    {
        public class SoundData
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public string FileUri { get; set; }
            public int Length { get; set; }
            public DateTime Date { get; set; }
        }

        public List<SoundData> SoundDatas { get; set; }
    }
}
