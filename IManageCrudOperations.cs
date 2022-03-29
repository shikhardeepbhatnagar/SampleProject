using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Classes;
using WebAPI.Enumration;

namespace WebAPI.DataAccessLayer.CommonDataAccessLayer
{
    interface IManageCrudOperations
    {
        /// <summary>
        /// Method to insert required properties in the table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_dbManager"></param>
        /// <param name="modelType"></param>
        /// <param name="userName"></param>
        void Insert<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType);

        /// <summary>
        /// /// <summary>
        /// Method to delete required properties from the table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_dbManager"></param>
        /// <param name="modelType"></param>
        /// <param name="userName"></param>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_dbManager"></param>
        /// <param name="modelType"></param>
        /// <param name="userName"></param>
        void Delete<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType);

        /// <summary>
        /// Method to update required properties in the database table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_dbManager"></param>
        /// <param name="modelType"></param>
        /// <param name="userName"></param>
        void Update<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType);

        /// <summary>
        /// Method to get data from the table 
        /// </summary>
        /// <param name="data"></param>
        void GetData(SqlDataReader data);
    }
}
