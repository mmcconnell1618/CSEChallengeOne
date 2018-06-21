using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileManager.BusinessLayer
{
    public class WritePhotoResponse
    {
        public bool Success { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
    }
}
