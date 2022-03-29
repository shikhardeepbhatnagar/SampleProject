using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebAPI.Classes;
using WebAPI.Controllers;
using WebAPI.DataAccessLayer.CommonDataAccessLayer;
using WebAPI.Enumration;
using WebAPI.Models.TombstoneModals;

namespace WebAPI.Models.TemplateRepositoryExcel
{
    public class ChildCategoryJsonModel<T> : BaseDataModal
    {
        [JsonProperty("CategoryID")]
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryParentId { get; set; }
        public DateTime? UpdatedAtCategory { get; set; }
        public int? TemplateWorkbookId { get; set; }

        [JsonProperty("TemplateName")]
        public string TemplateName { get; set; }

        [JsonProperty("OrigFileName")]
        public string OrigFileName { get; set; }
        public string SystemFileName { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("FileSizeInKB")]
        public int? FileSizeInKB { get; set; }

        [JsonProperty("WorksheetCount")]
        public int? WorksheetCount { get; set; }

        [JsonProperty("IsPreviewAvailable")]
        public bool? IsPreviewAvailable { get; set; }
        public DateTime? UpdatedAtTemplate { get; set; }

        public T inputData { get; set; }
    }

    public class WorksheetInfoModel : IManageCrudOperations
    {
        public int? TemplateWorkbookId { get; set; }

        public string WorksheetName { get; set; }

        public string SystemWorksheetName { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public void Delete<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType)
        {
            //throw new NotImplementedException();
        }

        public void GetData(SqlDataReader data)
        {
            TemplateWorkbookId = Convert.IsDBNull(data["TemplateWorkbookId"]) ? default(int?) : Convert.ToInt32(data["TemplateWorkbookId"]);
            WorksheetName = data["WorksheetName"].ToString().Trim();
            SystemWorksheetName = data["SystemWorksheetName"].ToString().Trim();
            UpdatedAt = Convert.IsDBNull(data["UpdatedAt"]) ? default(DateTime?) : Convert.ToDateTime(data["UpdatedAt"]);

        }

        public void Insert<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType)
        {
            //throw new NotImplementedException();
        }

