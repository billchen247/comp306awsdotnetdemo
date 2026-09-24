using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoObjectPersistentModel
{
    [DynamoDBTable("Student")]
    public class Student
    {
        [DynamoDBHashKey("ID")] //Partition key
        public string Id
        {
            get; set;
        }
        [DynamoDBProperty]
        public string FirstName
        {
            get; set;
        }
       
        public string LastName
        {
            get; set;
        }

        [DynamoDBIgnore]
        public string ProgramName
        { get; set; }
    }
}
