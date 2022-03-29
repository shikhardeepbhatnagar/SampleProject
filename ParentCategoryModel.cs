using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebAPI.Classes;
using WebAPI.DataAccessLayer.CommonDataAccessLayer;
using WebAPI.Enumration;
using WebAPI.Models.TombstoneModals;

namespace WebAPI.Models.TemplateRepositoryExcel
{
    public class ParentCategoryModel : BaseDataModal, IManageCrudOperations
    {
        [JsonProperty("CategoryID")]
        public int? Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryIconName { get; set; }
        public int? CategoryParentId { get; set; }
        public bool Status { get; set; }
        public string StatementType { get; set; }

        public void Insert<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType)
        {
            var modelData = modelType as ParentCategoryModel;
            //_dbManager._command.Parameters.Add(new SqlParameter("@categoryId", result.Id));
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryName", modelData.CategoryName));
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryIconName", modelData.CategoryIconName));
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryParentId", modelData.CategoryParentId));
            _dbManager._command.Parameters.Add(new SqlParameter("@status", modelData.Status));
            _dbManager._command.Parameters.Add(new SqlParameter("@insertedAt", DateTime.Now));
            _dbManager._command.Parameters.Add(new SqlParameter("@insertedBy", userName));
            _dbManager._command.Parameters.Add(new SqlParameter("@updatedAt", DateTime.Now));
            _dbManager._command.Parameters.Add(new SqlParameter("@updatedBy", userName));
            _dbManager._command.Parameters.Add(new SqlParameter("@lastUpdatedAt", DateTime.Now));
            _dbManager._command.Parameters.Add(new SqlParameter("@StatementType", operationType.ToString()));
        }

        public void Delete<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType)
        {
            var modelData = modelType as ParentCategoryModel;
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryId", modelData.Id));
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryName", modelData.CategoryName));
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryParentId", modelData.CategoryParentId));
            _dbManager._command.Parameters.Add(new SqlParameter("@lastUpdatedAt", DateTime.Now));
            _dbManager._command.Parameters.Add(new SqlParameter("@StatementType", operationType.ToString()));
        }

        public void Update<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType)
        {
            var modelData = modelType as ParentCategoryModel;
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryId", modelData.Id));
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryIconName", modelData.CategoryIconName));
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryName", modelData.CategoryName));
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryParentId", modelData.CategoryParentId));
            _dbManager._command.Parameters.Add(new SqlParameter("@status", modelData.Status));
            _dbManager._command.Parameters.Add(new SqlParameter("@insertedAt", DateTime.Now));
            _dbManager._command.Parameters.Add(new SqlParameter("@insertedBy", userName));
            _dbManager._command.Parameters.Add(new SqlParameter("@updatedAt", DateTime.Now));
            _dbManager._command.Parameters.Add(new SqlParameter("@updatedBy", userName));
            _dbManager._command.Parameters.Add(new SqlParameter("@lastUpdatedAt", DateTime.Now));
            _dbManager._command.Parameters.Add(new SqlParameter("@StatementType", operationType.ToString()));
        }

        public void GetData(SqlDataReader data)
        {
            Id = Convert.ToInt32(data["Id"]);
            CategoryName = data["CategoryName"].ToString().Trim();
            CategoryIconName = Convert.IsDBNull(data["CategoryIconName"]) ? default(string) : data["CategoryIconName"].ToString().Trim();
            CategoryParentId = Convert.IsDBNull(data["CategoryParentId"]) ? default(int?) : Convert.ToInt32(data["CategoryParentId"]);
            Status = Convert.ToBoolean(data["Status"]);
        }
    }


}