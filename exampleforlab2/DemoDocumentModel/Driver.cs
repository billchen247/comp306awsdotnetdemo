using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDocumentModel
{
    class Driver
    {
        static async Task Main(string[] args)
        {
            DDBOperation op = new DDBOperation("User");
            await op.InsertAsync();
            await op.LoadAllItemAsync();

           await op.LoadItemAsync(1003, "yli@gmail.com");



            //await op.DeleteItemAsyc("yli202@my.centennialcollege.ca");


            Console.WriteLine("Operation has been successfully done!");
            Console.ReadKey();
        }
    }
}
