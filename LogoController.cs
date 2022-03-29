using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPI.DataAccessLayer;
using WebAPI.Enumration;
using WebAPI.Models;
using WebAPI.Models.LogoModals;

namespace WebAPI.Controllers
{
    public class LogoController : ApiController
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        [Authorize]
        [Route("pitchready/Logo/GetAllField")]
        [HttpPost]
        public IHttpActionResult GetAllFieldCatagories([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                int? IsFilterRequired = null;
                var IsRequiredFilterStatus = apidata["IsRequiredFilterStatus"];
                if (!string.IsNullOrEmpty(IsRequiredFilterStatus))
                    IsFilterRequired = Convert.ToInt32(IsRequiredFilterStatus);
                var retStr = LogoDAL.GetAllField(IsFilterRequired);
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing DML operation in Manage Group Master " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/UpdateField")]
        [HttpPost]
        public IHttpActionResult DMLField([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<ManageFieldGroupMaster> returnList = new List<ManageFieldGroupMaster>();
                var ManageFieldData = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<ManageFieldGroupMaster> FieldDmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ManageFieldGroupMaster>>(ManageFieldData);

                foreach (var field in FieldDmlData)
                {
                    var Result = LogoDAL.DMLField(OperationType, field, UserName);
                    if (Result != null)
                    {
                        returnList.Add(Result);
                    }
                }
                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage Group Master" + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/AddFilterData")]
        [HttpPost]
        public IHttpActionResult DMLSaveFilterData([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<ManageHierarchyData> returnList = new List<ManageHierarchyData>();
                var ManageHierarchyData = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<ManageHierarchyData> HierarchyDmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ManageHierarchyData>>(ManageHierarchyData);
                foreach (var data in HierarchyDmlData)
                {
                    var Result = LogoDAL.SaveFilterData(OperationType, data, UserName);
                    if (Result != null)
                    {
                        returnList.Add(Result);
                    }
                }
                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage Hierarchy data" + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/DeleteMetaDataLevel1")]
        [HttpPost]
        public IHttpActionResult DeleteMetaDataLevel1([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel1> returnUpdateList = new List<DataLevel1>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                if (OperationType.ToString() == "CheckDeleteAll" || OperationType.ToString() == "DeleteAll")
                {
                    List<DataLevel1> Level1DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel1>>(ManageLevel1Data);
                    List<int> lstIDs = Level1DmlData.Select(x => x.ID).ToList();
                    var Level1Result = LogoDAL.DeleteAllLevel1(OperationType, lstIDs, UserName);
                    if (Level1Result != null)
                    {
                        returnUpdateList.Add(Level1Result);
                    }
                }
                else
                {
                    List<DataLevel1> Level1DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel1>>(ManageLevel1Data);
                    var Level1Result = LogoDAL.DeleteLevel1(OperationType, Level1DmlData[0], UserName);
                    if (Level1Result != null)
                    {
                        returnUpdateList.Add(Level1Result);
                    }
                }
                return Ok(returnUpdateList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level1 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/DeleteMetaDataLevel2")]
        [HttpPost]
        public IHttpActionResult DeleteMetaDataLevel2([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel2> returnUpdateList = new List<DataLevel2>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                if (OperationType.ToString() == "CheckDeleteAll" || OperationType.ToString() == "DeleteAll")
                {
                    List<DataLevel2> Level2DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel2>>(ManageLevel1Data);
                    List<int> lstIDs = Level2DmlData.Select(x => x.ID).ToList();
                    var Level1Result = LogoDAL.DeleteAllLevel2(OperationType, lstIDs, UserName);
                    if (Level1Result != null)
                    {
                        returnUpdateList.Add(Level1Result);
                    }
                }
                else
                {
                    List<DataLevel2> Level2DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel2>>(ManageLevel1Data);
                    var Level1Result = LogoDAL.DeleteLevel2(OperationType, Level2DmlData[0], UserName);
                    if (Level1Result != null)
                    {
                        returnUpdateList.Add(Level1Result);
                    }
                }
                return Ok(returnUpdateList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level2 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/DeleteMetaDataLevel3")]
        [HttpPost]
        public IHttpActionResult DeleteMetaDataLevel3([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel3> returnUpdateList = new List<DataLevel3>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                if (OperationType.ToString() == "CheckDeleteAll" || OperationType.ToString() == "DeleteAll")
                {
                    List<DataLevel3> Level3DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel3>>(ManageLevel1Data);
                    List<int> lstIDs = Level3DmlData.Select(x => x.ID).ToList();
                    var Level1Result = LogoDAL.DeleteAllLevel3(OperationType, lstIDs, UserName);
                    if (Level1Result != null)
                    {
                        returnUpdateList.Add(Level1Result);
                    }
                }
                else
                {
                    List<DataLevel3> Level3DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel3>>(ManageLevel1Data);
                    var Level1Result = LogoDAL.DeleteLevel3(OperationType, Level3DmlData[0], UserName);
                    if (Level1Result != null)
                    {
                        returnUpdateList.Add(Level1Result);
                    }
                }
                return Ok(returnUpdateList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level3 " + ex);
                return NotFound();
            }
        }


        [Authorize]
        [Route("pitchready/Logo/DeleteMetaDataLevel4")]
        [HttpPost]
        public IHttpActionResult DeleteMetaDataLevel4([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel4> returnUpdateList = new List<DataLevel4>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                if (OperationType.ToString() == "CheckDeleteAll" || OperationType.ToString() == "DeleteAll")
                {
                    List<DataLevel4> Level4DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel4>>(ManageLevel1Data);
                    List<int> lstIDs = Level4DmlData.Select(x => x.ID).ToList();
                    var Level1Result = LogoDAL.DeleteAllLevel4(OperationType, lstIDs, UserName);
                    if (Level1Result != null)
                    {
                        returnUpdateList.Add(Level1Result);
                    }
                }
                else
                {
                    List<DataLevel4> Level4DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel4>>(ManageLevel1Data);
                    var Level1Result = LogoDAL.DeleteLevel4(OperationType, Level4DmlData[0], UserName);
                    if (Level1Result != null)
                    {
                        returnUpdateList.Add(Level1Result);
                    }
                }
                return Ok(returnUpdateList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level4 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/UpdateMetaDataLevel1")]
        [HttpPost]
        public IHttpActionResult UpdateMetaDataLevel1([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel1> returnUpdateList = new List<DataLevel1>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<DataLevel1> Level1DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel1>>(ManageLevel1Data);
                var Level1Result = LogoDAL.UpdateLevel1(OperationType, Level1DmlData[0], UserName);
                if (Level1Result != null)
                {
                    returnUpdateList.Add(Level1Result);
                }
                return Ok(returnUpdateList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level1 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/UpdateMetaDataLevel2")]
        [HttpPost]
        public IHttpActionResult UpdateMetaDataLevel2([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel2> returnUpdateList = new List<DataLevel2>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<DataLevel2> Level2DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel2>>(ManageLevel1Data);
                var Level1Result = LogoDAL.UpdateLevel2(OperationType, Level2DmlData[0], UserName);
                if (Level1Result != null)
                {
                    returnUpdateList.Add(Level1Result);
                }
                return Ok(returnUpdateList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level2 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/UpdateMetaDataLevel3")]
        [HttpPost]
        public IHttpActionResult UpdateMetaDataLevel3([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel3> returnUpdateList = new List<DataLevel3>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<DataLevel3> Level3DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel3>>(ManageLevel1Data);
                var Level1Result = LogoDAL.UpdateLevel3(OperationType, Level3DmlData[0], UserName);
                if (Level1Result != null)
                {
                    returnUpdateList.Add(Level1Result);
                }
                return Ok(returnUpdateList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level3 " + ex);
                return NotFound();
            }
        }


        [Authorize]
        [Route("pitchready/Logo/UpdateMetaDataLevel4")]
        [HttpPost]
        public IHttpActionResult UpdateMetaDataLevel4([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel4> returnUpdateList = new List<DataLevel4>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<DataLevel4> Level4DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataLevel4>>(ManageLevel1Data);
                var Level1Result = LogoDAL.UpdateLevel4(OperationType, Level4DmlData[0], UserName);
                if (Level1Result != null)
                {
                    returnUpdateList.Add(Level1Result);
                }
                return Ok(returnUpdateList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level4 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/InsertMetaDataLevel1")]
        [HttpPost]
        public IHttpActionResult InsertMetaDataLevel1([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel1> returnList = new List<DataLevel1>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<ManageFilterMetaDataLevel1Modal> Level1DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ManageFilterMetaDataLevel1Modal>>(ManageLevel1Data);
                var Level1Result = LogoDAL.InsertLevel1(OperationType, Level1DmlData, UserName);
                if (Level1Result != null)
                {
                    returnList = Level1Result;
                }
                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level1 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/InsertMetaDataLevel2")]
        [HttpPost]
        public IHttpActionResult InsertMetaDataLevel2([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel2> returnList = new List<DataLevel2>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<ManageFilterMetaDataLevel2Modal> Level2DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ManageFilterMetaDataLevel2Modal>>(ManageLevel1Data);
                var Level2Result = LogoDAL.InsertLevel2(OperationType, Level2DmlData, UserName);
                if (Level2Result != null)
                {
                    returnList = Level2Result;
                }
                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level2 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/InsertMetaDataLevel3")]
        [HttpPost]
        public IHttpActionResult InsertMetaDataLevel3([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel3> returnList = new List<DataLevel3>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<ManageFilterMetaDataLevel3Modal> Level1DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ManageFilterMetaDataLevel3Modal>>(ManageLevel1Data);
                var Level1Result = LogoDAL.InsertLevel3(OperationType, Level1DmlData, UserName);
                if (Level1Result != null)
                {
                    returnList = Level1Result;
                }
                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level3 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/InsertMetaDataLevel4")]
        [HttpPost]
        public IHttpActionResult InsertMetaDataLevel4([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel4> returnList = new List<DataLevel4>();
                var ManageLevel1Data = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<ManageFilterMetaDataLevel4Modal> Level1DmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ManageFilterMetaDataLevel4Modal>>(ManageLevel1Data);
                var Level1Result = LogoDAL.InsertLevel4(OperationType, Level1DmlData, UserName);
                if (Level1Result != null)
                {
                    returnList = Level1Result;
                }
                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage MetaData Level4 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/GetMetaDataLevel1")]
        [HttpPost]
        public IHttpActionResult GetMetaDataLevel1([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel1> retStr = new List<DataLevel1>();
                List<string> FilterGroupId = new List<string>();
                int? FilterGId = null;
                if (apidata["FilterGroupId"].Contains(","))
                    FilterGroupId = apidata["FilterGroupId"].Split(',').ToList();
                else
                    FilterGroupId.Add(apidata["FilterGroupId"]);
                //if (!string.IsNullOrEmpty(FilterGroupId))
                if (FilterGroupId.Count != 0)
                {
                    if (FilterGroupId.Count > 1)
                    {
                        foreach (var filterGId in FilterGroupId)
                        {
                            FilterGId = Convert.ToInt32(filterGId);
                            retStr.AddRange(LogoDAL.GetAllDataLevel1(FilterGId));
                        }
                    }
                    else
                    {
                        FilterGId = Convert.ToInt32(FilterGroupId[0]);
                        retStr = LogoDAL.GetAllDataLevel1(FilterGId);
                    }
                }
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level1 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/GetMetaDataLevel2")]
        [HttpPost]
        public IHttpActionResult GetMetaDataLevel2([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel2> retStr = new List<DataLevel2>();
                List<string> FilterGroupId = new List<string>();
                int? FilterGId = null;
                if (apidata["FilterGroupId"].Contains(","))
                    FilterGroupId = apidata["FilterGroupId"].Split(',').ToList();
                else
                    FilterGroupId.Add(apidata["FilterGroupId"]);
                //if (!string.IsNullOrEmpty(FilterGroupId))
                if (FilterGroupId.Count != 0)
                {
                    if (FilterGroupId.Count > 1)
                    {
                        foreach (var filterGId in FilterGroupId)
                        {
                            FilterGId = Convert.ToInt32(filterGId);
                            retStr.AddRange(LogoDAL.GetAllDataLevel2(FilterGId));
                        }
                    }
                    else
                    {
                        FilterGId = Convert.ToInt32(FilterGroupId[0]);
                        retStr = LogoDAL.GetAllDataLevel2(FilterGId);
                    }
                }
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level2 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/GetMetaDataLevel3")]
        [HttpPost]
        public IHttpActionResult GetMetaDataLevel3([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel3> retStr = new List<DataLevel3>();
                List<string> FilterGroupId = new List<string>();
                int? FilterGId = null;
                if (apidata["FilterGroupId"].Contains(","))
                    FilterGroupId = apidata["FilterGroupId"].Split(',').ToList();
                else
                    FilterGroupId.Add(apidata["FilterGroupId"]);
                //if (!string.IsNullOrEmpty(FilterGroupId))
                if (FilterGroupId.Count != 0)
                {
                    if (FilterGroupId.Count > 1)
                    {
                        foreach (var filterGId in FilterGroupId)
                        {
                            FilterGId = Convert.ToInt32(filterGId);
                            retStr.AddRange(LogoDAL.GetAllDataLevel3(FilterGId));
                        }
                    }
                    else
                    {
                        FilterGId = Convert.ToInt32(FilterGroupId[0]);
                        retStr = LogoDAL.GetAllDataLevel3(FilterGId);
                    }
                }
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level3 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/GetMetaDataLevel4")]
        [HttpPost]
        public IHttpActionResult GetMetaDataLevel4([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel4> retStr = new List<DataLevel4>();
                List<string> FilterGroupId = new List<string>();
                int? FilterGId = null;
                if (apidata["FilterGroupId"].Contains(","))
                    FilterGroupId = apidata["FilterGroupId"].Split(',').ToList();
                else
                    FilterGroupId.Add(apidata["FilterGroupId"]);
                //if (!string.IsNullOrEmpty(FilterGroupId))
                if (FilterGroupId.Count != 0)
                {
                    if (FilterGroupId.Count > 1)
                    {
                        foreach (var filterGId in FilterGroupId)
                        {
                            FilterGId = Convert.ToInt32(filterGId);
                            retStr.AddRange(LogoDAL.GetAllDataLevel4(FilterGId));
                        }
                    }
                    else
                    {
                        FilterGId = Convert.ToInt32(FilterGroupId[0]);
                        retStr = LogoDAL.GetAllDataLevel4(FilterGId);
                    }
                }
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level4 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/GetAdminFilterDataLevel1")]
        [HttpPost]
        public IHttpActionResult GetAdminFilterDataLevel1([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel1> retStr = new List<DataLevel1>();

                retStr = LogoDAL.GetAdminFilterDataLevel1();
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level1 " + ex);
                return NotFound();
            }
        }
        [Authorize]
        [Route("pitchready/Logo/GetAdminFilterDataLevel2")]
        [HttpPost]
        public IHttpActionResult GetAdminFilterDataLevel2([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel2> retStr = new List<DataLevel2>();

                retStr = LogoDAL.GetAdminFilterDataLevel2();
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level1 " + ex);
                return NotFound();
            }
        }
        [Authorize]
        [Route("pitchready/Logo/GetAdminFilterDataLevel3")]
        [HttpPost]
        public IHttpActionResult GetAdminFilterDataLevel3([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel3> retStr = new List<DataLevel3>();

                retStr = LogoDAL.GetAdminFilterDataLevel3();
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level3 " + ex);
                return NotFound();
            }
        }
        [Authorize]
        [Route("pitchready/Logo/GetAdminFilterDataLevel4")]
        [HttpPost]
        public IHttpActionResult GetAdminFilterDataLevel4([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel4> retStr = new List<DataLevel4>();

                retStr = LogoDAL.GetAdminFilterDataLevel4();
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level4 " + ex);
                return NotFound();
            }
        }
        [Authorize]
        [Route("pitchready/Logo/GetFilterDataLevel1")]
        [HttpPost]
        public IHttpActionResult GetFilterDataLevel1([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel1> retStr = new List<DataLevel1>();

                retStr = LogoDAL.GetFilterDataLevel1();
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level1 " + ex);
                return NotFound();
            }
        }
        [Authorize]
        [Route("pitchready/Logo/GetFilterDataLevel2")]
        [HttpPost]
        public IHttpActionResult GetFilterDataLevel2([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel2> retStr = new List<DataLevel2>();

                retStr = LogoDAL.GetFilterDataLevel2();
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level1 " + ex);
                return NotFound();
            }
        }
        [Authorize]
        [Route("pitchready/Logo/GetFilterDataLevel3")]
        [HttpPost]
        public IHttpActionResult GetFilterDataLevel3([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel3> retStr = new List<DataLevel3>();

                retStr = LogoDAL.GetFilterDataLevel3();
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level3 " + ex);
                return NotFound();
            }
        }
        [Authorize]
        [Route("pitchready/Logo/GetFilterDataLevel4")]
        [HttpPost]
        public IHttpActionResult GetFilterDataLevel4([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<DataLevel4> retStr = new List<DataLevel4>();

                retStr = LogoDAL.GetFilterDataLevel4();
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching Meta Data Level4 " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/GetAllFilterById")]
        [HttpPost]
        public IHttpActionResult GetAllFilterCatagories([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                int? FilterGId = null;
                List<ManageHierarchyData> retStr = new List<ManageHierarchyData>();
                List<string> FilterGroupId = new List<string>();
                if (apidata["FilterGroupId"].Contains(","))
                    FilterGroupId = apidata["FilterGroupId"].Split(',').ToList();
                else
                    FilterGroupId.Add(apidata["FilterGroupId"]);
                if (FilterGroupId.Count != 0)
                {
                    if (FilterGroupId.Count > 1)
                    {
                        foreach (var filterGId in FilterGroupId)
                        {
                            FilterGId = Convert.ToInt32(filterGId);
                            retStr.AddRange(LogoDAL.GetAllFiltersByID(FilterGId));
                            retStr.Where(x => x.FilterGroupId == 0).ToList().ForEach(x => x.FilterGroupId = Convert.ToInt32(filterGId));
                        }
                    }
                    else
                    {
                        FilterGId = Convert.ToInt32(FilterGroupId[0]);
                        retStr = LogoDAL.GetAllFiltersByID(FilterGId);
                    }
                }
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to fetching all filter group " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/DeleteAllFiltersById")]
        [HttpPost]
        public IHttpActionResult DeleteAllFilters([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                int? FilterGId = null;
                var FilterGroupId = apidata["FilterGroupId"];
                if (!string.IsNullOrEmpty(FilterGroupId))
                    FilterGId = Convert.ToInt32(FilterGroupId);
                var retStr = LogoDAL.DeleteAllFiltersByID(FilterGId);
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing operation to deleteing all filter groupup " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/GetAllCompanyMasterData")]
        [HttpPost]
        public IHttpActionResult GetAllCompanyMasterData([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                int? IsFilterRequired = null;
                var IsRequiredFilterStatus = apidata["IsRequiredFilterStatus"];
                if (!string.IsNullOrEmpty(IsRequiredFilterStatus))
                    IsFilterRequired = Convert.ToInt32(IsRequiredFilterStatus);
                var retStr = LogoDAL.GetAllCompanyMasterData1(IsFilterRequired);
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing DML operation in Manage Company Master " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/InsertCompanyData")]
        [HttpPost]
        public async Task<IHttpActionResult> InsertCompanyData()
        {
            try
            {
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());
                NameValueCollection formData = provider.FormData;
                //access files  
                string tempDocUrl = ConfigurationManager.AppSettings["PitchreadyBasePathLogoTool"];
                IList<HttpContent> FileStreamData = provider.Files;
                //var ManageTombstoneTemplateData = formData["Dealdata"];

                List<InsertCompanyData> returnList = new List<InsertCompanyData>();
                var CompanyData = formData["OperationData"];
                var OperationType = (CRUDType)Convert.ToInt32(formData["OperationType"]);
                var UserName = formData["UserName"];
                List<CompanyData> CompanyDataa = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CompanyData>>(CompanyData);
                foreach (var item1 in CompanyDataa)
                {
                    string filePath = string.Empty;
                    if (FileStreamData != null || FileStreamData.Count() > 0)
                    {
                        foreach (var item in FileStreamData)
                        {
                            string FileName = null;
                            var fileName = Guid.NewGuid();
                            var Filekey = item.Headers.ContentDisposition.Name.Trim('\"');
                            var SplitStr = Regex.Split(Filekey, "__");
                            var CompanyId = Convert.ToInt32(SplitStr[SplitStr.Length - 1]);
                            var thisFileName = item.Headers.ContentDisposition.FileName.Trim('\"');
                            for (int i = 0; i < SplitStr.Length - 1; i++)
                            {
                                FileName = string.Join("", SplitStr[i]);
                            }

                            if (item1.CompanyID == CompanyId)
                            {
                                Stream File1Stream = await item.ReadAsStreamAsync();
                                using (var input = File.Create(tempDocUrl + fileName + Path.GetExtension(FileName)))
                                {
                                    File1Stream.CopyTo(input);
                                    item1.SystemLogoName = fileName + Path.GetExtension(FileName);
                                }
                            }
                        }
                    }
                }

                var CompanydataResult = LogoDAL.InsertCompanyMasterDataWithMapping(CompanyDataa);
                if (CompanydataResult != null)
                {
                    returnList = CompanydataResult;
                    var records = CompanydataResult.Where(x => x.Message == ErrorMessage.Duplicate);
                    if (records != null && records.Count() > 0)
                    {
                        foreach (var item in records)
                        {
                            tryToDelete(tempDocUrl + item.SystemLogoName);
                        }
                    }
                }
                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Insert Company data " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/UpdateCompanyData")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateCompanyData()
        {
            try
            {
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());
                NameValueCollection formData = provider.FormData;
                //access files  
                string tempDocUrl = ConfigurationManager.AppSettings["PitchreadyBasePathLogoTool"];
                IList<HttpContent> FileStreamData = provider.Files;
                //var ManageTombstoneTemplateData = formData["Dealdata"];

                List<InsertCompanyData> returnList = new List<InsertCompanyData>();
                var CompanyData = formData["OperationData"];
                var OperationType = (CRUDType)Convert.ToInt32(formData["OperationType"]);
                var UserName = formData["UserName"];
                List<CompanyData> CompanyDataa = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CompanyData>>(CompanyData);
                foreach (var item1 in CompanyDataa)
                {
                    string filePath = string.Empty;
                    if (FileStreamData != null || FileStreamData.Count() == 0)
                    {
                        //foreach (var item in FileStreamData)
                        //{
                        //    var fileName = Guid.NewGuid();
                        //    var thisFileName = item.Headers.ContentDisposition.FileName.Trim('\"');
                        //    Stream File1Stream = await item.ReadAsStreamAsync();
                        //    using (var input = File.Create(tempDocUrl + fileName + Path.GetExtension(thisFileName)))
                        //    {
                        //        File1Stream.CopyTo(input);
                        //        item1.SystemLogoName = fileName + Path.GetExtension(thisFileName);
                        //    }
                        //}
                        foreach (var item in FileStreamData)
                        {
                            string FileName = null;
                            var fileName = Guid.NewGuid();
                            var Filekey = item.Headers.ContentDisposition.Name.Trim('\"');
                            var SplitStr = Regex.Split(Filekey, "__");
                            var CompanyId = Convert.ToInt32(SplitStr[SplitStr.Length - 1]);
                            var thisFileName = item.Headers.ContentDisposition.FileName.Trim('\"');
                            for (int i = 0; i < SplitStr.Length - 1; i++)
                            {
                                FileName = string.Join("", SplitStr[i]);
                            }

                            if (item1.CompanyID == CompanyId)
                            {
                                Stream File1Stream = await item.ReadAsStreamAsync();
                                using (var input = File.Create(tempDocUrl + fileName + Path.GetExtension(FileName)))
                                {
                                    File1Stream.CopyTo(input);
                                    item1.SystemLogoName = fileName + Path.GetExtension(FileName);
                                }
                            }
                        }

                    }
                }

                var CompanydataResult = LogoDAL.UpdateCompanyMasterDataWithMapping(CompanyDataa, false);
                if (CompanydataResult != null)
                {
                    returnList = CompanydataResult;
                    var records = CompanydataResult.Where(x => x.Message == ErrorMessage.Duplicate);
                    if (records != null && records.Count() > 0)
                    {
                        foreach (var item in records)
                        {
                            tryToDelete(tempDocUrl + item.SystemLogoName);
                        }
                    }
                }

                return Ok(returnList);

            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Insert Company data " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/BulkUpdateCompanyData")]
        [HttpPost]
        public async Task<IHttpActionResult> BulkUpdateCompanyData()
        {
            try
            {
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());
                NameValueCollection formData = provider.FormData;
                //access files  
                string tempDocUrl = ConfigurationManager.AppSettings["PitchreadyBasePathLogoTool"];
                IList<HttpContent> FileStreamData = provider.Files;
                //var ManageTombstoneTemplateData = formData["Dealdata"];

                List<InsertCompanyData> returnList = new List<InsertCompanyData>();
                var CompanyData = formData["OperationData"];
                var OperationType = (CRUDType)Convert.ToInt32(formData["OperationType"]);
                var UserName = formData["UserName"];
                List<CompanyData> CompanyDataa = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CompanyData>>(CompanyData);
                foreach (var item1 in CompanyDataa)
                {
                    string filePath = string.Empty;
                    if (FileStreamData != null || FileStreamData.Count() == 0)
                    {
                        //foreach (var item in FileStreamData)
                        //{
                        //    var fileName = Guid.NewGuid();
                        //    var thisFileName = item.Headers.ContentDisposition.FileName.Trim('\"');
                        //    Stream File1Stream = await item.ReadAsStreamAsync();
                        //    using (var input = File.Create(tempDocUrl + fileName + Path.GetExtension(thisFileName)))
                        //    {
                        //        File1Stream.CopyTo(input);
                        //        item1.SystemLogoName = fileName + Path.GetExtension(thisFileName);
                        //    }
                        //}
                        foreach (var item in FileStreamData)
                        {
                            string FileName = null;
                            var fileName = Guid.NewGuid();
                            var Filekey = item.Headers.ContentDisposition.Name.Trim('\"');
                            var SplitStr = Regex.Split(Filekey, "__");
                            var CompanyId = Convert.ToInt32(SplitStr[SplitStr.Length - 1]);
                            var thisFileName = item.Headers.ContentDisposition.FileName.Trim('\"');
                            for (int i = 0; i < SplitStr.Length - 1; i++)
                            {
                                FileName = string.Join("", SplitStr[i]);
                            }

                            if (item1.CompanyID == CompanyId)
                            {
                                Stream File1Stream = await item.ReadAsStreamAsync();
                                using (var input = File.Create(tempDocUrl + fileName + Path.GetExtension(FileName)))
                                {
                                    File1Stream.CopyTo(input);
                                    item1.SystemLogoName = fileName + Path.GetExtension(FileName);
                                }
                            }
                        }

                    }
                }

                var CompanydataResult = LogoDAL.UpdateCompanyMasterDataWithMapping(CompanyDataa, true);
                if (CompanydataResult != null)
                {
                    returnList = CompanydataResult;
                    var records = CompanydataResult.Where(x => x.Message == ErrorMessage.Duplicate);
                    if (records != null && records.Count() > 0)
                    {
                        foreach (var item in records)
                        {
                            tryToDelete(tempDocUrl + item.SystemLogoName);
                        }
                    }
                }
                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Insert Company data " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/DeleteCompanyData")]
        [HttpPost]
        public IHttpActionResult DeleteCompanyData([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<CompanyMasterData> returnList = new List<CompanyMasterData>();
                string tempDocUrl = ConfigurationManager.AppSettings["PitchreadyBasePathLogoTool"];
                var CompanyData = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<CompanyMasterData> CompanyDataa = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CompanyMasterData>>(CompanyData);
                List<int> companyIDs = CompanyDataa.Select(x => x.ID).ToList();
                List<string> filesToDelete = CompanyDataa.Select(x => x.SystemLogoName).ToList();
                var allDeals = LogoDAL.GetAllCompanyMasterData1(0);
                bool isdeleteallCalled;
                if (companyIDs.Count() == allDeals.Select(x => x.ID).Distinct().ToList().Count())
                {
                    isdeleteallCalled = true;
                }
                else
                {
                    isdeleteallCalled = false;
                }

                var CompanydataResult = LogoDAL.DeleteCompanyData(companyIDs, isdeleteallCalled);

                if (filesToDelete.Count > 0)
                {
                    int divideBy4Count = filesToDelete.Count() / 5;
                    int remainingCount = filesToDelete.Count % 5;
                    List<string> FileTODeleteInChunk = new List<string>();
                    FileTODeleteInChunk.AddRange(filesToDelete.Take(divideBy4Count + remainingCount).ToList());
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {

                        foreach (var item in FileTODeleteInChunk)
                            tryToDelete(tempDocUrl + item);
                    });
                    filesToDelete = filesToDelete.Except(FileTODeleteInChunk).ToList();
                    if (filesToDelete.Count > 0)
                    {
                        FileTODeleteInChunk = new List<string>();
                        FileTODeleteInChunk.AddRange(filesToDelete.Take(divideBy4Count).ToList());
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            foreach (var item in FileTODeleteInChunk)
                                tryToDelete(tempDocUrl + item);

                        });
                    }
                    filesToDelete = filesToDelete.Except(FileTODeleteInChunk).ToList();
                    if (filesToDelete.Count > 0)
                    {
                        FileTODeleteInChunk = new List<string>();
                        FileTODeleteInChunk.AddRange(filesToDelete.Take(divideBy4Count).ToList());
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            foreach (var item in FileTODeleteInChunk)
                                tryToDelete(tempDocUrl + item);

                        });
                    }
                    filesToDelete = filesToDelete.Except(FileTODeleteInChunk).ToList();
                    if (filesToDelete.Count > 0)
                    {
                        FileTODeleteInChunk = new List<string>();
                        FileTODeleteInChunk.AddRange(filesToDelete.Take(divideBy4Count).ToList());
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            foreach (var item in FileTODeleteInChunk)
                                tryToDelete(tempDocUrl + item);

                        });
                    }
                    filesToDelete = filesToDelete.Except(FileTODeleteInChunk).ToList();
                    if (filesToDelete.Count > 0)
                    {
                        FileTODeleteInChunk = new List<string>();
                        FileTODeleteInChunk.AddRange(filesToDelete.Take(divideBy4Count).ToList());
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            foreach (var item in FileTODeleteInChunk)
                                tryToDelete(tempDocUrl + item);
                        });
                    }
                }
                if (CompanydataResult != null)
                {
                    returnList.Add(CompanydataResult);
                }
                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Insert Company data " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/UpdateCompanyFlag")]
        [HttpPost]
        public IHttpActionResult UpdateCompanyFlag([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<CompanyMasterData> returnList = new List<CompanyMasterData>();
                var CompanyData = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<CompanyMasterData> CompanyMasterData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CompanyMasterData>>(CompanyData);
                var CompanydataResult = LogoDAL.UpdateCompanyFlag(CompanyMasterData);
                if (CompanydataResult != null)
                {
                    returnList.Add(CompanydataResult);
                }
                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Insert Company data " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/GetAllClientCompanyMasterData")]
        [HttpPost]
        public IHttpActionResult GetAllCompanyMasterDataClient([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                var filterdata = apidata["FilterData"];
                string searchString = apidata["SearchString"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                List<FilterDataModal> lstFilterData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FilterDataModal>>(filterdata);
                var retStr = LogoDAL.GetClientCompanyMasterData1(OperationType, searchString, lstFilterData);
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while performing DML operation in Manage Company Master " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/GetAllLogoTemplateData")]
        [HttpGet]
        public IHttpActionResult GetAllLogoTemplateData()
        {
            try
            {
                var retStr = LogoDAL.GetAllLogoTemplateData();
                return Ok(retStr);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while downloading GetAllLogoTemplateData data " + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/Logo/DMLLogoTemplate")]
        [HttpPost]
        public async Task<IHttpActionResult> DMLLogoTemplate()
        {
            try
            {
                //string returnString = null;
                ManageLogoTemplateModel returnList = new ManageLogoTemplateModel();
                var provider = await Request.Content.ReadAsMultipartAsync(new InMemoryMultipartFormDataStreamProvider());
                NameValueCollection formData = provider.FormData;
                //access files  
                string tempDocUrl = ConfigurationManager.AppSettings["PitchreadyBasePathLogoTool"];
                IList<HttpContent> FileStreamData = provider.Files;
                var ManageLogoTemplateData = formData["OperationData"];
                var OperationType = (CRUDType)Convert.ToInt32(formData["OperationType"]);
                var UserName = formData["UserName"];
                ManageLogoTemplateModel LogoTemplateDmlData =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<ManageLogoTemplateModel>(ManageLogoTemplateData);
                string filePath = string.Empty;
                if (FileStreamData != null || FileStreamData.Count() > 0)
                {
                    foreach (var item in FileStreamData)
                    {
                        var fileName = Guid.NewGuid();
                        var thisFileName = item.Headers.ContentDisposition.FileName.Trim('\"');
                        Stream File1Stream = await item.ReadAsStreamAsync();
                        using (var input = File.Create(tempDocUrl + fileName + Path.GetExtension(thisFileName)))
                        {
                            //  DeckTemplateDmlData.OriginalFileName = thisFileName;
                            File1Stream.CopyTo(input);
                            if (Path.GetExtension(thisFileName).Equals(".pptx", StringComparison.CurrentCultureIgnoreCase))
                                LogoTemplateDmlData.FileName = fileName + Path.GetExtension(thisFileName);
                            else if (Path.GetExtension(thisFileName).Equals(".png", StringComparison.CurrentCultureIgnoreCase))
                                LogoTemplateDmlData.TemplatePreviewFileName = fileName + Path.GetExtension(thisFileName);
                        }
                    }
                }


                // CRUDType OperationType = operationType.Cast<CRUDType>().FirstOrDefault();


                var LogoTemplateResult = LogoDAL.DMLLogoTemplate(OperationType, LogoTemplateDmlData, UserName);
                if (LogoTemplateResult != null)
                {
                    returnList = (LogoTemplateResult);
                }
                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage Logo Template" + ex);
                return NotFound();
            }
        }


        [Authorize]
        [Route("pitchready/Logo/DeleteLogoTemplate")]
        [HttpPost]
        public IHttpActionResult DeleteLogoTemplate([FromBody] Dictionary<string, string> apidata)
        {
            try
            {
                List<ManageLogoTemplateModel> returnList = new List<ManageLogoTemplateModel>();

                //access files  
                string tempDocUrl = ConfigurationManager.AppSettings["PitchreadyBasePathLogoTool"];
                //IList<HttpContent> FileStreamData = provider.Files;
                var ManageLogoTemplateData = apidata["Data"];
                var OperationType = (CRUDType)Convert.ToInt32(apidata["OperationType"]);
                var UserName = apidata["UserName"];
                List<ManageLogoTemplateModel> LogoTemplateDmlData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ManageLogoTemplateModel>>(ManageLogoTemplateData);
                string filePath = string.Empty;

                //List<int> lstTemplateIDs = LogoTemplateDmlData.Select(x => x.LogoTemplateId).ToList();
                foreach (ManageLogoTemplateModel TemplateData in LogoTemplateDmlData)
                {
                    var LogoTemplateResult = LogoDAL.DeleteLogoTemplate(OperationType, TemplateData, UserName);
                    if (LogoTemplateResult != null)
                    {
                        LogoTemplateResult.TemplateName = TemplateData.TemplateName;
                        returnList.Add(LogoTemplateResult);
                    }
                }

                List<string> filesToDelete = LogoTemplateDmlData.Select(x => x.TemplatePreviewFileName).ToList();
                filesToDelete.AddRange(LogoTemplateDmlData.Select(x => x.FileName).ToList());

                if (filesToDelete.Count > 0)
                {
                    int divideBy4Count = filesToDelete.Count() / 5;
                    int remainingCount = filesToDelete.Count % 5;
                    List<string> FileTODeleteInChunk = new List<string>();
                    FileTODeleteInChunk.AddRange(filesToDelete.Take(divideBy4Count + remainingCount).ToList());
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {

                        foreach (var item in FileTODeleteInChunk)
                            tryToDelete(tempDocUrl + item);
                    });
                    filesToDelete = filesToDelete.Except(FileTODeleteInChunk).ToList();
                    if (filesToDelete.Count > 0)
                    {
                        FileTODeleteInChunk = new List<string>();
                        FileTODeleteInChunk.AddRange(filesToDelete.Take(divideBy4Count).ToList());
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            foreach (var item in FileTODeleteInChunk)
                                tryToDelete(tempDocUrl + item);

                        });
                    }
                    filesToDelete = filesToDelete.Except(FileTODeleteInChunk).ToList();
                    if (filesToDelete.Count > 0)
                    {
                        FileTODeleteInChunk = new List<string>();
                        FileTODeleteInChunk.AddRange(filesToDelete.Take(divideBy4Count).ToList());
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            foreach (var item in FileTODeleteInChunk)
                                tryToDelete(tempDocUrl + item);

                        });
                    }
                    filesToDelete = filesToDelete.Except(FileTODeleteInChunk).ToList();
                    if (filesToDelete.Count > 0)
                    {
                        FileTODeleteInChunk = new List<string>();
                        FileTODeleteInChunk.AddRange(filesToDelete.Take(divideBy4Count).ToList());
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            foreach (var item in FileTODeleteInChunk)
                                tryToDelete(tempDocUrl + item);

                        });
                    }
                    filesToDelete = filesToDelete.Except(FileTODeleteInChunk).ToList();
                    if (filesToDelete.Count > 0)
                    {
                        FileTODeleteInChunk = new List<string>();
                        FileTODeleteInChunk.AddRange(filesToDelete.Take(divideBy4Count).ToList());
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            foreach (var item in FileTODeleteInChunk)
                                tryToDelete(tempDocUrl + item);
                        });
                    }
                }

                return Ok(returnList);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while Performing DML operations in Manage Logo Template" + ex);
                return NotFound();
            }
        }

        [Authorize]
        [Route("pitchready/LogoTool/getCompressedfiles")]
        [HttpPost]
        public HttpResponseMessage getCompressedfiles()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                List<string> returnValue = new List<string>();
                var basePath = ConfigurationManager.AppSettings["PitchreadyBasePathLogoTool"].ToString();
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
    }
}
