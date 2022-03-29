using NLog;
using System;
using System.Configuration;
using System.IO;
using System.Web.Http;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebAPI.Models;
using System.Net.Http;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;
using System.Net.Http.Headers;
using System.Globalization;
using WebAPI.Extensions;

namespace WebAPI.Controllers
{
    public class FeatureLogController : ApiController
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public static Queue<FeatureModels> queue = new Queue<FeatureModels>();
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Method to fetch tracking logs
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns></returns>
        [HttpPost]
        [Route("pitchready/FeatureLog/GetData")]
        public IHttpActionResult GetData([FromBody] Dictionary<string, string> apidata)
        {
            List<FeatureModels> FeatureTrackingLogs = new List<FeatureModels>();
            var path = ConfigurationManager.AppSettings["FeatureTrackingFileDirectory"].ToString();
            try
            {
                var fileName = ConfigurationManager.AppSettings["ClientId"] + "_" + ConfigurationManager.AppSettings["Region"]
                    + "_TrackFeatureLog.xml";

                XDocument doc = XDocument.Load(path + fileName);
                var startDateArray = apidata["StartDate"].Split('/');
                var endDateArray = apidata["EndDate"].Split('/');
                var startDate = new DateTime(Convert.ToInt32(startDateArray[2]), Convert.ToInt32(startDateArray[0]), Convert.ToInt32(startDateArray[1]));
                var endDate = new DateTime(Convert.ToInt32(endDateArray[2]), Convert.ToInt32(endDateArray[0]), Convert.ToInt32(endDateArray[1]));
                foreach (XElement element in doc.Descendants("TrackFeatures").Descendants("TrackFeature"))
                {
                    string[] currentDateArray;
                    if (element.Element("TimeStamp").Value.Contains("/"))
                        currentDateArray = element.Element("TimeStamp").Value.Split(' ').FirstOrDefault().Split('/');
                    else
                        currentDateArray = element.Element("TimeStamp").Value.Split(' ').FirstOrDefault().Split('-');
                    var currentDate = new DateTime(Convert.ToInt32(currentDateArray[2]), Convert.ToInt32(currentDateArray[0]), Convert.ToInt32(currentDateArray[1]));
                    if (currentDate >= startDate && currentDate <= endDate)
                    {
                        FeatureModels fm = new FeatureModels();
                        fm.ID = Convert.ToInt32(element.Element("ID").Value);
                        fm.UserName = element.Element("UserName").Value;
                        fm.MachineID = element.Element("MachineID").Value;//
                        fm.Module = element.Element("Module").Value;
                        fm.Feature = element.Element("Feature").Value;
                        fm.Activity = element.Element("Activity").Value;
                        fm.ElapsedTime = element.Element("ElapsedTime").Value;
                        fm.FileSize = element.Element("FileSize").Value;
                        fm.AccessCount = Convert.ToInt32(element.Element("AccessCount").Value);
                        fm.TimeStamp = element.Element("TimeStamp").Value;
                        fm.Month_Year = DateTime.ParseExact(fm.TimeStamp.Split(' ').FirstOrDefault(), 
                            "MM/dd/yyyy", CultureInfo.InvariantCulture).Date.PitchReadyMonthYearFormat();
                        fm.Date = fm.TimeStamp.Split(' ').FirstOrDefault();
                        fm.User = fm.UserName.Substring(fm.UserName.IndexOf("\\") + 1);
                        FeatureTrackingLogs.Add(fm);
                    }
                }
                return Ok(FeatureTrackingLogs);
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while getting the feature tracking logs" + ex);
                if (FeatureTrackingLogs.Any())
                {
                    return Ok(FeatureTrackingLogs);
                }
                return NotFound();
            }
        }

        [Authorize]
        public IHttpActionResult Post()
        {
            var DataJson = Request.Content.ReadAsStringAsync().Result;
            var values = JsonConvert.DeserializeObject<string[]>(DataJson);
            FeatureModels obj = new FeatureModels();
            obj.UserName = values[0];
            obj.MachineID = values[1];
            obj.Module = values[2];
            obj.Feature = values[3];
            obj.Activity = values[4];
            obj.ElapsedTime = values[5];
            obj.FileSize = values[6];
            obj.AccessCount = Convert.ToInt32(values[7]);
            obj.TimeStamp = DateTime.Now.PitchReadyCommanDTFormat();
            queue.Enqueue(obj);
            return Ok();
        }

        /// <summary>
        /// WriteFeaturelog
        /// </summary>
        /// <param name="CurrentTool"></param>
        /// <param name="userName"></param>
        /// <param name="macID"></param>
        /// <param name="accessCount"></param>
        /// <param name="activity"></param>
        public static void WriteFeaturelog(FeatureModels featureModel)
        {
            try
            {
                string basePath = ConfigurationManager.AppSettings["FeatureTrackingFileDirectory"].ToString();
                string regionName = ConfigurationManager.AppSettings["Region"].ToString();
                string Client = ConfigurationManager.AppSettings["ClientId"].ToString();
                string fileNm = Client + "_" + regionName + "_TrackFeatureLog" + ".xml";
                var targetDirectory = Path.Combine(basePath, fileNm);
                #region TrackFeatureLog add
                if (!File.Exists(targetDirectory))
                {
                    XDocument doc = new XDocument(
                      new XDeclaration("1.0", "UTF-8", "yes"),
                      new XElement("TrackFeatures"));
                    doc.Save(targetDirectory);
                }
                XDocument addTrackFeaturesXML = XDocument.Load(targetDirectory);
                int id = 0;
                int nodeCount = Convert.ToInt32(addTrackFeaturesXML.Descendants("TrackFeature").LongCount());
                if (nodeCount > 0)
                {
                    id = addTrackFeaturesXML.Descendants("TrackFeature").Max(x => (int)x.Element("ID"));
                }
                XElement newTrackFeature = new XElement("TrackFeature"
                                               , new XElement("ID", id + 1)
                                               , new XElement("UserName", featureModel.UserName)
                                               , new XElement("MachineID", featureModel.MachineID)
                                               , new XElement("Module", featureModel.Module)
                                               , new XElement("Feature", featureModel.Feature)
                                               , new XElement("Activity", featureModel.Activity)
                                               , new XElement("ElapsedTime", featureModel.ElapsedTime)
                                               , new XElement("FileSize", featureModel.FileSize)
                                               , new XElement("AccessCount", featureModel.AccessCount)
                                               , new XElement("TimeStamp", featureModel.TimeStamp)//DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"))
                                          );
                addTrackFeaturesXML.Element("TrackFeatures").Add(newTrackFeature);
                addTrackFeaturesXML.Save(targetDirectory);
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("Error occured when saving TrackFeature with count record in xml. Message: " + ex.Message + ". StackTrace: " + ex.StackTrace + ". Datetime: " + DateTime.Now);
            }
        }
    }
}