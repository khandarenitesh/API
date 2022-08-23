using CNF.Business.BusinessConstant;
using CNF.Business.Model.ChequeAccounting;
using CNF.Business.Model.Context;
using CNF.Business.Model.InventoryInward;
using CNF.Business.Repositories;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CNF.API.Controllers
{
    public class InventoryInwardController : BaseApiController
    {
        #region InsuranceClaim Add Edit
        [HttpPost]
        [Route("InventoryInward/InsuranceClaimAddEdit")]
        public int InsuranceClaimAddEdit([FromBody] InsuranceClaimModel model)
        {
            int RetValue = 0;
            try
            {
                RetValue = _unitOfWork.inventoryInwardRepository.InsuranceClaimAddEdit(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "InsuranceClaimAddEdit", "InsuranceClaim AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Insurance Claim List
        [HttpGet]
        [Route("InventoryInward/InsuranceClaimList/{BranchId}/{CompId}")]
        public List<InsuranceClaimModel> InsuranceClaimList(int BranchId, int CompId)
        {
            List<InsuranceClaimModel> InsClaimList = new List<InsuranceClaimModel>();
            try
            {
                InsClaimList = _unitOfWork.inventoryInwardRepository.GetInsuranceClaimList(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInsuranceClaimList", "Insurance Claim List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return InsClaimList;
        }
        #endregion   

        #region Get Invoice List
        [HttpGet]
        [Route("InventoryInward/InsuranceClaimInvoiceList/{BranchId}/{CompId}")]
        public List<InvoiceListModel> GetInvoiceList(int BranchId, int CompId)
        {
            List<InvoiceListModel> getInvoicelist = new List<InvoiceListModel>();
            try
            {
                getInvoicelist = _unitOfWork.inventoryInwardRepository.GetInvoiceList(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetBranchList", "Get Invoice List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return getInvoicelist;
        }
        #endregion

        #region Get Claim Types List
        [HttpGet]
        [Route("InventoryInward/InsuranceClaimTypeList/{BranchId}/{CompanyId}")]
        public List<InsuranceClaimModel> GetClaimTypeList(int BranchId, int CompanyId)
        {
            List<InsuranceClaimModel> GetClaimTypeList = new List<InsuranceClaimModel>();
            try
            {
                GetClaimTypeList = _unitOfWork.inventoryInwardRepository.GetClaimTypeList(BranchId, CompanyId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetClaimTypeList", "Get Claim Type List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return GetClaimTypeList;
        }
        #endregion

        #region Get Insurance Claim Invoice By Id List
        [HttpGet]
        [Route("InventoryInward/GetInsuranceClaimInvById/{BranchId}/{CompanyId}/{InvoiceId}")]
        public List<InsuranceClaimModel> GetInsuranceClaimInvById(int BranchId, int CompanyId,string InvoiceId)
        {
            List<InsuranceClaimModel> GetClaimInvByIdTypeList = new List<InsuranceClaimModel>();
            try
            {
                GetClaimInvByIdTypeList = _unitOfWork.inventoryInwardRepository.GetInsuranceClaimInvByIdList(BranchId, CompanyId, InvoiceId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInsuranceClaimInvById", "Get Insurance Claim Inv By Id", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return GetClaimInvByIdTypeList;
        }
        #endregion

        #region Send Email For approval Update Alert
        [HttpGet]
        [Route("InventoryInward/GetInsuranceClmByIdForEmail/{BranchId}/{CompId}/{ClaimId}")]
        public string GetInsuranceClmByIdForEmail(int BranchId, int CompId, int ClaimId)
        {
            bool IsEmailSend = false;
            EmailNotification emailNotification = new EmailNotification();
            string MailFilePath = string.Empty, EmailCC = string.Empty,
            Subject = string.Empty, Date = string.Empty, CCEmail = string.Empty, isEmailFlag = string.Empty;
            string msgHTMLOutput = string.Empty;
            Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
            Subject = ConfigurationManager.AppSettings["InsuranceClaimAppr"] + Date + " ";
            List<InsuranceClaimModel> EmailDtls = new List<InsuranceClaimModel>();
            List<CCEmailDtls> CCEmailList = new List<CCEmailDtls>();
            try
            {                
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\Approval_Update.html");
                // create table for claimid wise (ClaimNo,ClaimDate and ClaimAmt) against claimid records
                EmailDtls = _unitOfWork.inventoryInwardRepository.GetInsuranceClmDtlsForEmail(BranchId, CompId, ClaimId);
                msgHTMLOutput = _unitOfWork.inventoryInwardRepository.InsuranceClaimforApproval(EmailDtls, MailFilePath);

                using (CFADBEntities contextManager = new CFADBEntities())
                {
                    CCEmailList = _unitOfWork.chequeAccountingRepository.GetCCEmailDtlsPvt(1, 1, 8);
                    if (CCEmailList.Count > 0)
                    {
                        for (int i = 0; i < CCEmailList.Count; i++)
                        {
                            CCEmail += ";" + CCEmailList[i].Email;
                        }
                        EmailCC = CCEmail.TrimStart(';');
                    }

                    if (EmailDtls.Count > 0)
                    {
                        foreach (var item in EmailDtls)
                        {
                            if(item.IsEmail == false) // email send process only not sent
                            {
                                    IsEmailSend = emailNotification.sendEmailForApproval(item.EmailId, EmailCC, Subject, msgHTMLOutput);
                            }                           
                        }

                        if (IsEmailSend == true) //email sent
                        {
                            isEmailFlag = _unitOfWork.inventoryInwardRepository.UpdateMailForApproval(BranchId, CompId, ClaimId, IsEmailSend);
                        }
                    }
                    else
                    {
                        BusinessCont.SaveLog(0, 0, 0, "EmailDtls", "No Records Found", "", "");
                    }
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInsuranceClmDtlsForEmail", "Get Insurance Clm Dtls For Email " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return isEmailFlag;
        }
        #endregion

        #region Add/Edit Map Inward Vehicle for Mobile
        [HttpPost]
        [Route("InventoryInward/MapInwardVehicleAddEditForMob")]
        public int MapInwardVehicleAddEditForMob([FromBody] MapInwardVehicleForMobModel model)
        {
            int RetValue = 0;
            try
            {
                RetValue = _unitOfWork.inventoryInwardRepository.MapInwardVehicleAddEditForMob(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "MapInwardVehicleAddEditForMob", "Map Inward Vehicle Add Edit For Mob", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Get Maping Inward Vehicle List For Mob
        [HttpGet]
        [Route("InventoryInward/GetMapInwardVehicleListForMob/{BranchId}/{CompId}")]
        public List<MapInwardVehicleForMobModel> GetMapInwardVehicleListForMob(int BranchId, int CompId)
        {
            List<MapInwardVehicleForMobModel> InwVehicleList = new List<MapInwardVehicleForMobModel>();
            try
            {
                InwVehicleList = _unitOfWork.inventoryInwardRepository.GetMapInwardVehicleListForMobs(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetMapInwardVehicleListForMob", "Get Map Inward Vehicle List For Mob", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return InwVehicleList;
        }
        #endregion

        #region Inv In Vehicle Checklist Add Edit For Mob
        [HttpPost]
        [Route("InventoryInward/InvInVehicleChecklistAddEditForMob")]
        public int VehicleCheckListAddEdit([FromBody] VehicleChecklistModel model)
        {
            int RetValue = 0;
            try
            {
                RetValue = _unitOfWork.inventoryInwardRepository.VehicleCheckListAddEdits(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "VehicleCheckListAddEdit", "Vehicle CheckList AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion
        
        #region Get Invoice In Vehicle Check List For Mob
        [HttpGet]
        [Route("InventoryInward/GetInvInVehicleCheckList/{BranchId}/{CompId}")]
        public List<InvInVehicleCheckListmodel> GetInvInVehicleCheckList(int BranchId, int CompId)
        {
            List<InvInVehicleCheckListmodel> VehicleCheckLst = new List<InvInVehicleCheckListmodel>();
            try
            {
                VehicleCheckLst = _unitOfWork.inventoryInwardRepository.GetInvInVehicleCheckList(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvInVehicleCheckList", " Get Invoice In Vehicle Check List" + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return VehicleCheckLst;
        }
        #endregion

        #region Update InvInVerify Approve Vehicle Issue
        [HttpPost]
        [Route("InventoryInward/UpdateVerifyApproveVehicleIssue")]
        public int UpdateVerifyApproveVehicleIssue([FromBody] VerifyCheckListApproveModel model)
        {
            int RetValue = 0;
            try
            {
                RetValue = _unitOfWork.inventoryInwardRepository.UpdateVerifyApproveVehicleIssues(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "UpdateVerifyApproveVehicleIssue", "UpdateVerify ApproveVehicleIssue", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion


        #region Import Transit Data - BranchId, CompanyId and Addedby
        [HttpPost]
        [Route("InventoryInward/ImportTransitData/{BranchId}/{CompanyId}/{Addedby}")]
        public string ImportTransitData(int BranchId, int CompanyId, string Addedby)
        {
            ImportReturnModel message = new ImportReturnModel();
            try
            {
                HttpResponseMessage result = null;
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    HttpPostedFile file = httpRequest.Files[0];
                    Stream stream = file.InputStream;
                    IExcelDataReader reader = null;

                    if (file.FileName == "ImportTransitReportTemplate.xls" || file.FileName == "ImportTransitReportTemplate.xlsx")
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
                            message.RetResult = "This file format is not supported";
                        }

                        DataSet excelRecords = reader.AsDataSet();
                        reader.Close();

                        var finalRecords = excelRecords.Tables[0];
                        if (finalRecords.Rows.Count > 0)
                        {
                            List<ImportTransitDataModel> modelList = new List<ImportTransitDataModel>();
                            // colums - 0
                            for (int i = 0; i < finalRecords.Rows.Count; i++)
                            {
                                ImportTransitDataModel modelColumn = new ImportTransitDataModel();
                                modelColumn.DeliveryNo = Convert.ToString(finalRecords.Rows[i][0]);
                                string ActualGIDate = Convert.ToString(finalRecords.Rows[i][1]);
                                modelColumn.RecPlant = Convert.ToString(finalRecords.Rows[i][2]);
                                modelColumn.RecPlantDesc = Convert.ToString(finalRecords.Rows[i][3]);
                                modelColumn.DispPlant = Convert.ToString(finalRecords.Rows[i][4]);
                                modelColumn.DispPlantDesc = Convert.ToString(finalRecords.Rows[i][5]);
                                modelColumn.InvoiceNo = Convert.ToString(finalRecords.Rows[i][6]);
                                string InvoiceDate = Convert.ToString(finalRecords.Rows[i][7]);
                                modelColumn.MaterialNo = Convert.ToString(finalRecords.Rows[i][8]);
                                modelColumn.MatDesc = Convert.ToString(finalRecords.Rows[i][9]);
                                modelColumn.UoM = Convert.ToString(finalRecords.Rows[i][10]);
                                modelColumn.BatchNo = Convert.ToString(finalRecords.Rows[i][11]);
                                modelColumn.Quantity = Convert.ToString(finalRecords.Rows[i][12]);
                                modelColumn.TransporterCode = Convert.ToString(finalRecords.Rows[i][13]);
                                modelColumn.TransporterName = Convert.ToString(finalRecords.Rows[i][14]);
                                modelColumn.LrNo = Convert.ToString(finalRecords.Rows[i][15]);
                                string LrDate = Convert.ToString(finalRecords.Rows[i][16]);
                                modelColumn.TotalCaseQty = Convert.ToString(finalRecords.Rows[i][17]);
                                modelColumn.VehicleNo = Convert.ToString(finalRecords.Rows[i][18]);

                                if (modelColumn.DeliveryNo == "DeliveryNo" && ActualGIDate == "ActualGIDate" && modelColumn.RecPlant == "RecPlant"
                                    && modelColumn.RecPlantDesc == "RecPlantDesc" && modelColumn.DispPlant == "DispPlant" && modelColumn.DispPlantDesc == "DispPlantDesc"
                                    && modelColumn.InvoiceNo == "InvoiceNo" && InvoiceDate == "InvoiceDate"
                                    && modelColumn.MaterialNo == "MaterialNo" && modelColumn.MatDesc == "MatDesc" && modelColumn.UoM == "UoM"
                                    && modelColumn.BatchNo == "BatchNo" && modelColumn.Quantity == "Quantity" && modelColumn.TransporterCode == "TransporterCode"
                                    && modelColumn.TransporterName == "TransporterName" && modelColumn.LrNo == "LrNo" && LrDate == "LrDate"
                                    && modelColumn.TotalCaseQty == "TotalCaseQty" && modelColumn.VehicleNo == "VehicleNo")
                                {
                                    // row wise data = 1
                                    for (int j = 1; j < finalRecords.Rows.Count; j++)
                                    {
                                        ImportTransitDataModel model = new ImportTransitDataModel();
                                        model.pkId = j;
                                        model.DeliveryNo = Convert.ToString(finalRecords.Rows[j][0]);
                                        model.ActualGIDate = Convert.ToDateTime(finalRecords.Rows[j][1]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.RecPlant = Convert.ToString(finalRecords.Rows[j][2]);
                                        model.RecPlantDesc = Convert.ToString(finalRecords.Rows[j][3]);
                                        model.DispPlant = Convert.ToString(finalRecords.Rows[j][4]);
                                        model.DispPlantDesc = Convert.ToString(finalRecords.Rows[j][5]);
                                        model.InvoiceNo = Convert.ToString(finalRecords.Rows[j][6]);
                                        model.InvoiceDate = Convert.ToDateTime(finalRecords.Rows[j][7]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.MaterialNo = Convert.ToString(finalRecords.Rows[j][8]);
                                        model.MatDesc = Convert.ToString(finalRecords.Rows[j][9]);
                                        model.UoM = Convert.ToString(finalRecords.Rows[j][10]);
                                        model.BatchNo = Convert.ToString(finalRecords.Rows[j][11]);
                                        model.Quantity = Convert.ToString(finalRecords.Rows[j][12]);
                                        model.TransporterCode = Convert.ToString(finalRecords.Rows[j][13]);
                                        model.TransporterName = Convert.ToString(finalRecords.Rows[j][14]);
                                        model.LrNo = Convert.ToString(finalRecords.Rows[j][15]);
                                        model.LrDate = Convert.ToDateTime(finalRecords.Rows[j][16]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.TotalCaseQty = Convert.ToString(finalRecords.Rows[j][17]);
                                        model.VehicleNo = Convert.ToString(finalRecords.Rows[j][18]);
                                        modelList.Add(model);
                                    }
                                }
                            }
                            if (modelList.Count > 0)
                            {
                                // Create DataTable headers
                                DataTable dt = new DataTable();
                                dt.Columns.Add("pkId");
                                dt.Columns.Add("DeliveryNo");
                                dt.Columns.Add("ActualGIDate");
                                dt.Columns.Add("RecPlant");
                                dt.Columns.Add("RecPlantDesc");
                                dt.Columns.Add("DispPlant");
                                dt.Columns.Add("DispPlantDesc");
                                dt.Columns.Add("InvoiceNo");
                                dt.Columns.Add("InvoiceDate");
                                dt.Columns.Add("MaterialNo");
                                dt.Columns.Add("MatDesc");
                                dt.Columns.Add("UoM");
                                dt.Columns.Add("BatchNo");
                                dt.Columns.Add("Quantity");
                                dt.Columns.Add("TransporterCode");
                                dt.Columns.Add("TransporterName");
                                dt.Columns.Add("LrNo");
                                dt.Columns.Add("LrDate");
                                dt.Columns.Add("TotalCaseQty");
                                dt.Columns.Add("VehicleNo");

                                foreach (var itemList in modelList)
                                {
                                    // Add Rows DataTable
                                    dt.Rows.Add(itemList.pkId, itemList.DeliveryNo, itemList.ActualGIDate, itemList.RecPlant, itemList.RecPlantDesc,
                                       itemList.DispPlant, itemList.DispPlantDesc, itemList.InvoiceNo, itemList.InvoiceDate, itemList.MaterialNo,
                                       itemList.MatDesc, itemList.UoM, itemList.BatchNo, itemList.Quantity, itemList.TransporterCode, itemList.TransporterName, itemList.LrNo,
                                       itemList.LrDate, itemList.TotalCaseQty, itemList.VehicleNo);
                                }
                                if (dt.Rows.Count > 0)
                                {
                                    using (var db = new CFADBEntities())
                                    {
                                        {
                                            SqlConnection connection = (SqlConnection)db.Database.Connection;
                                            SqlCommand cmd = new SqlCommand("CFA.usp_ImportTransitData", connection);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            SqlParameter BranchIdParameter = cmd.Parameters.AddWithValue("@BranchId", BranchId);
                                            BranchIdParameter.SqlDbType = SqlDbType.Int;
                                            SqlParameter CompanyIdParameter = cmd.Parameters.AddWithValue("@CompId", CompanyId);
                                            CompanyIdParameter.SqlDbType = SqlDbType.Int;
                                            SqlParameter ImportTransDataParameter = cmd.Parameters.AddWithValue("@ImportTransitData", dt);
                                            ImportTransDataParameter.SqlDbType = SqlDbType.Structured;
                                            SqlParameter AddedbyParameter = cmd.Parameters.AddWithValue("@Addedby", Addedby);
                                            AddedbyParameter.SqlDbType = SqlDbType.NVarChar;
                                            if (connection.State == ConnectionState.Closed)
                                                connection.Open();
                                            SqlDataReader dr = cmd.ExecuteReader();
                                            while (dr.Read())
                                            {
                                                message.RetResult = (string)dr["RetResult"];
                                            }
                                            if (connection.State == ConnectionState.Open)
                                                connection.Close();
                                        }
                                    }
                                }
                                else
                                {
                                    message.RetResult = BusinessCont.msg_NoRecordFound;
                                }

                            }
                        }
                        else
                        {
                            message.RetResult = BusinessCont.msg_NoRecordFoundExcelFile;
                        }
                    }

                    else
                    {
                        message.RetResult = BusinessCont.msg_InvalidExcelFile;
                    }
                }
                else
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, BranchId, CompanyId, "ImportTransitData", "Import Transit Upload Data:  " + Addedby, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return message.RetResult;

        }
        #endregion

        #region Get Transit Data List
        [HttpGet]
        [Route("InventoryInward/GetTransitDataList/{BranchId}/{CompId}")]
        public List<ImportTransitListModel> GetTransitDataList(int BranchId, int CompId)
        {
            List<ImportTransitListModel> TransitLst = new List<ImportTransitListModel>();
            try
            {
                TransitLst = _unitOfWork.inventoryInwardRepository.GetTransitDataLst(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetTransitDataList", "Get Transit Data List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return TransitLst;
        }
        #endregion

        #region Invoice Inward Raise Request By Id For Mobile
        [HttpPost]
        [Route("InventoryInward/updateInvInwardRaiseRequestById")]
        public string updateInvInwardRaiseRequestById([FromBody] InvInwardRaiseRequestByIdForModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.inventoryInwardRepository.UpdateInvInwardRaiseRequestById(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "updateInvInwardRaiseRequestById", "Invoice Inward Raise Request By Id", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region  Add/Edit and Delete LR Details
        [HttpPost]
        [Route("InventoryInward/AddEditDeleteLrDetails")]
        public string AddEditDeleteLrDetails([FromBody] LRDetailsModel model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.inventoryInwardRepository.AddEditDeleteLrDetails(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditDeleteLrDetails", "Add Edit Delete Lr Deatils", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }

        #endregion

        #region Get LR List Details
        [HttpGet]
        [Route("InventoryInward/GetLRDetailsList/{BranchId}/{CompId}")]
        public List<LRDetailsModel> GetLRDetailsList(int BranchId, int CompId)
        {
            List<LRDetailsModel> LRLst = new List<LRDetailsModel>();
            try
            {
                LRLst = _unitOfWork.inventoryInwardRepository.GetLRDetailsList(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetLRDetailsList", "Get LR Deatils List " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return LRLst;
        }
        #endregion       

        #region  Get Invoice In Vehicle Check List Master
        [HttpGet]
        [Route("InventoryInward/GetInvInVehicleCheckListMaster/{BranchId}/{CompId}/{ChecklistType}")]
        [AllowAnonymous]
        public List<InvInVehicleChecklistMaster> GetInvInVehicleCheckListMaster(int BranchId, int CompId, string ChecklistType)

        {
            List<InvInVehicleChecklistMaster> InvInVehicleCheckList = new List<InvInVehicleChecklistMaster>();
            try
            {
                InvInVehicleCheckList = _unitOfWork.inventoryInwardRepository.GetInvInVehicleCheckListMaster(BranchId, CompId, ChecklistType);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvInVehicleCheckListMaster", "Get Invoice In Vehicle Check List Master" + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvInVehicleCheckList;
        }
        #endregion

    }
}
