using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDocumentModel
{
    public class DDBOperation
    {
        AmazonDynamoDBClient client;
        // Amazon.DynamoDBv2.DataModel.DynamoDBContext context;
        Amazon.Runtime.BasicAWSCredentials credentials;
        Table userTable;

        public DDBOperation(string tableName)
        {
            credentials = new Amazon.Runtime.BasicAWSCredentials(ConfigurationManager.AppSettings["accessId"], ConfigurationManager.AppSettings["secretKey"]);
            client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.CACentral1);
            userTable = Table.LoadTable(client, tableName, DynamoDBEntryConversion.V2); //load the metadata of the table
        }

        public async Task InsertAsync()
        {
            Document newUser = new Document();
            newUser["Id"] = 1003;
            newUser["UserName"] = "yli@gmail.com";
            newUser["Address"] = "941 Progress Ave, Scarborough, Ontario, Canada";
            newUser["Active"] = true;
            newUser["Interests"] = new List<String> { "Yoga", "Running", "Golfing", "Playing Piano" };

            Document skills = new Document();
            skills["C#"] = 10;
            skills["Java"] = 12;
            skills["Python"] = 4;
            newUser["Skills"] = skills;

            await userTable.PutItemAsync(newUser);
        }

        public async Task LoadAllItemAsync()
        {
            try
            {
                // Create an empty scan filter (scans all items)
                ScanFilter filter = new ScanFilter();

                // Start scan
                Search search = userTable.Scan(filter);

                // Get the first page of results
                List<Document> documents = await search.GetNextSetAsync();

                // Output results
                foreach (var doc in documents)
                {
                    Console.WriteLine(doc.ToJsonPretty());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"got exception {e}");
            }
        }

        public async Task LoadAllItemAsyncBigData()
        {
            try
            {
                ScanFilter filter = new ScanFilter();
                Search search = userTable.Scan(filter);

                do
                {
                    List<Document> documents = await search.GetNextSetAsync();

                    foreach (var doc in documents)
                    {
                        Console.WriteLine(doc.ToJsonPretty());
                    }
                } while (!search.IsDone);
            }
            catch (Exception e)
            {
                Console.WriteLine($"got exception {e}");
            }
        }

        public async Task LoadItemAsync(int key, string sorted_key = "")
        {
            try
            {
                Console.WriteLine(">>> LoadItemAsync");

                QueryFilter filter = new QueryFilter();
                filter.AddCondition("Id", QueryOperator.Equal, key);

                if (!string.IsNullOrWhiteSpace(sorted_key))
                {
                    filter.AddCondition("UserName", QueryOperator.Equal, sorted_key);
                }

                Search search = userTable.Query(filter);
                do
                {
                    List<Document> documents = await search.GetNextSetAsync();

                    foreach (var document in documents)
                    {
                        Console.WriteLine(document.ToJsonPretty());
                    }
                } while (!search.IsDone);
            }
            catch (Exception e)
            {
                Console.WriteLine($"got exception {e}");
            }

        }

        public async Task DeleteItemAsyc(int Key)
        {
            await userTable.DeleteItemAsync(Key);
        }

        public async Task DeleteItemAsyc(Document doc)
        {
            await userTable.DeleteItemAsync(doc);
        }
    }
}
