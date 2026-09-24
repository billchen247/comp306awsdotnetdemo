using Amazon.S3.Transfer;

namespace Example4ObjectLevelOpsWithAppSetting
{
    internal class Program
    {
        private const string bucketName = "hchen247bucketcreatedprogrammatically";
        private const string filePath = @"../../../Helper.cs";
        private const string keyName = "uploadedbyapplication";
        private const string fileName = "uploadedbyapp";

        static async Task Main(string[] args)
        {
            await DownloadFile(bucketName, fileName);
        }


        private static async Task DownloadFile(string bucketName, string fileName)
        {
            //var pathAndFileName = $"C:\\temp\\{fileName}";
            var pathAndFileName = Path.Combine(Path.GetTempPath(), fileName);

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
    }
}