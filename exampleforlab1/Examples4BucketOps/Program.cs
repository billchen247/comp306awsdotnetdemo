



namespace Examples4BucketOps
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("start s3 demo operations");

            BucketOps op = new BucketOps();
            await op.GetBucketList();


            Console.WriteLine("create bucket");
            await op.CreateBucket("hchen247bucketcreatedprogrammatically");
            await op.GetBucketList();
            
            //Console.WriteLine("delete a bucket");
            // await Helper.s3Client.DeleteBucketAsync("hchen247bucketcreatedprogrammatically");

            Console.WriteLine("continue test s3 ops");

            await op.GetBucketList();

            Console.WriteLine("All s3 test have been finished sucessfully!");
            Console.ReadKey();
        }
    }
}
