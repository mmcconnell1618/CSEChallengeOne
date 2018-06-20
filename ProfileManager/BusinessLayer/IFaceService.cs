using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace ProfileManager.BusinessLayer
{
    public interface IFaceService
    {
        Task<Face[]> FindFaces(string imageUrl);
    }
}