        public void Update<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType)
        {
            //throw new NotImplementedException();
        }
    }

    public class ChildCategoryModel : BaseDataModal, IManageCrudOperations
    {
        private string username;
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? CategoryParentId { get; set; }
        public DateTime? UpdatedAtCategory { get; set; }
        public int? TemplateWorkbookId { get; set; }
        public string TemplateName { get; set; }
        public string OrigFileName { get; set; }
        public string SystemFileName { get; set; }
        public string Description { get; set; }
        public int? FileSizeInKB { get; set; }
        public int? WorksheetCount { get; set; }
        public bool? IsPreviewAvailable { get; set; }
        public bool IsWorksheetInfoNeedToUpdate { get; set; }
        public DateTime? UpdatedAtTemplate { get; set; }

        public List<WorksheetInfo> modelDataUDT;
        public void Insert<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType, List<WorksheetInfo> modelDataUDT)
        {
            var result = modelType as ChildCategoryModel;
            this.modelDataUDT = modelDataUDT;

            this.username = userName;
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryId", result.CategoryID));
            _dbManager._command.Parameters.Add(new SqlParameter("@origFileName", result.OrigFileName));
            _dbManager._command.Parameters.Add(new SqlParameter("@systemFileName", result.SystemFileName));
            _dbManager._command.Parameters.Add(new SqlParameter("@description", result.Description));
            _dbManager._command.Parameters.Add(new SqlParameter("@templateName", result.TemplateName));
            _dbManager._command.Parameters.Add(new SqlParameter("@fileSizeInKB", result.FileSizeInKB));
            _dbManager._command.Parameters.Add(new SqlParameter("@worksheetCount", result.WorksheetCount));
            _dbManager._command.Parameters.Add(new SqlParameter("@isPreviewAvailable", result.IsPreviewAvailable));
            _dbManager._command.Parameters.Add(new SqlParameter("@insertedAt", DateTime.Now));
            _dbManager._command.Parameters.Add(new SqlParameter("@insertedBy", userName));
            _dbManager._command.Parameters.Add(new SqlParameter("@WorksheetInfo", FillWorksheetInfo));



        }

        public DataTable FillWorksheetInfo
        {
            get
            {
                if (modelDataUDT == null)
                    return null;
                else
                {
                    DataTable DT = new DataTable();
                    DT.Columns.Add("Id", typeof(int));
                    DT.Columns.Add("TemplateWorkbookId", typeof(int));
                    DT.Columns.Add("WorksheetName", typeof(string));
                    DT.Columns.Add("SystemWorksheetName", typeof(string));
                    DT.Columns.Add("InsertedAt", typeof(DateTime));
                    DT.Columns.Add("InsertedBy", typeof(string));
                    DT.Columns.Add("UpdatedAt", typeof(DateTime));
                    DT.Columns.Add("UpdatedBy", typeof(string));
                    int ctr = 1;
                    foreach (var item in modelDataUDT)
                    {
                        var count = ++ctr;
                        DataRow DR = DT.NewRow();


                        DR["Id"] = count;
                        DR["TemplateWorkbookId"] = count; // adding field value
                        DR["WorksheetName"] = item.WorksheetName;   // adding field value
                        DR["SystemWorksheetName"] = item.SystemWorksheetName; //adding field value
                        DR["InsertedAt"] = DateTime.Now;   // adding field value
                        DR["InsertedBy"] = username;   // adding field value
                        DR["UpdatedAt"] = DateTime.Now;   // adding field value
                        DR["UpdatedBy"] = username;   // adding field value

                        DT.Rows.Add(DR);
                    }
                    return DT;
                }

            }
        }

        public void Delete<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType)
        {
            var result = modelType as ChildCategoryModel;
            _dbManager._command.Parameters.Add(new SqlParameter("@templateWorkbookId", result.TemplateWorkbookId));
        }

        public void Update<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType, List<WorksheetInfo> modelDataUDT)
        {
            var result = modelType as ChildCategoryModel;
            this.modelDataUDT = modelDataUDT;

            this.username = userName;
            _dbManager._command.Parameters.Add(new SqlParameter("@categoryId", result.CategoryID));
            _dbManager._command.Parameters.Add(new SqlParameter("@origFileName", result.OrigFileName));
            _dbManager._command.Parameters.Add(new SqlParameter("@systemFileName", result.SystemFileName));
            _dbManager._command.Parameters.Add(new SqlParameter("@description", result.Description));
            _dbManager._command.Parameters.Add(new SqlParameter("@templateName", result.TemplateName));
            _dbManager._command.Parameters.Add(new SqlParameter("@fileSizeInKB", result.FileSizeInKB));
            _dbManager._command.Parameters.Add(new SqlParameter("@worksheetCount", result.WorksheetCount));
            _dbManager._command.Parameters.Add(new SqlParameter("@isPreviewAvailable", result.IsPreviewAvailable));
            _dbManager._command.Parameters.Add(new SqlParameter("@updatedAt", DateTime.Now));
            _dbManager._command.Parameters.Add(new SqlParameter("@updatedBy", userName));

            _dbManager._command.Parameters.Add(new SqlParameter("@isWorksheetInfoNeedToUpdate", result.IsWorksheetInfoNeedToUpdate));
            _dbManager._command.Parameters.Add(new SqlParameter("@templateWorkbookId", result.TemplateWorkbookId));
            _dbManager._command.Parameters.Add(new SqlParameter("@WorksheetInfo", FillWorksheetInfo));


        }

        public void GetData(SqlDataReader data)
        {
            CategoryID = Convert.IsDBNull(data["CategoryID"]) ? default(int?) : Convert.ToInt32(data["CategoryID"]);
            CategoryName = data["CategoryName"].ToString().Trim();
            CategoryParentId = Convert.IsDBNull(data["CategoryParentId"]) ? default(int?) : Convert.ToInt32(data["CategoryParentId"]);
            UpdatedAtCategory = Convert.IsDBNull(data["UpdatedAtCategory"]) ? default(DateTime?) : Convert.ToDateTime(data["UpdatedAtCategory"]);
            TemplateWorkbookId = Convert.IsDBNull(data["TemplateWorkbookId"]) ? default(int?) : Convert.ToInt32(data["TemplateWorkbookId"]);
            TemplateName = data["TemplateName"].ToString().Trim();
            OrigFileName = data["OrigFileName"].ToString().Trim();
            SystemFileName = data["SystemFileName"].ToString().Trim();
            Description = data["Description"].ToString().Trim();
            FileSizeInKB = Convert.IsDBNull(data["FileSizeInKB"]) ? default(int?) : Convert.ToInt32(data["FileSizeInKB"]);
            WorksheetCount = Convert.IsDBNull(data["WorksheetCount"]) ? default(int?) : Convert.ToInt32(data["WorksheetCount"]);
            IsPreviewAvailable = Convert.IsDBNull(data["IsPreviewAvailable"]) ? default(bool?) : Convert.ToBoolean(data["IsPreviewAvailable"]);
            UpdatedAtTemplate = Convert.IsDBNull(data["UpdatedAtTemplate"]) ? default(DateTime?) : Convert.ToDateTime(data["UpdatedAtTemplate"]);
        }

        public void Insert<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType)
        {
            //throw new NotImplementedException();
        }

        public void Update<T>(DatabaseManager _dbManager, T modelType, string userName, CRUDType operationType)
        {
            //throw new NotImplementedException();
        }
    }
}