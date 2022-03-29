using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPI.Classes;
using WebAPI.DataAccessLayer;
using WebAPI.DataAccessLayer.CommonDataAccessLayer;
using WebAPI.Enumration;
using WebAPI.Models;
using WebAPI.Models.TemplateRepositoryExcel;
using static WebAPI.Controllers.GetFileController;

namespace WebAPI.Controllers
{
    public class TRExcelController : ApiController
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        string getAllParentCategoriesProcedureName = "GetAllParentCategoriesTR_EXL";
        string getAllChildCategoriesProcedureName = "GetAllChildCategoriesTR_EXL";

        [Authorize]
        [Route("pitchready/TemplateRepositoryExcel/GetAllParentCatagories")]
        [HttpGet]
        public IHttpActionResult GetAllParentCatagories()
        {
            try
            {
                return Ok(CommonDAL.GetAllData(getAllParentCategoriesProcedureName, typeof(ParentCategoryModel)));
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/TemplateRepositoryExcel/GetAllChildCatagories")]
        [HttpPost]
        public IHttpActionResult GetAllChildCatagories([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                var categoryID = apidata["CategoryID"];
                if (!string.IsNullOrEmpty(categoryID))
                {
                    //var retStr = JsonConvert.SerializeObject(TemplateRepositoryDAL.GetAllChildCatagories(Convert.ToInt32(categoryID)));
                    return Ok(CommonDAL.GetAllData(getAllChildCategoriesProcedureName, typeof(ChildCategoryModel), Convert.ToInt32(categoryID)));
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/TemplateRepositoryExcel/GetWorksheetInfo")]
        [HttpPost]
        public IHttpActionResult GetWorksheetInfo([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                var procedureName = "GetAllWorksheetInfoTR_EXL";
                var templateWorkbookId = apidata["TemplateWorkbookId"];
                if (!string.IsNullOrEmpty(templateWorkbookId))
                {
                    //var retStr = JsonConvert.SerializeObject(TemplateRepositoryDAL.GetAllChildCatagories(Convert.ToInt32(categoryID)));
                    return Ok(CommonDAL.GetAllDataById(procedureName, typeof(WorksheetInfoModel), "@templateWorkbookId", Convert.ToInt32(templateWorkbookId)));
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/TemplateRepositoryExcel/DMLCategoryDetailsTR_EXL")]
        [HttpPost]
        public async Task<IHttpActionResult> DMLCategoryDetailsTR_EXL()
        {
            try
            {
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());
                NameValueCollection formData = provider.FormData;
                if (!Directory.Exists(ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"]))
                {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"]);
                }
                string tempDocUrl = ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"];
                IList<HttpContent> FileStreamData = provider.Files;
                var OperationType = (CRUDType)Convert.ToInt32(formData["OperationType"]);
                var Data = formData["OperationData"];
                var UserName = formData["UserName"];
                string procedureName = "DMLCategoryDetailsTR_EXL";
                ParentCategoryModel catData = Newtonsoft.Json.JsonConvert.DeserializeObject<ParentCategoryModel>(Data);
                string filePath = string.Empty;
                if (FileStreamData != null || FileStreamData.Count() == 0)
                {
                    foreach (var item in FileStreamData)
                    {
                        var fileName = Guid.NewGuid();
                        var thisFileName = item.Headers.ContentDisposition.FileName.Trim('\"');
                        Stream File1Stream = await item.ReadAsStreamAsync();
                        using (var input = File.Create(tempDocUrl + fileName + Path.GetExtension(thisFileName)))
                        {
                            File1Stream.CopyTo(input);
                            catData.CategoryIconName = fileName.ToString() + ".png"/*+ Path.GetExtension(thisFileName)*/;
                        }
                    }
                }

                var catResult = CommonDAL.DatabaseCrudOperations(OperationType, catData, UserName, procedureName);
                if (catResult == null)
                {
                    return NotFound();
                }
                return Ok(catResult);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in TR Admin" + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/TemplateRepositoryExcel/InsertTemplatesWorkbookInfo_EXL")]
        [HttpPost]
        public async Task<IHttpActionResult> InsertTemplatesWorkbookInfo_EXL()
        {
            try
            {
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());
                NameValueCollection formData = provider.FormData;
                if (!Directory.Exists(ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"]))
                {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"]);
                }
                string tempDocUrl = ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"];
                IList<HttpContent> FileStreamData = provider.Files;
                var OperationType = (CRUDType)Convert.ToInt32(formData["OperationType"]);
                var Data = formData["OperationData"];
                var WorksheetNameList = formData["WorksheetNameList"];
                var UserName = formData["UserName"];
                string procedureName = "InsertTemplatesWorkbookInfo_EXL";
                ChildCategoryModel catData = Newtonsoft.Json.JsonConvert.DeserializeObject<ChildCategoryModel>(Data);
                List<LocalWorksheetInfo> worksheetNameList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LocalWorksheetInfo>>(WorksheetNameList);
                string filePath = string.Empty;
                List<WorksheetInfo> worksheetInfos = new List<WorksheetInfo>();

                if (FileStreamData != null || FileStreamData.Count() == 0)
                {
                    var sysfileName = string.Empty;
                    int j = 0;
                    for (int i = 0; i < FileStreamData.Count - 1 || i < worksheetNameList.Count; i++)
                    {
                        if (FileStreamData.Count - 1 <= worksheetNameList.Count)
                        {
                            if (worksheetNameList[i].HasPreview)
                            {
                                var fileName = Guid.NewGuid();
                                var thisFileName = FileStreamData[j].Headers.ContentDisposition.FileName.Trim('\"');
                                Stream File1Stream = await FileStreamData[j].ReadAsStreamAsync();

                                if (!string.IsNullOrEmpty(thisFileName))
                                {
                                    using (var input = File.Create(tempDocUrl + fileName + Path.GetExtension(thisFileName)))
                                    {
                                        File1Stream.CopyTo(input);
                                        sysfileName = fileName.ToString() + Path.GetExtension(thisFileName);
                                    }

                                    worksheetInfos.Add(new WorksheetInfo
                                    {
                                        WorksheetName = worksheetNameList[i].WorksheetName.ToString(),
                                        SystemWorksheetName = sysfileName,
                                        InsertedAt = DateTime.Now,
                                        InsertedBy = UserName,
                                        UpdatedAt = DateTime.Now,
                                        UpdatedBy = UserName
                                    });
                                }
                                j++;
                            }

                            else
                            {
                                worksheetInfos.Add(new WorksheetInfo
                                {
                                    WorksheetName = worksheetNameList[i].WorksheetName.ToString(),
                                    SystemWorksheetName = null,
                                    InsertedAt = DateTime.Now,
                                    InsertedBy = UserName,
                                    UpdatedAt = DateTime.Now,
                                    UpdatedBy = UserName
                                });
                            }
                        }
                    }
                }

                if (FileStreamData != null || FileStreamData.Count() == 0)
                {
                    var item = FileStreamData.LastOrDefault();
                    var fileName = Guid.NewGuid();
                    var thisFileName = item.Headers.ContentDisposition.FileName.Trim('\"');
                    Stream File1Stream = await item.ReadAsStreamAsync();
                    using (var input = File.Create(tempDocUrl + fileName + Path.GetExtension(thisFileName)))
                    {
                        File1Stream.CopyTo(input);
                        catData.SystemFileName = fileName.ToString() + Path.GetExtension(thisFileName);
                    }
                }


                var catResult = DatabaseCrudOperationsWithUDT(OperationType, catData, UserName, procedureName, worksheetInfos);
                if (catResult == null)
                {
                    return NotFound();
                }
                return Ok(catResult);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in TR Admin" + ex);
                return NotFound();
            }
        }

        [HttpGet]
        [Route("pitchready/TemplateRepositoryExcel/getfile/{fileName}")]
        [Authorize]
        public HttpResponseMessage getFile(string fileName)
        {
            HttpResponseMessage result = null;

            if (!Directory.Exists(ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"]))
            {
                Directory.CreateDirectory(ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"]);
            }

            string basePath = ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"].ToString();
            var localFilePath = Path.Combine(basePath, fileName);
            UNCAccess unc = new UNCAccess();

            if (!File.Exists(localFilePath))
            {
                result = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = fileName;
            }
            return result;
        }

        /// <summary>
        /// Method that manages crud operations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operationType">stores operation type</param>
        /// <param name="modelData">stores model data of type T</param>
        /// <param name="userName">stores user name</param>
        /// <param name="procedureName">stores procedure name</param>
        /// <returns></returns>
        private ChildCategoryModel DatabaseCrudOperationsWithUDT(CRUDType operationType, ChildCategoryModel modelData, string userName, string procedureName, List<WorksheetInfo> modelDataUDT)
        {
            dynamic result = modelData;
            DatabaseManager _dbManager = null;
            // Create instance of 'T'
            var obj = new ChildCategoryModel();

            int res = 0;

            try
            {
                _dbManager = new DatabaseManager(DALConstants.CONNECTION_KEY);
                _dbManager.prepareProcedure(procedureName);

                switch (operationType)
                {
                    case CRUDType.Insert:
                        obj.Insert(_dbManager, modelData, userName, operationType, modelDataUDT);
                        break;

                    case CRUDType.Delete:
                        obj.Delete(_dbManager, modelData, userName, operationType);
                        break;

                    case CRUDType.Update:
                    case CRUDType.Submit:
                        obj.Update(_dbManager, modelData, userName, operationType, modelDataUDT);
                        break;
                }

                res = _dbManager._command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (operationType == CRUDType.Delete)
                {
                    logger.Info(string.Format("Record has been deleted by username : {0} at time: {1} record name: {2}", userName, DateTime.Now, modelData));
                    throw ex;
                }
                else
                {
                    logger.Error(ex);
                    result.Message = ErrorMessage.SQLServerDown;
                    throw ex;
                }
            }
            finally
            {
                result.SatusCode = res;
                switch (res)
                {
                    case 0:
                        result.Message = ErrorMessage.RefreshData;
                        break;
                    case -1:
                        result.Message = ErrorMessage.Duplicate;
                        break;
                    case 1:
                        result.Message = ErrorMessage.Successfully;
                        break;
                    default:
                        result.Message = ErrorMessage.RefreshData;
                        break;
                }
                _dbManager.Close();
                _dbManager.Dispose();
            }
            return result;
        }



        [Authorize]
        [Route("pitchready/TemplateRepositoryExcel/DMLOperationWithoutFiles")]
        [HttpPost]
        public IHttpActionResult DMLOperationWithoutFiles([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                string procedureName = "DMLCategoryDetailsTR_EXL";

                var provider = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                ParentCategoryModel catData = Newtonsoft.Json.JsonConvert.DeserializeObject<ParentCategoryModel>(provider);
                //ServerResponseModel<ParentCategoryModel> returnValue = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerResponseModel<ParentCategoryModel>>(provider);

                var result = CommonDAL.DatabaseCrudOperations(OperationType, catData, UserName, procedureName);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while downloading the files and compressing them in a zip file " + ex);
                return NotFound();
            }
        }


        [Authorize]
        [Route("pitchready/TemplateRepositoryExcel/DeleteArtefact")]
        [HttpPost]
        public IHttpActionResult DeleteArtefact([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                var provider = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<string> fileNames = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(provider);
                string basePath = ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"].ToString();
                foreach (var item in fileNames)
                {
                    tryToDelete(basePath + item);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while downloading the files and compressing them in a zip file " + ex);
                return NotFound();
            }
        }

        private void tryToDelete(string v)
        {
            try
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                if (File.Exists(v))
                    File.Delete(v);
            }
            catch (Exception ex)
            {

            }
        }


        [Authorize]
        [Route("pitchready/TemplateRepositoryExcel/DeletTemplateWorkbookInfo")]
        [HttpPost]
        public IHttpActionResult DeletTemplateWorkbookInfo([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                string procedureName = "DeleteTemplatesWorkbookInfo_EXL";

                var provider = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                ChildCategoryModel catData = Newtonsoft.Json.JsonConvert.DeserializeObject<ChildCategoryModel>(provider);
                //ServerResponseModel<ParentCategoryModel> returnValue = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerResponseModel<ParentCategoryModel>>(provider);

                var result = CommonDAL.DatabaseCrudOperations(OperationType, catData, UserName, procedureName);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while downloading the files and compressing them in a zip file " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/TemplateRepositoryExcel/getCompressedfiles")]
        [HttpPost]
        public HttpResponseMessage getCompressedfiles()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                List<string> returnValue = new List<string>();
                if (!Directory.Exists(ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"]))
                {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"]);
                }
                var basePath = ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"].ToString();
                var FilesJson = Request.Content.ReadAsStringAsync().Result;
                var fileNames = JsonConvert.DeserializeObject<List<String>>(FilesJson);
                using (MemoryStream memory = new MemoryStream())
                {
                    byte[] buffer = new byte[6500];
                    MemoryStream returnStream = new MemoryStream();
                    var zipMs = new MemoryStream();

                    using (ZipOutputStream zipStream = new ZipOutputStream(zipMs))
                    {
                        zipStream.SetLevel(9);
                        foreach (var file in fileNames)
                        {
                            try
                            {
                                if (!File.Exists(basePath + file)) continue;
                                using (var StreamFileData = new FileStream(basePath + file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    string fileName = file;
                                    zipStream.PutNextEntry(new ZipEntry(fileName));
                                    while (true)
                                    {
                                        var readCount = StreamFileData.Read(buffer, 0, buffer.Length);
                                        if (readCount > 0)
                                        {
                                            zipStream.Write(buffer, 0, readCount);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    zipStream.Flush();
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error("Error occured while downloading the files and compressing them in a zip file " + ex);
                            }
                        }

                        zipStream.Finish();
                        zipMs.Position = 0;
                        zipMs.CopyTo(returnStream, 5600);
                    }
                    returnStream.Position = 0;
                    response.Content = new StreamContent(returnStream);
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = string.Format("download_compressed_{0}.zip", DateTime.Now.ToString("yyyyMMddHHmmss")) };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error occured while downloading the files and compressing them in a zip file " + ex);
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            return response;
        }

        [Authorize]
        [Route("pitchready/TemplateRepositoryExcel/UpdateTemplatesWorkbookInfo_EXL")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateTemplatesWorkbookInfo_EXL()
        {
            try
            {
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());
                NameValueCollection formData = provider.FormData;
                if (!Directory.Exists(ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"]))
                {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"]);
                }
                string tempDocUrl = ConfigurationManager.AppSettings["PitchreadyBasePathTemplateRepositoryExcel"];
                IList<HttpContent> FileStreamData = provider.Files;
                var OperationType = (CRUDType)Convert.ToInt32(formData["OperationType"]);
                var Data = formData["OperationData"];
                var WorksheetNameList = formData["WorksheetNameList"];
                var UserName = formData["UserName"];
                string procedureName = "UpdateTemplatesWorkbookInfo_EXL";
                ChildCategoryModel catData = Newtonsoft.Json.JsonConvert.DeserializeObject<ChildCategoryModel>(Data);
                List<LocalWorksheetInfo> worksheetNameList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LocalWorksheetInfo>>(WorksheetNameList);
                string filePath = string.Empty;
                List<WorksheetInfo> worksheetInfos = new List<WorksheetInfo>();

                if (catData.IsWorksheetInfoNeedToUpdate)
                {
                    if (FileStreamData != null || FileStreamData.Count() == 0)
                    {
                        var sysfileName = string.Empty;
                        int j = 0;
                        for (int i = 0; i < FileStreamData.Count - 1 || i < worksheetNameList.Count; i++)
                        {
                            if (FileStreamData.Count - 1 <= worksheetNameList.Count)
                            {
                                if (worksheetNameList[i].HasPreview)
                                {
                                    var fileName = Guid.NewGuid();
                                    var thisFileName = FileStreamData[j].Headers.ContentDisposition.FileName.Trim('\"');
                                    Stream File1Stream = await FileStreamData[j].ReadAsStreamAsync();

                                    if (!string.IsNullOrEmpty(thisFileName))
                                    {
                                        using (var input = File.Create(tempDocUrl + fileName + Path.GetExtension(thisFileName)))
                                        {
                                            File1Stream.CopyTo(input);
                                            sysfileName = fileName.ToString() + Path.GetExtension(thisFileName);
                                        }

                                        worksheetInfos.Add(new WorksheetInfo
                                        {
                                            WorksheetName = worksheetNameList[i].WorksheetName.ToString(),
                                            SystemWorksheetName = sysfileName,
                                            InsertedAt = DateTime.Now,
                                            InsertedBy = UserName,
                                            UpdatedAt = DateTime.Now,
                                            UpdatedBy = UserName
                                        });
                                    }
                                    j++;
                                }

                                else
                                {
                                    worksheetInfos.Add(new WorksheetInfo
                                    {
                                        WorksheetName = worksheetNameList[i].WorksheetName.ToString(),
                                        SystemWorksheetName = null,
                                        InsertedAt = DateTime.Now,
                                        InsertedBy = UserName,
                                        UpdatedAt = DateTime.Now,
                                        UpdatedBy = UserName
                                    });
                                }
                            }
                        }
                    }




                    if (FileStreamData != null && FileStreamData.Count() > 1)
                    {
                        var item = FileStreamData.LastOrDefault();
                        var fileName = Guid.NewGuid();
                        var thisFileName = item.Headers.ContentDisposition.FileName.Trim('\"');
                        Stream File1Stream = await item.ReadAsStreamAsync();
                        using (var input = File.Create(tempDocUrl + fileName + Path.GetExtension(thisFileName)))
                        {
                            File1Stream.CopyTo(input);
                            catData.SystemFileName = fileName.ToString() + Path.GetExtension(thisFileName);
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < worksheetNameList.Count; i++)
                    {
                        worksheetInfos.Add(new WorksheetInfo
                        {
                            WorksheetName = worksheetNameList[i].WorksheetName.ToString(),
                            SystemWorksheetName = null,
                            InsertedAt = DateTime.Now,
                            InsertedBy = UserName,
                            UpdatedAt = DateTime.Now,
                            UpdatedBy = UserName
                        });
                    }
                }

                var catResult = DatabaseCrudOperationsWithUDT(OperationType, catData, UserName, procedureName, worksheetInfos);
                if (catResult == null)
                {
                    return NotFound();
                }
                return Ok(catResult);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in TR Admin" + ex);
                return NotFound();
            }
        }
    }

    public class WorksheetInfo
    {
        public string WorksheetName { get; set; }
        public string SystemWorksheetName { get; set; }
        public DateTime? InsertedAt { get; set; }

        public string InsertedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }
    }

    public class LocalWorksheetInfo
    {
        public int? Id { get; set; }

        public string FileLocation { get; set; }

        public string WorksheetName { get; set; }

        public bool HasPreview { get; set; }
    }
}