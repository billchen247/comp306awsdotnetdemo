using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon;

namespace Examples4BucketOps
{
    public static class Helper
    {
        public readonly static IAmazonS3 s3Client;

        static Helper()
        {
            s3Client = GetS3Client();
        }

        private static IAmazonS3 GetS3Client()
        {
            //string awsAccessKey = "xxx-REPLACE-YOUR-OWN-AWS-ACCESS-KEY-xxx";
            //string awsSecretKey = "xxx-REPLACE-YOUR-OWN-AWS-SECRET-KEY-xxx";
            string awsAccessKey = ConfigurationManager.AppSettings["accessId"];
            string awsSecretKey = ConfigurationManager.AppSettings["secretKey"];
            return new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.CACentral1);
        }


        //private static BasicAWSCredentials GetBasicCredentials()
        //{
        //    var builder = new ConfigurationBuilder()
        //                            .SetBasePath(Directory.GetCurrentDirectory())
        //                            .AddJsonFile("AppSettings.json");

        //    var accessKeyID = builder.Build().GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
        //    var secretKey = builder.Build().GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;

        //    return new BasicAWSCredentials(accessKeyID, secretKey);

        //}

    }
}
