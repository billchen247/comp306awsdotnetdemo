using Amazon.S3.Transfer;
using Amazon.S3;
using System;
using System.IO;

namespace Example4ObjectLevelOps
{
    internal class Program
    {
        private const string bucketName = "hchen247bucketcreatedprogrammatically";
        private const string filePath = "../../../Helper.cs";
        private const string keyName = "uploadedbyapplication";
        private const string fileName = "uploadedbyapp";


        static async Task Main(string[] args)
        {
            Console.WriteLine("start bucket object ops: upload and download file to s3");
            await UploadFileAsync();
            Console.WriteLine("just done upload to s3, will try download");
            Console.ReadLine();
            await DownloadFile(bucketName,fileName);
            Console.WriteLine("just done download file from s3. press any key to exit");
            Console.ReadLine();
        }


        private static async Task DownloadFile(string bucketName, string fileName)
        {
            var pathAndFileName = Path.Combine(Path.GetTempPath(), fileName);
            Console.WriteLine($"will try to download s3  bucket: {bucketName}, key: {keyName} to the local file {pathAndFileName}");

            var downloadRequest = new TransferUtilityDownloadRequest
            {
                BucketName = bucketName,
                Key = keyName,
                FilePath = pathAndFileName
            };

            using (var transferUtility = new TransferUtility(Helper.s3Client))
            {
                await transferUtility.DownloadAsync(downloadRequest);
            }
        }


        private static async Task UploadFileAsync()
        {
            try
            {
                var fileTransferUtility = new TransferUtility(Helper.s3Client);

                // Option 1. Upload a file. The file name is used as the object key name.
                //await fileTransferUtility.UploadAsync(filePath, bucketName);
                //Console.WriteLine("Upload 1 completed");

                // Option 2. Specify object key name explicitly.
                string full_path = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                Console.WriteLine($"will upload {full_path} to aws s3 bucket: {bucketName}, key: {keyName}");

                await fileTransferUtility.UploadAsync(full_path, bucketName, keyName);
                Console.WriteLine("Upload 2 completed");

                //// Option 3. Upload data from a type of System.IO.Stream.
                //using (var fileToUpload = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                //{
                //    await fileTransferUtility.UploadAsync(fileToUpload, bucketName, keyName);
                //}
                //Console.WriteLine("Upload 3 completed");

                //// Option 4. Specify advanced settings.
                //var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                //{
                //    BucketName = bucketName,
                //    FilePath = filePath,
                //    StorageClass = S3StorageClass.StandardInfrequentAccess,
                //    PartSize = 6291456, // 6 MB.
                //    Key = keyName,
                //    CannedACL = S3CannedACL.PublicRead
                //};
                //fileTransferUtilityRequest.Metadata.Add("date", "2020Fall");
                //fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

                //await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                //Console.WriteLine("Upload 4 completed");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }
    }
}