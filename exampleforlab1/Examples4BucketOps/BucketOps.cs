using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Examples4BucketOps
{
    public class BucketOps
    {
        //private static IAmazonS3 s3Client;

        //static async Task Main(string[] args)
        //{
        //    s3Client = new AmazonS3Client(GetBasicCredentials(), RegionEndpoint.USEast1);
        //    await CreateBucket(bucketName);

        //    // s3Client.DeleteBucketAsync(bucketName).Wait();
        //}


        public async Task<CreateBucketResponse> CreateBucket(string bucketName)
        {
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true
            };

            var response = await Helper.s3Client.PutBucketAsync(putBucketRequest);

            return new CreateBucketResponse
            {
                BucketName = bucketName,
                RequestId = response.ResponseMetadata.RequestId
            };
        }

        public async Task GetBucketList()
        {
            ListBucketsResponse response = await Helper.s3Client.ListBucketsAsync();
            foreach (S3Bucket bucket in response.Buckets)
            {
                Console.WriteLine("Bucket Name: "+bucket.BucketName + ", Created at: " + bucket.CreationDate.ToShortDateString());
            }
        }
    }
}


