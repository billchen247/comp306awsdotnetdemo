using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessDynamoDB
{
   public class Driver
    {
        static async Task Main(string[] args)
        {
            DDBOperations op = new DDBOperations();
            await op.GetAllTables();
            Console.WriteLine("****already print all tables *** ");

            //await op.CreateTable();

             await op.InsertItem();
             await op.GetItem();
            //await op.DeleteItem();
            await op.DescribeTable();
            //await op.DeleteTable();
            Console.ReadKey();
        }

    }
}
