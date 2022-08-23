using CNF.Business.BusinessConstant;
using CNF.Business.Model.Context;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static CNF.Business.Model.OrderReturn.OrderReturn;

namespace CNF.API.Controllers
{
    public class OrderReturnController : BaseApiController
    {
        #region Get New Generated Gatepass No
        [HttpPost]
        [Route("OrderReturn/GetNewGeneratedGatepassNo")]
        public string GetNewGeneratedGatepassNo(InwardGatepassModel model)
        {
            string gatepassNo = string.Empty;

            try
            {
                gatepassNo = _unitOfWork.OrderReturnRepository.GetNewGeneratedGatepassNo(model.BranchId, model.CompId, model.ReceiptDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickListGenerateNewNo", "Get New Generated Gatepass No " + "BranchId:  " + model.BranchId + "CompId:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return gatepassNo;
        }
        #endregion

        #region Generate Inward Gatepass Add Edit - for Mobile
        [HttpPost]
        [Route("OrderReturn/AddEditInwardGatepass")]
        public string AddEditInwardGatepass([FromBody] InwardGatepassModel model)
        {
            string result = string.Empty, emailDtls = string.Empty;
            List<StokistDtlsModel> InwardGatepassDtls = new List<StokistDtlsModel>();
            try
            {
                result = _unitOfWork.OrderReturnRepository.AddEditInwardGatepass(model);
                if (model.Action == "ADD")
                {
                    if (result != "")
                    {
                        InwardGatepassDtls = _unitOfWork.OrderReturnRepository.GetStockistDtlsForEmail(model.BranchId, model.CompId, Convert.ToInt32(result));
                        if (InwardGatepassDtls.Count > 0)
                        {
                            foreach (var i in InwardGatepassDtls)
                            {
                                BusinessCont.SaveLog(0, 0, 0, "SendEmailForConsignmentRecieved", " ", "START", "");
                                emailDtls = _unitOfWork.OrderReturnRepository.SendEmailForConsignmentReceived(i.Emailid, i.StockistName, i.LRNumber, i.LRDate, i.ReceiptDate, model.BranchId, model.CompId);
                                BusinessCont.SaveLog(0, 0, 0, "SendEmailForConsignmentRecieved", " ", "END", "");
                            }
                        }
                        else
                        {
                            BusinessCont.SaveLog(0, 0, 0, "InwardGatepassDtls", "No Records Found", "", "");
                        }
                    }
                }
                else
                {
                    BusinessCont.SaveLog(0, 0, 0, "InwardGatepassDtls", "result;  " + result, "", "");
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditInwardGatepass", "Add Edit Inward Gatepass", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        #region Get Inward Gatepass List - For Mobile
        [HttpPost]
        [Route("OrderReturn/GetInwardGatepassList")]
        public List<InwardGatepassModel> GetInwardGatepassList(InwardGatepassModel model)
        {
            List<InwardGatepassModel> GatepassLst = new List<InwardGatepassModel>();
            try
            {
                GatepassLst = _unitOfWork.OrderReturnRepository.GetInwardGatepassList(model.BranchId, model.CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInwardGatepassList", "Get Inward Gatepass List ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return GatepassLst;
        }
        #endregion

        #region Get Missing Claim Form List
        [HttpPost]
        [Route("OrderReturn/GetMissingClaimFormList")]
        public List<InwardGatepassModel> GetMissingClaimFormList(InwardGatepassModel model)
        {
            List<InwardGatepassModel> MissingClaimFormLst = new List<InwardGatepassModel>();
            try
            {
                MissingClaimFormLst = _unitOfWork.OrderReturnRepository.GetMissingClaimFormList(model.BranchId, model.CompId, model.Flag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetMissingClaimFormList", "Get Missing Claim Form List ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return MissingClaimFormLst;
        }
        #endregion

        #region Send Email For Missing Claim Form
        [HttpGet]
        [Route("OrderReturn/SendEmailForMissingClaimFormAlert/{BranchId}/{CompId}/{GatepassId}/{Flag}")]
        public int SendEmailForMissingClaimForm(int BranchId, int CompId, int GatepassId, int Flag)
        {
            string emailSent = string.Empty;
            List<StokistDtlsModel> EmailDtls = new List<StokistDtlsModel>();
            int emailSend = 0;
            try
            {
                EmailDtls = _unitOfWork.OrderReturnRepository.GetStockistDtlsForEmail(BranchId, CompId, GatepassId);
                if (EmailDtls.Count > 0)
                {
                    emailSent = _unitOfWork.OrderReturnRepository.SendEmailForMissingClaimForm(EmailDtls[0].Emailid, EmailDtls[0].BranchId,
                                   EmailDtls[0].CompId, EmailDtls[0].StockistName);
                    emailSend = _unitOfWork.OrderReturnRepository.AddSendEmailFlag(BranchId, CompId, GatepassId, Flag);
                }
                else
                {
                    BusinessCont.SaveLog(0, 0, 0, "EmailDtls", "No Records Found", "", "");
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailForMissingClaimFormAlert", "Send Email For Missing Claim Form Alert " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return emailSend;
        }
        #endregion

        #region 1st Physical Check Add Edit
        [HttpPost]
        [Route("OrderReturn/PhysicalCheckAddEdit")]
        public int PhysicalCheck1AddEdit([FromBody]PhysicalCheck1 model)
        {
            int physicalcheck = 0;
            try
            {
                physicalcheck = _unitOfWork.OrderReturnRepository.PhysicalCheck1AddEdit(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PhysicalCheckAddEdit", "Physical Check Add Edit ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return physicalcheck;
        }
        #endregion

        #region Physical check 1 List
        [HttpGet]
        [Route("OrderReturn/GetPhysicalCheck1List/{BranchId}/{CompId}")]
        public List<PhysicalCheck1> GetPhysicalCheck1List(int BranchId, int CompId)
        {
            List<PhysicalCheck1> PhysicalCheck1List = new List<PhysicalCheck1>();
            try
            {
                PhysicalCheck1List = _unitOfWork.OrderReturnRepository.GetPhysicalCheck1List(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPhysicalCheck1List", "Get Physical Check1 List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return PhysicalCheck1List;
        }
        #endregion

        #region Physical check 1 List
        [HttpPost]
        [Route("OrderReturn/PhysicalCheck1Concern")]
        public int PhysicalCheck1Concern([FromBody] PhysicalCheck1 model)
        {
            int RetValue = 0;
            try
            {
                RetValue = _unitOfWork.OrderReturnRepository.PhysicalCheck1Concern(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPhysicalCheck1List", "Get Physical Check1 List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Get Auditor Check - Verify and Correction List
        [HttpGet]
        [Route("OrderReturn/GetSRSClaimListForVerifyList/{BranchId}/{CompId}")]
        public List<SRSClaimListForVerifyModel> GetSRSClaimListForVerifyList(int BranchId, int CompId)
        {
            List<SRSClaimListForVerifyModel> SRSClaimListForVerifyList = new List<SRSClaimListForVerifyModel>();
            try
            {
                SRSClaimListForVerifyList = _unitOfWork.OrderReturnRepository.GetSRSClaimListForVerifyList(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetSRSClaimListForVerifyList", "Get Auditor Check - Verify and Correction List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return SRSClaimListForVerifyList;
        }
        #endregion

        #region Auditor Check - Verify and Correction Required(Remark)
        [HttpPost]
        [Route("OrderReturn/AuditorCheckCorrection")]
        public int AuditorCheckCorrection([FromBody] AuditorCheckCorrectionModel model)
        {
            int RetValue = 0;
            try
            {
                RetValue = _unitOfWork.OrderReturnRepository.AuditorCheckCorrection(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AuditorCheckCorrection", "Auditor Check - Verify and Correction Required(Remark)", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Import CN Data - BranchId, CompanyId and AddedBy
        [HttpPost]
        [Route("OrderReturn/ImportCNData/{BranchId}/{CompanyId}/{AddedBy}")]
        public string ImportCNData(int BranchId, int CompanyId, string AddedBy)
        {
            ImportCNData message = new ImportCNData();
            try
            {
                HttpResponseMessage result = null;
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count > 0)
                {
                    HttpPostedFile file = httpRequest.Files[0];
                    Stream stream = file.InputStream;
                    IExcelDataReader reader = null;

                    if (file.FileName == "ImportCNTemplate.xls" || file.FileName == "ImportCNTemplate.xlsx")
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
                            List<ImportCNData> modelList = new List<ImportCNData>();

                            for (int i = 0; i < finalRecords.Rows.Count; i++)
                            {
                                ImportCNData modelColumn = new ImportCNData();
                                modelColumn.CompanyCode = Convert.ToString(finalRecords.Rows[i][0]);
                                modelColumn.DistChannel = Convert.ToString(finalRecords.Rows[i][1]);
                                modelColumn.Division = Convert.ToString(finalRecords.Rows[i][2]);
                                modelColumn.SalesOrderNo = Convert.ToString(finalRecords.Rows[i][3]);
                                modelColumn.SalesOrderDate = Convert.ToString(finalRecords.Rows[i][4]);
                                modelColumn.CrDrNoteNo = Convert.ToString(finalRecords.Rows[i][5]);
                                modelColumn.CRDRCreationDate = Convert.ToString(finalRecords.Rows[i][6]);
                                modelColumn.CrDrAmt = Convert.ToString(finalRecords.Rows[i][7]);
                                modelColumn.SoldToCode = Convert.ToString(finalRecords.Rows[i][8]);
                                modelColumn.SoldToName = Convert.ToString(finalRecords.Rows[i][9]);
                                modelColumn.SoldToCity = Convert.ToString(finalRecords.Rows[i][10]);
                                modelColumn.BillingTypeDescription = Convert.ToString(finalRecords.Rows[i][11]);
                                modelColumn.OrderReason = Convert.ToString(finalRecords.Rows[i][12]);
                                modelColumn.OrderReasonDescription = Convert.ToString(finalRecords.Rows[i][13]);
                                modelColumn.LRNo = Convert.ToString(finalRecords.Rows[i][14]);
                                modelColumn.LRDate = Convert.ToString(finalRecords.Rows[i][15]);
                                modelColumn.CFAGRDate = Convert.ToString(finalRecords.Rows[i][16]);
                                modelColumn.MaterialNumber = Convert.ToString(finalRecords.Rows[i][17]);
                                modelColumn.MaterialDescription = Convert.ToString(finalRecords.Rows[i][18]);
                                modelColumn.BatchNo = Convert.ToString(finalRecords.Rows[i][19]);
                                modelColumn.BillingQty = Convert.ToString(finalRecords.Rows[i][20]);
                                modelColumn.ItemCatagoryDescription = Convert.ToString(finalRecords.Rows[i][21]);

                                if (modelColumn.CompanyCode == "Company Code" && modelColumn.DistChannel == "Dist Channel" && modelColumn.Division == "Division" &&
                                    modelColumn.SalesOrderNo == "Sales Order No" && modelColumn.SalesOrderDate == "Sales Order Date" && modelColumn.CrDrNoteNo == "Cr/Dr Note No" && modelColumn.CRDRCreationDate == "CR/DR Creation Date" && modelColumn.CrDrAmt == "Cr/Dr Amt." && modelColumn.SoldToCode == "Sold To Code" && modelColumn.SoldToName == "Sold To Name" && modelColumn.SoldToCity == "Sold to City" && 
                                    modelColumn.BillingTypeDescription == "Billing Type Description" && modelColumn.OrderReason == "Order Reason" &&
                                    modelColumn.OrderReasonDescription == "Order Reason Description" && modelColumn.LRNo == "LR No." && modelColumn.LRDate == "LR Date" &&
                                    modelColumn.CFAGRDate == "CFA GR date" && modelColumn.MaterialNumber == "Material Number" && modelColumn.MaterialDescription == "Material Description" && modelColumn.BatchNo == "Batch No" && modelColumn.BillingQty == "Billing Qty" && modelColumn.ItemCatagoryDescription == "Item Catagory Description")
                                {
                                    for (int j = 1; j < finalRecords.Rows.Count; j++)
                                    {
                                        ImportCNData model = new ImportCNData();
                                        model.pkId = j;
                                        model.CompanyCode = Convert.ToString(finalRecords.Rows[j][0]);
                                        model.DistChannel = Convert.ToString(finalRecords.Rows[j][1]);
                                        model.Division = Convert.ToString(finalRecords.Rows[j][2]);
                                        model.SalesOrderNo = Convert.ToString(finalRecords.Rows[j][3]);
                                        model.SalesOrderDate = Convert.ToDateTime(finalRecords.Rows[j][4]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.CrDrNoteNo = Convert.ToString(finalRecords.Rows[j][5]);
                                        model.CRDRCreationDate = Convert.ToDateTime(finalRecords.Rows[j][6]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.CrDrAmt = Convert.ToString(finalRecords.Rows[j][7]);
                                        model.SoldToCode = Convert.ToString(finalRecords.Rows[j][8]);
                                        model.SoldToName = Convert.ToString(finalRecords.Rows[j][9]);
                                        model.SoldToCity = Convert.ToString(finalRecords.Rows[j][10]);
                                        model.BillingTypeDescription = Convert.ToString(finalRecords.Rows[j][11]);
                                        model.OrderReason = Convert.ToString(finalRecords.Rows[j][12]);
                                        model.OrderReasonDescription = Convert.ToString(finalRecords.Rows[j][13]);
                                        model.LRNo = Convert.ToString(finalRecords.Rows[j][14]);
                                        model.LRDate = Convert.ToDateTime(finalRecords.Rows[j][15]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.CFAGRDate = Convert.ToDateTime(finalRecords.Rows[j][16]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.MaterialNumber = Convert.ToString(finalRecords.Rows[j][17]);
                                        model.MaterialDescription = Convert.ToString(finalRecords.Rows[j][18]);
                                        model.BatchNo = Convert.ToString(finalRecords.Rows[j][19]);
                                        model.BillingQty = Convert.ToString(finalRecords.Rows[j][20]);
                                        model.ItemCatagoryDescription = Convert.ToString(finalRecords.Rows[j][21]);
                                        model.AddedBy = Convert.ToInt32(AddedBy);
                                        model.AddedOn = Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd hh:mm:ss");
                                        modelList.Add(model);
                                    }
                                }
                            }

                            if (modelList.Count > 0)
                            {
                                // Create DataTable
                                DataTable dt = new DataTable();
                                dt.Columns.Add("pkId");
                                dt.Columns.Add("CompanyCode");
                                dt.Columns.Add("DistChannel");
                                dt.Columns.Add("Division");
                                dt.Columns.Add("SalesOrderNo");
                                dt.Columns.Add("SalesOrderDate");
                                dt.Columns.Add("CrDrNoteNo");
                                dt.Columns.Add("CRDRCreationDate");
                                dt.Columns.Add("CrDrAmt");
                                dt.Columns.Add("SoldToCode");
                                dt.Columns.Add("SoldToName");
                                dt.Columns.Add("SoldToCity");
                                dt.Columns.Add("BillingTypeDescription");
                                dt.Columns.Add("OrderReason");
                                dt.Columns.Add("OrderReasonDescription");
                                dt.Columns.Add("LRNo");
                                dt.Columns.Add("LRDate");
                                dt.Columns.Add("CFAGRDate");
                                dt.Columns.Add("MaterialNumber");
                                dt.Columns.Add("MaterialDescription");
                                dt.Columns.Add("BatchNo");
                                dt.Columns.Add("BillingQty");
                                dt.Columns.Add("ItemCatagoryDescription");
                                dt.Columns.Add("AddedBy");
                                dt.Columns.Add("AddedOn");

                                foreach (var itemList in modelList)
                                {
                                    // Add Rows DataTable
                                    dt.Rows.Add(itemList.pkId, itemList.CompanyCode, itemList.DistChannel, itemList.Division, itemList.SalesOrderNo, itemList.SalesOrderDate, 
                                                itemList.CrDrNoteNo, itemList.CRDRCreationDate, itemList.CrDrAmt, itemList.SoldToCode, itemList.SoldToName, itemList.SoldToCity, 
                                                itemList.BillingTypeDescription, itemList.OrderReason, itemList.OrderReasonDescription, itemList.LRNo,
                                                itemList.LRDate, itemList.CFAGRDate, itemList.MaterialNumber, itemList.MaterialDescription, itemList.BatchNo,
                                                itemList.BillingQty, itemList.ItemCatagoryDescription, itemList.AddedBy, itemList.AddedOn);
                                }

                                if (dt.Rows.Count > 0)
                                {
                                    using (var db = new CFADBEntities())
                                    {
                                        {
                                            SqlConnection connection = (SqlConnection)db.Database.Connection;
                                            SqlCommand cmd = new SqlCommand("CFA.usp_ImportCNData", connection);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            SqlParameter BranchIdParameter = cmd.Parameters.AddWithValue("@BranchId", BranchId);
                                            BranchIdParameter.SqlDbType = SqlDbType.Int;
                                            SqlParameter CompanyIdParameter = cmd.Parameters.AddWithValue("@CompId", CompanyId);
                                            CompanyIdParameter.SqlDbType = SqlDbType.Int;
                                            SqlParameter ImportInvDataParameter = cmd.Parameters.AddWithValue("@ImportCNData", dt);
                                            ImportInvDataParameter.SqlDbType = SqlDbType.Structured;
                                            SqlParameter AddedbyParameter = cmd.Parameters.AddWithValue("@AddedBy", AddedBy);
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
                BusinessCont.SaveLog(0, BranchId, CompanyId, "ImportCNData", "Import CN Data - BranchId, CompanyId and AddedBy:  " + AddedBy, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return message.RetResult;
        }
        #endregion

        #region Get Import CN Data List
        [HttpGet]
        [Route("OrderReturn/GetImportCNDataList/{BranchId}/{CompId}")]
        public List<ImportCNData> GetImportCNDataList(int BranchId, int CompId)
        {
            List<ImportCNData> ImportCNDataList = new List<ImportCNData>();
            try
            {
                ImportCNDataList = _unitOfWork.OrderReturnRepository.GetImportCNDataList(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetImportCNDataList", "Get Import CN Data List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ImportCNDataList;
        }
        #endregion

        #region Get Import CN Data For Email
        [HttpGet]
        [Route("OrderReturn/GetImportCNDataForEmail/{BranchId}/{CompId}")]
        public List<CNDataForEmail> GetImportCNDataForEmail(int BranchId, int CompId)
        {
            List<CNDataForEmail> ImportCNDataList = new List<CNDataForEmail>();
            try
            {
                ImportCNDataList = _unitOfWork.OrderReturnRepository.GetImportCNDataForEmail(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetImportCNDataForEmail", "Get Import CN Data For Email", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return ImportCNDataList;
        }
        #endregion

        #region Add/Edit Claim - SRS Mapping
        [HttpPost]
        [Route("OrderReturn/ClaimSRSMappingAddEdit")]
        public int ClaimSRSMappingAddEdit([FromBody] AddClaimSRSMappingModel model)
        {
            int RetValue = 0;
            try
            {
                RetValue = _unitOfWork.OrderReturnRepository.ClaimSRSMappingAddEdits(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ClaimSRSMappingAddEdit", "Claim SRS Mapping AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Claim - SRS Mapping List
        [HttpGet]
        [Route("OrderReturn/ClaimSRSMappingList/{BranchId}/{CompId}/{PhyChkId}")]
        public List<AddClaimSRSMappingModel> ClaimSRSMappingList(int BranchId, int CompId, int PhyChkId)
        {
            List<AddClaimSRSMappingModel> ClaimSRSList = new List<AddClaimSRSMappingModel>();
            try
            {
                ClaimSRSList = _unitOfWork.OrderReturnRepository.GetClaimSRSMappingLists(BranchId, CompId, PhyChkId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ClaimSRSMappingList", "Claim SRS Mapping List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return ClaimSRSList;
        }
        #endregion

        #region Get ClaimNo List
        [HttpGet]
        [Route("OrderReturn/GetClaimNoList/{BranchId}/{CompId}")]
        public List<GetClaimNoListModel> getClaimNolist(int BranchId, int CompId)
        {
            List<GetClaimNoListModel> getClaimNolist = new List<GetClaimNoListModel>();
            try
            {
                getClaimNolist = _unitOfWork.OrderReturnRepository.GetClaimNoLists(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "getClaimNolist", "Get ClaimNo list", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return getClaimNolist;
        }
        #endregion

        #region Get SRS Claim Mapped List
        [HttpGet]
        [Route("OrderReturn/GetSRSClaimMappedList/{BranchId}/{CompId}")]
        public List<AddClaimSRSMappingModel> GetSRSClaimMappedList(int BranchId, int CompId)
        {
            List<AddClaimSRSMappingModel> Claimappedlist = new List<AddClaimSRSMappingModel>();
            try
            {
                Claimappedlist = _unitOfWork.OrderReturnRepository.GetSRSClaimMappedLists(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetSRSClaimMappedList", "Get SRS Claim Mapped List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return Claimappedlist;
        }
        #endregion

        #region Add Delay Reason Of Pending CN
        [HttpPost]
        [Route("OrderReturn/AddDelayReasonOfPendingCN")]
        public int AddDelayReasonOfPendingCN([FromBody] UpdateCNDelayReason model)
        {
            int RetValue = 0;
            try
            {
                RetValue = _unitOfWork.OrderReturnRepository.UpdateCNDelayReason(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddDelayReasonOfPendingCN", "Add Delay Reason Of Pending CN", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Get LR Mismatch List
        [HttpGet]
        [Route("OrderReturn/GetLrMisMatchList/{BranchId}/{CompId}")]
        public List<LRmisMatchModel> GetLrMisMatchList(int BranchId, int CompId)
        {
            List<LRmisMatchModel> Claimappedlist = new List<LRmisMatchModel>();
            try
            {
                Claimappedlist = _unitOfWork.OrderReturnRepository.GetLrMisMatchLists(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetLrMisMatchList", "Get Lr MisMatch List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return Claimappedlist;
        }
        #endregion

    }
}
