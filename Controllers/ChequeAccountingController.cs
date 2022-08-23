using System;
using CNF.Business.BusinessConstant;
using System.Web.Http;
using CNF.Business.Model.ChequeAccounting;
using System.Collections.Generic;
using CNF.Business.Model.Context;
using ExcelDataReader;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;

namespace CNF.API.Controllers
{
    public class ChequeAccountingController : BaseApiController
    {
        #region Add Cheque Register 
        [HttpPost]
        [Route("ChequeAccounting/ChequeRegisterAdd")]
        public string ChequeRegisterAdd([FromBody] ChequeRegisterModel model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.chequeAccountingRepository.ChequeRegisterAdd(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ChequeRegisterAdd", "Add Cheque Register Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        #region Cheque Register Edit Delete
        [HttpPost]
        [Route("ChequeAccounting/ChequeRegisterEditDelete")]
        public string ChequeRegisterEditDelete([FromBody] ChequeRegisterModel model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.chequeAccountingRepository.ChequeRegisterEditDelete(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ChequeRegisterAdd", "Add Cheque Register Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        #region Get Cheque Register List
        [HttpGet]
        [Route("ChequeAccounting/ChequeRegisterList/{BranchId}/{CompId}/{StockistId}")]
        public List<ChequeRegisterModel> ChequeRegisterList(int BranchId, int CompId, int StockistId)
        {
            List<ChequeRegisterModel> ChequeRegisterList = new List<ChequeRegisterModel>();
            try
            {
                ChequeRegisterList = _unitOfWork.chequeAccountingRepository.ChequeRegisterList(BranchId, CompId, StockistId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "Cheque Register List", "Cheque Register List " + "BranchId:  " + BranchId + "CompId:  " + CompId + "StockistId:  " + StockistId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChequeRegisterList;
        }
        #endregion

        #region Get Cheque Summary Count List
        [HttpGet]
        [Route("ChequeAccounting/ChequeSummyCount/{BranchId}/{CompId}/{StockistId}")]
        public ChequeSummyCountModel GetChequeSummyCountList(int BranchId, int CompId, int StockistId)
        {
            ChequeSummyCountModel ChequeSummyCounts = new ChequeSummyCountModel();
            try
            {
                ChequeSummyCounts = _unitOfWork.chequeAccountingRepository.ChequeSummyCountLst(BranchId, CompId, StockistId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetChequeSummyCountList", "Cheque Summary Count List " + "BranchId:  " + BranchId + "CompId:  " + CompId + ":" + StockistId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChequeSummyCounts;
        }
        #endregion

        #region Import Stockist OutStanding - BranchId, CompanyId, OSData and Addedby
        [HttpPost]
        [Route("ChequeAccounting/ImportStockistOutStanding/{BranchId}/{CompanyId}/{Addedby}")]
        public string ImportStockistOutStanding(int BranchId, int CompanyId, string Addedby)
        {
            string message = "";
            try
            {
                HttpResponseMessage result = null;
                var httpRequest = HttpContext.Current.Request;
                int InsertedCount = 0;

                if (httpRequest.Files.Count > 0)
                {
                    HttpPostedFile file = httpRequest.Files[0];
                    Stream stream = file.InputStream;
                    IExcelDataReader reader = null;

                    if (file.FileName == "ImportStockistOutStandingTemplate.xls" || file.FileName == "ImportStockistOutStandingTemplate.xlsx")
                    {
                        if (file.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (file.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            message = "This file format is not supported";
                        }

                        DataSet excelRecords = reader.AsDataSet();
                        reader.Close();

                        var finalRecords = excelRecords.Tables[0];
                        if (finalRecords.Rows.Count > 0)
                        {
                            List<ImportStockistOutStandingModel> modelList = new List<ImportStockistOutStandingModel>();

                            for (int i = 0; i < finalRecords.Rows.Count; i++)
                            {
                                ImportStockistOutStandingModel modelColumn = new ImportStockistOutStandingModel();
                                modelColumn.Div_Cd = Convert.ToString(finalRecords.Rows[i][0]);
                                modelColumn.CustomerCode = Convert.ToString(finalRecords.Rows[i][1]);
                                modelColumn.CustomerName = Convert.ToString(finalRecords.Rows[i][2]);
                                modelColumn.City = Convert.ToString(finalRecords.Rows[i][3]);
                                modelColumn.DocName = Convert.ToString(finalRecords.Rows[i][4]);
                                modelColumn.DocDate = Convert.ToString(finalRecords.Rows[i][5]);
                                modelColumn.DueDate = Convert.ToString(finalRecords.Rows[i][6]);
                                modelColumn.OpenAmt = Convert.ToString(finalRecords.Rows[i][7]);
                                modelColumn.ChqNo = Convert.ToString(finalRecords.Rows[i][8]);
                                modelColumn.DistrChannel = Convert.ToString(finalRecords.Rows[i][9]);
                                modelColumn.DocTypeDesc = Convert.ToString(finalRecords.Rows[i][10]);
                                modelColumn.DocType = Convert.ToString(finalRecords.Rows[i][11]);
                                modelColumn.OverdueAmt = Convert.ToString(finalRecords.Rows[i][12]);

                                if (modelColumn.Div_Cd == "Div_Cd" && modelColumn.CustomerCode == "CustomerCode" && modelColumn.CustomerName == "CustomerName" && modelColumn.City == "City" && modelColumn.DocName == "DocName" && modelColumn.DocDate == "DocDate" && modelColumn.DueDate == "DueDate" && modelColumn.OpenAmt == "OpenAmt" && modelColumn.ChqNo == "ChqNo" && modelColumn.DistrChannel == "DistrChannel" && modelColumn.DocTypeDesc == "DocTypeDesc" && modelColumn.DocType == "DocType" && modelColumn.OverdueAmt == "OverdueAmt")
                                {
                                    for (int j = 1; j < finalRecords.Rows.Count; j++)
                                    {
                                        ImportStockistOutStandingModel model = new ImportStockistOutStandingModel();

                                        model.pkId = j;
                                        model.Div_Cd = Convert.ToString(finalRecords.Rows[j][0]);
                                        model.CustomerCode = Convert.ToString(finalRecords.Rows[j][1]);
                                        model.CustomerName = Convert.ToString(finalRecords.Rows[j][2]);
                                        model.City = Convert.ToString(finalRecords.Rows[j][3]);
                                        model.DocName = Convert.ToString(finalRecords.Rows[j][4]);
                                        model.DocDate = Convert.ToDateTime(finalRecords.Rows[j][5]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.DueDate = Convert.ToDateTime(finalRecords.Rows[j][6]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.OpenAmt = Convert.ToString(finalRecords.Rows[j][7]);
                                        model.ChqNo = Convert.ToString(finalRecords.Rows[j][8]);
                                        model.DistrChannel = Convert.ToString(finalRecords.Rows[j][9]);
                                        model.DocTypeDesc = Convert.ToString(finalRecords.Rows[j][10]);
                                        model.DocType = Convert.ToString(finalRecords.Rows[j][11]);
                                        model.OverdueAmt = Convert.ToString(finalRecords.Rows[j][12]);
                                        modelList.Add(model);
                                    }
                                }
                            }

                            if (modelList.Count > 0)
                            {
                                // Create DataTable
                                DataTable dt = new DataTable();
                                dt.Columns.Add("pkId");
                                dt.Columns.Add("Div_Cd");
                                dt.Columns.Add("CustomerCode");
                                dt.Columns.Add("CustomerName");
                                dt.Columns.Add("City");
                                dt.Columns.Add("DocName");
                                dt.Columns.Add("DocDate");
                                dt.Columns.Add("DueDate");
                                dt.Columns.Add("OpenAmt");
                                dt.Columns.Add("ChqNo");
                                dt.Columns.Add("DistrChannel");
                                dt.Columns.Add("DocTypeDesc");
                                dt.Columns.Add("DocType");
                                dt.Columns.Add("OverdueAmt");

                                foreach (var itemList in modelList)
                                {
                                    // Add Rows DataTable
                                    dt.Rows.Add(itemList.pkId, itemList.Div_Cd, itemList.CustomerCode, itemList.CustomerName, itemList.City, itemList.DocName, itemList.DocDate, itemList.DueDate, itemList.OpenAmt, itemList.ChqNo, itemList.DistrChannel, itemList.DocTypeDesc, itemList.DocType, itemList.OverdueAmt);
                                }

                                if (dt.Rows.Count > 0)
                                {
                                    using (var db = new CFADBEntities())
                                    {
                                        SqlConnection connection = (SqlConnection)db.Database.Connection;
                                        SqlCommand cmd = new SqlCommand("CFA.usp_ImportStockistOSData", connection);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        SqlParameter BranchIdParameter = cmd.Parameters.AddWithValue("@BranchId", BranchId);
                                        BranchIdParameter.SqlDbType = SqlDbType.Int;
                                        SqlParameter CompanyIdParameter = cmd.Parameters.AddWithValue("@CompId", CompanyId);
                                        CompanyIdParameter.SqlDbType = SqlDbType.Int;
                                        SqlParameter OSDateParameter = cmd.Parameters.AddWithValue("@OSDate", DateTime.Now);
                                        OSDateParameter.SqlDbType = SqlDbType.DateTime;
                                        SqlParameter ImportSODataParameter = cmd.Parameters.AddWithValue("@OSData", dt);
                                        ImportSODataParameter.SqlDbType = SqlDbType.Structured;
                                        SqlParameter AddedbyParameter = cmd.Parameters.AddWithValue("@Addedby", Addedby);
                                        AddedbyParameter.SqlDbType = SqlDbType.NVarChar;
                                        SqlParameter RetValParameter = cmd.Parameters.AddWithValue("@RetVal", 0);
                                        RetValParameter.Direction = ParameterDirection.Output;
                                        if (connection.State == ConnectionState.Closed)
                                            connection.Open();
                                        InsertedCount = cmd.ExecuteNonQuery();
                                        if (connection.State == ConnectionState.Open)
                                            connection.Close();
                                    }

                                    if (InsertedCount > 0)
                                    {
                                        message = BusinessCont.SuccessStatus;
                                    }
                                    else
                                    {
                                        message = BusinessCont.FailStatus;
                                    }
                                }
                                else
                                {
                                    message = BusinessCont.msg_NoRecordFound;
                                }
                            }
                        }
                        else
                        {
                            message = BusinessCont.msg_NoRecordFoundExcelFile;
                        }
                    }
                    else
                    {
                        message = BusinessCont.msg_InvalidExcelFile;
                    }
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, BranchId, CompanyId, "ImportStockistOutStanding", "Import Stockist OutStanding Data:  " + Addedby, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return message;
        }
        #endregion

        #region Get Stockist OutStanding List
        [HttpGet]
        [Route("ChequeAccounting/StockistOutStandingList/{BranchId}/{CompId}")]
        public List<ImportStockistOutStandingModel> GetStockistOSList(int BranchId, int CompId)
        {
            List<ImportStockistOutStandingModel> StockistOSLst = new List<ImportStockistOutStandingModel>();
            try
            {
                StockistOSLst = _unitOfWork.chequeAccountingRepository.GeStockistOSLst(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistOSList", "Get Stockist OutStanding List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return StockistOSLst;
        }
        #endregion

        #region Get Admin Details
        [HttpGet]
        [Route("ChequeAccounting/GetAdminDetails/{EmailFor}")]
        public List<DetailsForEmail> GetAdminDetails(int EmailFor)
        {

            List<DetailsForEmail> EmailPurposes = new List<DetailsForEmail>();
            try
            {
                EmailPurposes = _unitOfWork.chequeAccountingRepository.GetAdminDetails(EmailFor);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetAdminDetails", "", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return EmailPurposes;
        }
        #endregion

        #region Get CCEmail and Purpose Details
        [HttpGet]
        [Route("ChequeAccounting/GetCCEmailandPurposeDetails")]
        public List<DetailsForEmail> GetCCEmailandPurposeDetails()
        {
            List<DetailsForEmail> EmailPurposes = new List<DetailsForEmail>();
            try
            {
                string flag = "";
                EmailPurposes = _unitOfWork.chequeAccountingRepository.GetCCEmailandPurposeDetails(flag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCCEmailandPurposeDetails", "", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return EmailPurposes;
        }
        #endregion

        #region Email Configuration Add
        [HttpPost]
        [Route("ChequeAccounting/EmailConfigurationAdd")]
        public string EmailConfigurationAdd([FromBody] EmailModel model)
        {

            string result = string.Empty;
            try
            {
                result = _unitOfWork.chequeAccountingRepository.EmailConfigurationAdd(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "EmailConfigurationAdd", "", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        #region Get Email Configuration List
        [HttpGet]
        [Route("ChequeAccounting/GetEmailConfigList/{BranchId}/{CompanyId}")]
        public List<EmailConfigModel> GetEmailConfigList(int BranchId, int CompanyId)
        {
            List<EmailConfigModel> Result = new List<EmailConfigModel>();
            try
            {
                Result = _unitOfWork.chequeAccountingRepository.GetEmailConfigList(BranchId, CompanyId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetEmailConfigList", "Get Email Configuration List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Update Cheque Status
        [HttpPost]
        [Route("ChequeAccounting/UpdateChequeStatus")]
        public string UpdateChequeStatus([FromBody] UpdateChequeSts model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.chequeAccountingRepository.UpdateChequeStatus(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "UpdateChequeStatus", "Update Cheque Status", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        #region Get Invoice For Chq Block List 
        [HttpGet]
        [Route("ChequeAccounting/GetInvoiceForChqBlockLst/{stockistId}")]
        public List<InvoiceForChqBlockModel> InvoiceForChqBlockList(int stockistId)
        {
            List<InvoiceForChqBlockModel> ChequeRegisterList = new List<InvoiceForChqBlockModel>();
            try
            {
                ChequeRegisterList = _unitOfWork.chequeAccountingRepository.InvoiceForChqBlockList(stockistId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "Cheque Register List", "Cheque Register List " + "stockistId:  " + stockistId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChequeRegisterList;
        }
        #endregion

        #region Import Deposited Cheque Receipt - BranchId, CompanyId, OSData and Addedby
        [HttpPost]
        [Route("ChequeAccounting/ImportDepositedCheque/{BranchId}/{CompanyId}/{Addedby}")]
        public string ImportDepositedCheque(int BranchId, int CompanyId, string Addedby)
        {
            string message = "";
            try
            {
                HttpResponseMessage result = null;
                var httpRequest = HttpContext.Current.Request;
                int InsertedCount = 0;

                if (httpRequest.Files.Count > 0)
                {
                    HttpPostedFile file = httpRequest.Files[0];
                    Stream stream = file.InputStream;
                    IExcelDataReader reader = null;

                    if (file.FileName == "ImportDepositedChequeDataTemplate.xls" || file.FileName == "ImportDepositedChequeDataTemplate.xlsx")
                    {
                        if (file.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (file.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            message = "This file format is not supported";
                        }

                        DataSet excelRecords = reader.AsDataSet();
                        reader.Close();

                        var finalRecords = excelRecords.Tables[0];
                        if (finalRecords.Rows.Count > 0)
                        {
                            List<ImportDepositedChequeModel> modelList = new List<ImportDepositedChequeModel>();

                            for (int i = 0; i < finalRecords.Rows.Count; i++)
                            {
                                ImportDepositedChequeModel modelColumn = new ImportDepositedChequeModel();
                                modelColumn.StockistName = Convert.ToString(finalRecords.Rows[i][0]);
                                modelColumn.StockistCode = Convert.ToString(finalRecords.Rows[i][1]);
                                modelColumn.DepositeDate = Convert.ToString(finalRecords.Rows[i][2]);
                                modelColumn.BankName = Convert.ToString(finalRecords.Rows[i][3]);
                                modelColumn.AccountNo = Convert.ToString(finalRecords.Rows[i][4]);
                                modelColumn.ChequeNo = Convert.ToString(finalRecords.Rows[i][5]);
                                modelColumn.Amount = Convert.ToString(finalRecords.Rows[i][6]);

                                if (modelColumn.StockistName == "StockistName" && modelColumn.StockistCode == "StockistCode" && modelColumn.DepositeDate == "DepositeDate" && modelColumn.BankName == "BankName" && modelColumn.AccountNo == "AccountNo" && modelColumn.ChequeNo == "ChequeNo" && modelColumn.Amount == "Amount")
                                {
                                    for (int j = 1; j < finalRecords.Rows.Count; j++)
                                    {
                                        ImportDepositedChequeModel model = new ImportDepositedChequeModel();
                                        model.pkId = j;
                                        model.StockistName = Convert.ToString(finalRecords.Rows[j][0]);
                                        model.StockistCode = Convert.ToString(finalRecords.Rows[j][1]);
                                        model.DepositeDate = Convert.ToDateTime(finalRecords.Rows[j][2]).ToString("yyyy/MM/dd hh:mm:ss"); ;
                                        model.BankName = Convert.ToString(finalRecords.Rows[j][3]);
                                        model.AccountNo = Convert.ToString(finalRecords.Rows[j][4]);
                                        model.ChequeNo = Convert.ToString(finalRecords.Rows[j][5]);
                                        model.Amount = Convert.ToString(finalRecords.Rows[j][6]);
                                        modelList.Add(model);
                                    }
                                }
                            }

                            if (modelList.Count > 0)
                            {
                                // Create DataTable
                                DataTable dt = new DataTable();
                                dt.Columns.Add("pkId");
                                dt.Columns.Add("StockistName");
                                dt.Columns.Add("StockistCode");
                                dt.Columns.Add("DepositeDate");
                                dt.Columns.Add("BankName");
                                dt.Columns.Add("AccountNo");
                                dt.Columns.Add("ChequeNo");
                                dt.Columns.Add("Amount");

                                foreach (var itemList in modelList)
                                {
                                    // Add Rows DataTable
                                    dt.Rows.Add(itemList.pkId, itemList.StockistName, itemList.StockistCode, itemList.DepositeDate, itemList.BankName, itemList.AccountNo, itemList.ChequeNo, itemList.Amount);
                                }

                                if (dt.Rows.Count > 0)
                                {
                                    using (var db = new CFADBEntities())
                                    {
                                        SqlConnection connection = (SqlConnection)db.Database.Connection;
                                        SqlCommand cmd = new SqlCommand("CFA.usp_ImportChqDepoReceiptData", connection);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        SqlParameter BranchIdParameter = cmd.Parameters.AddWithValue("@BranchId", BranchId);
                                        BranchIdParameter.SqlDbType = SqlDbType.Int;
                                        SqlParameter CompanyIdParameter = cmd.Parameters.AddWithValue("@CompId", CompanyId);
                                        CompanyIdParameter.SqlDbType = SqlDbType.Int;
                                        SqlParameter DepoDateParameter = cmd.Parameters.AddWithValue("@DepoDate", DateTime.Now);
                                        DepoDateParameter.SqlDbType = SqlDbType.DateTime;
                                        SqlParameter ImportDepoDataParameter = cmd.Parameters.AddWithValue("@DepoData", dt);
                                        ImportDepoDataParameter.SqlDbType = SqlDbType.Structured;
                                        SqlParameter AddedbyParameter = cmd.Parameters.AddWithValue("@Addedby", Addedby);
                                        AddedbyParameter.SqlDbType = SqlDbType.NVarChar;
                                        SqlParameter RetValParameter = cmd.Parameters.AddWithValue("@RetVal", 0);
                                        RetValParameter.Direction = ParameterDirection.Output;
                                        if (connection.State == ConnectionState.Closed)
                                            connection.Open();
                                        InsertedCount = cmd.ExecuteNonQuery();
                                        if (connection.State == ConnectionState.Open)
                                            connection.Close();
                                    }

                                    if (InsertedCount > 0)
                                    {
                                        message = BusinessCont.SuccessStatus;
                                    }
                                    else if (InsertedCount < 0)
                                    {
                                        message = BusinessCont.msg_exist;
                                    }
                                    else
                                    {
                                        message = BusinessCont.FailStatus;
                                    }
                                }
                                else
                                {
                                    message = BusinessCont.msg_NoRecordFound;
                                }
                            }
                        }
                        else
                        {
                            message = BusinessCont.msg_NoRecordFoundExcelFile;
                        }
                    }
                    else
                    {
                        message = BusinessCont.msg_InvalidExcelFile;
                    }
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, BranchId, CompanyId, "ImportDepositedCheque", "Import Deposited Cheque:  " + Addedby, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return message;
        }
        #endregion

        #region Get Import Deposited Cheque Receipt List
        [HttpGet]
        [Route("ChequeAccounting/DepoChequeReceiptList/{BranchId}/{CompId}")]
        public List<ImportDepositedChequeModel> GetChequeReceiptList(int BranchId, int CompId)
        {
            List<ImportDepositedChequeModel> ChequeReceiptLst = new List<ImportDepositedChequeModel>();
            try
            {
                ChequeReceiptLst = _unitOfWork.chequeAccountingRepository.GetChequeReceiptLst(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetChequeReceiptList", "Get Import Deposited Cheque Receipt List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChequeReceiptLst;
        }
        #endregion

        #region Get Cheque Register Summary Reports List
        [HttpGet]
        [Route("ChequeAccounting/RpchequeSmmryRepoList/{BranchId}/{CompId}")]
        public List<ChequeRegstrSmmryRptModel> ChequeRegisterList(int BranchId, int CompId)
        {
            List<ChequeRegstrSmmryRptModel> ChequeRegisterLst = new List<ChequeRegstrSmmryRptModel>();
            try
            {
                ChequeRegisterLst = _unitOfWork.chequeAccountingRepository.ChequeRegisterLst(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ChequeRegisterList", "Get Cheque Register Summary Reports List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChequeRegisterLst;
        }
        #endregion

        #region Send Email For Outstanding Update Alert
        [HttpGet]
        [Route("ChequeAccounting/GetStkOutstandingDtlsForEmail/{BranchId}/{CompId}")]
        public List<OutStandingDtls> GetStkOutstandingDtlsForEmail(int BranchId, int CompId)
        {
            string emailSent = string.Empty;
            List<OutStandingDtls> EmailDtls = new List<OutStandingDtls>();
            try
            {
                EmailDtls = _unitOfWork.chequeAccountingRepository.GetStkOutstandingDtlsForEmail(BranchId, CompId);
                if (EmailDtls.Count > 0)
                {
                    foreach (var i in EmailDtls)
                    {
                        emailSent = _unitOfWork.chequeAccountingRepository.sendEmailForOutstanding(i.Emailid, i.BranchId, i.CompId,i.TotOverdueAmt,i.StockistName,i.OSDate);
                    }
                }
                else
                {
                    BusinessCont.SaveLog(0, 0, 0, "EmailDtls", "No Records Found", "", "");
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStkOutstandingDtlsForEmail", "Get StkOutstanding Dtls For Email " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return EmailDtls;
        }
        #endregion

        #region Get Stockist OS Doc Types Reports List
        [HttpGet]
        [Route("ChequeAccounting/OsdocTypesReport/{BranchId}/{CompId}")]
        public List<StockistOsReportModel> OsdocTypesReportList(int BranchId, int CompId)
        {
            List<StockistOsReportModel> OsdocTypesReportLst = new List<StockistOsReportModel>();
            try
            {
                OsdocTypesReportLst = _unitOfWork.chequeAccountingRepository.OsdocTypesReportLst(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "OsdocTypesReportList", "Get Stockist OS Doc Types Reports List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return OsdocTypesReportLst;
        }
        #endregion

        #region Get Cheque Summary of previous month/Week
        [HttpPost]
        [Route("ChequeAccounting/GetChqSummaryForMonthlyList")]
        public List<ChqSummaryForMonthlyModel> GetChqSummaryForMonthlyList([FromBody] ChqSummaryForMonthly model)
        {
            List<ChqSummaryForMonthlyModel> ChqSummaryForMonthlyList = new List<ChqSummaryForMonthlyModel>();
            try
            {
                ChqSummaryForMonthlyList = _unitOfWork.chequeAccountingRepository.GetChqSummaryForMonthlyList(model.CompId, model.BranchId, model.FromDate, model.ToDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetChqSummaryForMonthlyList", "Get Cheque Summary of previous month/Week List " + "BranchId:  " + model.BranchId + "CompId:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ChqSummaryForMonthlyList;
        }
        #endregion

    }
}
