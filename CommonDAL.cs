using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebAPI.Classes;
using WebAPI.Controllers;
using WebAPI.Enumration;
using WebAPI.Models.DesignToolkitModals;
using WebAPI.Models.TemplateRepositoryExcel;

namespace WebAPI.DataAccessLayer.CommonDataAccessLayer
{
    public static class CommonDAL
    {
        #region private variables

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        /// <summary>
        /// Method that manages crud operations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operationType">stores operation type</param>
        /// <param name="modelData">stores model data of type T</param>
        /// <param name="userName">stores user name</param>
        /// <param name="procedureName">stores procedure name</param>
        /// <returns></returns>
        internal static T DatabaseCrudOperations<T>(CRUDType operationType, T modelData, string userName, string procedureName)
        {
            dynamic result = modelData;
            DatabaseManager _dbManager = null;
            // Create instance of 'T'
            dynamic obj = (T)Activator.CreateInstance(typeof(T));
            int res = 0;

            try
            {
                _dbManager = new DatabaseManager(DALConstants.CONNECTION_KEY);
                _dbManager.prepareProcedure(procedureName);

                switch (operationType)
                {
                    case CRUDType.Insert:
                        obj.Insert(_dbManager, modelData, userName, operationType);
                        break;

                    case CRUDType.Delete:
                        obj.Delete(_dbManager, modelData, userName, operationType);
                        break;

                    case CRUDType.Update:
                    case CRUDType.Submit:
                        obj.Update(_dbManager, modelData, userName, operationType);
                        break;
                }

                res = _dbManager._command.ExecuteNonQuery();

                if (res != 1 && operationType == CRUDType.Update)
                {
                    if (res != 1)
                    {
                        SqlDataReader data = _dbManager._command.ExecuteReader();
                        while (data.Read())
                        {
                            res = Convert.ToInt32(data["ReturnCode"]);
                        }
                        if (res != 4)
                        {
                            res = 0;
                        }
                    }
                }
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
                if (operationType == CRUDType.Update)
                {
                    FinallyBlockStatementsForUpdate(result, res);
                }
                else
                {
                    FinallyBlockStatements(result, res);
                }
                _dbManager.Close();
                _dbManager.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Statements that are to be executed in finally block in DatabaseCrudOperations method 
        /// when insert and delete operations are executed
        /// </summary>
        /// <param name="result"></param>
        /// <param name="res"></param>
        public static void FinallyBlockStatements(dynamic result, int res)
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
        }



        /// <summary>
        /// /// <summary>
        /// Statements that are to be executed in finally block in DatabaseCrudOperations method 
        /// when update operation is executed
        /// </summary>
        /// <param name="result"></param>
        /// <param name="res"></param>
        public static void FinallyBlockStatementsForUpdate(dynamic result, int res)
        {
            result.SatusCode = res;
            switch (res)
            {
                case 0:
                    result.Message = ErrorMessage.RefreshData;
                    break;
                case 4:
                    result.Message = ErrorMessage.Duplicate;
                    break;
                case 1:
                    result.Message = ErrorMessage.Successfully;
                    break;
                case 3:
                    result.Message = ErrorMessage.ReferenceConstraint;
                    break;
                default:
                    result.Message = ErrorMessage.SQLServerDown;
                    break;
            }
        }



        /// <summary>
        /// Method to get all the data from table in a database
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        internal static List<object> GetAllData(string procedureName, Type className, int? param = null)
        {
            List<object> returnValue = new List<object>();
            DatabaseManager _dbManager = null;
            try
            {
                _dbManager = new DatabaseManager(DALConstants.CONNECTION_KEY);
                
                _dbManager.prepareProcedure(procedureName);
                if (param != null)
                    _dbManager._command.Parameters.Add(new SqlParameter("@categoryID", param.Value));
                SqlDataReader data = _dbManager._command.ExecuteReader();
                while (data.Read())
                {
                    dynamic obj = Activator.CreateInstance(className);
                    obj.GetData(data);
                    returnValue.Add(obj);
                }
                _dbManager.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw (ex);
            }
            finally
            {
                _dbManager.Dispose();
            }
            return returnValue;
        }

        /// <summary>
        /// Method to get all the data from table in a database
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        internal static List<object> GetAllDataById(string procedureName, Type className, string paramKey, int? param = null)
        {
            List<object> returnValue = new List<object>();
            DatabaseManager _dbManager = null;
            try
            {
                _dbManager = new DatabaseManager(DALConstants.CONNECTION_KEY);

                _dbManager.prepareProcedure(procedureName);
                if (param != null)
                    _dbManager._command.Parameters.Add(new SqlParameter(paramKey, param.Value));
                SqlDataReader data = _dbManager._command.ExecuteReader();
                while (data.Read())
                {
                    dynamic obj = Activator.CreateInstance(className);
                    obj.GetData(data);
                    returnValue.Add(obj);
                }
                _dbManager.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw (ex);
            }
            finally
            {
                _dbManager.Dispose();
            }
            return returnValue;
        }

        #endregion
    }
}