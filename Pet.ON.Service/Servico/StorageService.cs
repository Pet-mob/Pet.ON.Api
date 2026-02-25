using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Pet.ON.Domain.Interfaces.Servico.v1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pet.ON.Service.Servico
{
    public class S3StorageService : IStorageService
    {
        private readonly IAmazonS3 _s3;
        private readonly string _bucket;

        public S3StorageService()
        {
            var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            var region = Environment.GetEnvironmentVariable("AWS_REGION");
            _bucket = Environment.GetEnvironmentVariable("AWS_BUCKET_NAME");

            _s3 = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
        }

        public async Task<string> UploadAsync(string key, Stream stream, string contentType)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucket,
                Key = key,
                InputStream = stream,
                ContentType = contentType
            };

            await _s3.PutObjectAsync(request);

            return $"https://{_bucket}.s3.amazonaws.com/{key}";
        }

        public async Task<List<string>> ListAsync(string prefix)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucket,
                Prefix = prefix
            };

            var response = await _s3.ListObjectsV2Async(request);

            return response.S3Objects
                           .Select(o => $"https://{_bucket}.s3.amazonaws.com/{o.Key}")
                           .ToList();
        }
    }
}
