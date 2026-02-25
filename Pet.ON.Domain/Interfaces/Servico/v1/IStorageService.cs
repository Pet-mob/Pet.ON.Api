using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pet.ON.Domain.Interfaces.Servico.v1
{
    public interface IStorageService
    {
        Task<string> UploadAsync(string key, Stream stream, string contentType);
        Task<List<string>> ListAsync(string prefix);
    }
}
