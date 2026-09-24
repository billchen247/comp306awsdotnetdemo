using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccessDynamoDB;
using DemoDocumentModel;

namespace DemoObjectPersistentModel
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("start dynamodb demo operations");
            // lower model 
            AccessDynamoDB.DDBOperations operation = new AccessDynamoDB.DDBOperations();
            await operation.GetAllTables();
            bool existed = await operation.TableExisted();
            if (existed)
            {
                Console.WriteLine("The Dynamodb table has been existed.");
            }
            else
            {
                await operation.CreateTable();
            }
            
            await operation.DescribeTable();
            await operation.GetAllTables();
            await operation.GetItem();
            //await operation.DeleteTable();
            Console.WriteLine("presee any key to continue");
            Console.ReadKey();

            // document model
            DemoDocumentModel.DDBOperation docops = new DemoDocumentModel.DDBOperation("User");
            await docops.InsertAsync();
            await docops.LoadItemAsync(1003, "yli@gmail.com");

            // orm model similar like entity framework with RDS
            Console.WriteLine("Demo object persistent model ops to dynamodb");

            DDBOperation ops = new DDBOperation();
            Book book1 = new Book();
            book1.ISBN = "XXX-FFFF-YYY-001";
            book1.Title = "cloud computing demo";
            await ops.SaveBookAsync(book1);

            await ops.CRUDOperations();
            await ops.LoadSpecificBookAsync("999-000001");

            Console.WriteLine("All operations have been finished sucessfully!");
            Console.ReadKey();

        }
    }
}
