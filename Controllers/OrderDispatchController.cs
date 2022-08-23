using CNF.Business.BusinessConstant;
using CNF.Business.Model.Context;
using CNF.Business.Model.Login;
using CNF.Business.Model.OrderDispatch;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using QRCoder;
using System.Globalization;

namespace CNF.API.Controllers
{
    public class OrderDispatchController : BaseApiController
    {       
        #region PickListHeader list & AddEdit
        [HttpPost]
        [Route("OrderDispatch/PickListHeaderAddEdit")]
        public string PickListHeaderAddEdit([FromBody] PickListModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.PickListHeaderAddEdit(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PickListHeaderAddEdit", "Picklist AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Get Picklist By BranchId and CompanyId and PicklistDate
        [HttpPost]
        [Route("OrderDispatch/GetPickList")]
        public List<PickListModel> GetPickList(PickListModel model)
        {
            List<PickListModel> pickList = new List<PickListModel>();
            try
            {
                pickList = _unitOfWork.OrderDispatchRepository.GetPickLst(model.BranchId, model.CompId, model.PicklistDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickList", "Get Pick List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return pickList;
        }
        #endregion

        #region Get PickList Details By Id
        [HttpGet]
        [Route("OrderDispatch/GetPickListDetailsById/{Picklistid}")]
        public List<PickListDetailsByIdModel> GetPickListDetailsById(int Picklistid)
        {
            List<PickListDetailsByIdModel> pickListDetailsByIdModel = new List<PickListDetailsByIdModel>();
            try
            {
                pickListDetailsByIdModel = _unitOfWork.OrderDispatchRepository.GetPickListDetailsById(Picklistid);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickList", "Get Pick List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return pickListDetailsByIdModel;
        }
        #endregion

        #region Send Notification To Stockist
        [HttpPost]
        [Route("OrderDispatch/sendEmail")]
        public string sendEmail(UserModel model)
        {
            string emailsts = string.Empty;
            try
            {
                emailsts = _unitOfWork.OrderDispatchRepository.sendEmail(model.Email,model.PicklistNo); 
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "sendEmail", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return emailsts;
        }
        #endregion

        #region Send Notification To Picker
        [HttpPost]
        [Route("OrderDispatch/sendEmailtopicker")]
        public string sendEmailtopicker(UserModel model)
        {
            string emailsts = string.Empty;
            try
            {
                emailsts = _unitOfWork.OrderDispatchRepository.sendEmailToPicker(model.Email, model.PicklistNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "sendEmailtopicker", DateTime.Now.ToString(), BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return emailsts;
        }
        #endregion

        #region Picklist Allotment Add
        [HttpPost]
        [Route("OrderDispatch/PicklistAllotmentAdd")]
        public string PicklistAllotmentAdd([FromBody] PicklstAllotReallotModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.PicklistAllotmentAdd(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PicklistAllotmentAdd", "Picklist Allotment Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Picklist ReAllotment Add
        [HttpPost]
        [Route("OrderDispatch/PicklistReAllotmentAdd")]
        public string PicklistReAllotmentAdd([FromBody] PicklstAllotReallotModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.PicklistReAllotmentAdd(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PicklistAllotmentAdd", "Picklist ReAllotment Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Picklist Allotment Status
        [HttpPost]
        [Route("OrderDispatch/PicklistAllotmentStatus")]
        public string PicklistAllotmentStatus([FromBody] PicklistAllotmentStatusModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.PicklistAllotmentStatus(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PicklistAllotmentStatus", "Picklist Allotment Status", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Get Invoice Header List
        [HttpPost]
        [Route("OrderDispatch/GetInvoiceHeaderList")]
        public List<InvoiceLstModel> GetInvoiceHeaderList(InvoiceLstModel model)
        {
            List<InvoiceLstModel> InvoiceLst = new List<InvoiceLstModel>();
            try
            {
                InvoiceLst = _unitOfWork.OrderDispatchRepository.GetInvoiceHeaderLst(model.BranchId, model.CompId, model.FromDate, model.ToDate, model.BillDrawerId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceHeaderList", "Get Invoice Header List " + "BranchId:  " + model.BranchId + "CompId:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvoiceLst;
        }
        #endregion

        #region Import Invoice Data - BranchId, CompanyId and Addedby
        [HttpPost]
        [Route("OrderDispatch/ImportInvoiceData/{BranchId}/{CompanyId}/{Addedby}")]
        public string ImportInvoiceData(int BranchId, int CompanyId, string Addedby)
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

                    if (file.FileName == "ImportInvoiceDataTemplate.xls" || file.FileName == "ImportInvoiceDataTemplate.xlsx")
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
                            List<ImportInvoiceDataModel> modelList = new List<ImportInvoiceDataModel>();

                            for (int i = 0; i < finalRecords.Rows.Count; i++)
                            {
                                ImportInvoiceDataModel modelColumn = new ImportInvoiceDataModel();
                                modelColumn.ShipFromCode = Convert.ToString(finalRecords.Rows[i][0]);
                                modelColumn.CompanyCode = Convert.ToString(finalRecords.Rows[i][1]);
                                modelColumn.DisctChannel = Convert.ToString(finalRecords.Rows[i][2]);
                                modelColumn.Division = Convert.ToString(finalRecords.Rows[i][3]);
                                modelColumn.InvoiceNo = Convert.ToString(finalRecords.Rows[i][4]);
                                string InvoicePostDate = Convert.ToString(finalRecords.Rows[i][5]);
                                string InvoiceCreateDate = Convert.ToString(finalRecords.Rows[i][6]);
                                modelColumn.InvoiceAmount = Convert.ToString(finalRecords.Rows[i][7]);
                                modelColumn.SoldToCode = Convert.ToString(finalRecords.Rows[i][8]);
                                modelColumn.SoldToName = Convert.ToString(finalRecords.Rows[i][9]);
                                modelColumn.SoldToCity = Convert.ToString(finalRecords.Rows[i][10]);
                                modelColumn.BillingType = Convert.ToString(finalRecords.Rows[i][11]);
                                modelColumn.PONo = Convert.ToString(finalRecords.Rows[i][12]);
                                string PODate = Convert.ToString(finalRecords.Rows[i][13]);
                                modelColumn.MaterialNo = Convert.ToString(finalRecords.Rows[i][14]);
                                modelColumn.MatDesc = Convert.ToString(finalRecords.Rows[i][15]);
                                modelColumn.BatchNo = Convert.ToString(finalRecords.Rows[i][16]);
                                modelColumn.ItemCategory = Convert.ToString(finalRecords.Rows[i][17]);
                                modelColumn.BillingQty = Convert.ToString(finalRecords.Rows[i][18]);
                                string ExpiryDate = Convert.ToString(finalRecords.Rows[i][19]);
                                modelColumn.BasicAmt = Convert.ToString(finalRecords.Rows[i][20]);
                                modelColumn.Discount = Convert.ToString(finalRecords.Rows[i][21]);
                                modelColumn.ManualDiscount = Convert.ToString(finalRecords.Rows[i][22]);

                                if (modelColumn.ShipFromCode == "ShipFromCode" && modelColumn.CompanyCode == "CompanyCode" && modelColumn.DisctChannel == "DisctChannel"
                                    && modelColumn.Division == "Division" && modelColumn.InvoiceNo == "InvoiceNo" && InvoicePostDate == "InvoicePostDate"
                                    && InvoiceCreateDate == "InvoiceCreateDate" && modelColumn.InvoiceAmount == "InvoiceAmount"
                                    && modelColumn.SoldToCode == "SoldToCode" && modelColumn.SoldToName == "SoldToName" && modelColumn.SoldToCity == "SoldToCity"
                                    && modelColumn.BillingType == "BillingType" && modelColumn.PONo == "PONo" && PODate == "PODate"
                                    && modelColumn.MaterialNo == "MaterialNo" && modelColumn.MatDesc == "MatDesc" && modelColumn.BatchNo == "BatchNo"
                                    && modelColumn.ItemCategory == "ItemCategory" && modelColumn.BillingQty == "BillingQty" && ExpiryDate == "ExpiryDate" 
                                    && modelColumn.BasicAmt == "BasicAmt" && modelColumn.Discount == "Discount" && modelColumn.ManualDiscount == "ManualDiscount")
                                {
                                    for (int j = 1; j < finalRecords.Rows.Count; j++)
                                    {
                                        ImportInvoiceDataModel model = new ImportInvoiceDataModel();
                                        model.pkId = j;
                                        model.ShipFromCode = Convert.ToString(finalRecords.Rows[j][0]);
                                        model.CompanyCode = Convert.ToString(finalRecords.Rows[j][1]);
                                        model.DisctChannel = Convert.ToString(finalRecords.Rows[j][2]);
                                        model.Division = Convert.ToString(finalRecords.Rows[j][3]);
                                        model.InvoiceNo = Convert.ToString(finalRecords.Rows[j][4]);
                                        model.InvoicePostDate = Convert.ToDateTime(finalRecords.Rows[j][5]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.InvoiceCreateDate = Convert.ToDateTime(finalRecords.Rows[j][6]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.InvoiceAmount = Convert.ToString(finalRecords.Rows[j][7]);
                                        model.SoldToCode = Convert.ToString(finalRecords.Rows[j][8]);
                                        model.SoldToName = Convert.ToString(finalRecords.Rows[j][9]);
                                        model.SoldToCity = Convert.ToString(finalRecords.Rows[j][10]);
                                        model.BillingType = Convert.ToString(finalRecords.Rows[j][11]);
                                        model.PONo = Convert.ToString(finalRecords.Rows[j][12]);
                                        model.PODate = Convert.ToDateTime(finalRecords.Rows[j][13]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.MaterialNo = Convert.ToString(finalRecords.Rows[j][14]);
                                        model.MatDesc = Convert.ToString(finalRecords.Rows[j][15]);
                                        model.BatchNo = Convert.ToString(finalRecords.Rows[j][16]);
                                        model.ItemCategory = Convert.ToString(finalRecords.Rows[j][17]);
                                        model.BillingQty = Convert.ToString(finalRecords.Rows[j][18]);
                                        model.ExpiryDate = Convert.ToDateTime(finalRecords.Rows[j][19]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.BasicAmt = Convert.ToString(finalRecords.Rows[j][20]);
                                        model.Discount = Convert.ToString(finalRecords.Rows[j][21]);
                                        model.ManualDiscount = Convert.ToString(finalRecords.Rows[j][22]);
                                        modelList.Add(model);
                                    }
                                }
                            }

                            if (modelList.Count > 0)
                            {
                                // Create DataTable
                                DataTable dt = new DataTable();
                                dt.Columns.Add("pkId");
                                dt.Columns.Add("ShipFromCode");
                                dt.Columns.Add("CompanyCode");
                                dt.Columns.Add("DisctChannel");
                                dt.Columns.Add("Division");
                                dt.Columns.Add("InvoiceNo");
                                dt.Columns.Add("InvoicePostDate");
                                dt.Columns.Add("InvoiceCreateDate");
                                dt.Columns.Add("InvoiceAmount");
                                dt.Columns.Add("SoldToCode");
                                dt.Columns.Add("SoldToName");
                                dt.Columns.Add("SoldToCity");
                                dt.Columns.Add("BillingType");
                                dt.Columns.Add("PONo");
                                dt.Columns.Add("PODate");
                                dt.Columns.Add("MaterialNo");
                                dt.Columns.Add("MatDesc");
                                dt.Columns.Add("BatchNo");
                                dt.Columns.Add("ItemCategory");
                                dt.Columns.Add("BillingQty");
                                dt.Columns.Add("ExpiryDate");
                                dt.Columns.Add("BasicAmt");
                                dt.Columns.Add("Discount");
                                dt.Columns.Add("ManualDiscount");

                                foreach (var itemList in modelList)
                                {
                                    // Add Rows DataTable
                                    dt.Rows.Add(itemList.pkId, itemList.ShipFromCode, itemList.CompanyCode, itemList.DisctChannel, itemList.Division,
                                       itemList.InvoiceNo, itemList.InvoicePostDate, itemList.InvoiceCreateDate, itemList.InvoiceAmount, itemList.SoldToCode,
                                       itemList.SoldToName, itemList.SoldToCity, itemList.BillingType, itemList.PONo, itemList.PODate, itemList.MaterialNo, itemList.MatDesc,
                                       itemList.BatchNo, itemList.ItemCategory, itemList.BillingQty, itemList.ExpiryDate, itemList.BasicAmt, itemList.Discount, itemList.ManualDiscount);
                                }

                                if (dt.Rows.Count > 0)
                                {
                                    using (var db = new CFADBEntities())
                                    {
                                        {
                                            SqlConnection connection = (SqlConnection)db.Database.Connection;
                                            SqlCommand cmd = new SqlCommand("CFA.usp_ImportInvoiceData", connection);
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            SqlParameter BranchIdParameter = cmd.Parameters.AddWithValue("@BranchId", BranchId);
                                            BranchIdParameter.SqlDbType = SqlDbType.Int;
                                            SqlParameter CompanyIdParameter = cmd.Parameters.AddWithValue("@CompId", CompanyId);
                                            CompanyIdParameter.SqlDbType = SqlDbType.Int;
                                            SqlParameter ImportInvDataParameter = cmd.Parameters.AddWithValue("@ImportInvData", dt);
                                            ImportInvDataParameter.SqlDbType = SqlDbType.Structured;
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
                BusinessCont.SaveLog(0, BranchId, CompanyId, "ImportInvoiceData", "Import Invoice Upload Data:  " + Addedby, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return message.RetResult;
        }
        #endregion

        #region Get Alloted Picklist For Picker
        [HttpPost]
        [Route("OrderDispatch/GetAllotedPickListForPicker")]
        public List<Picklstmodel> GetAllotedPickListForPicker(Picklstmodel model)
        {
            List<Picklstmodel> picklstmodels = new List<Picklstmodel>();
            try
            {
                picklstmodels = _unitOfWork.OrderDispatchRepository.GetAllotedPickListForPicker(model.BranchId,model.CompId,model.PickerId,model.PicklistDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetAllotedPickListForPicker", "Get Alloted Picklist For Picker", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return picklstmodels;
        }
        #endregion

        #region Invoice Header Status Update
        [HttpPost]
        [Route("OrderDispatch/InvoiceHeaderStatusUpdate")]
        public string InvoiceHeaderStatusUpdate([FromBody] InvoiceHeaderStatusUpdateModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.InvoiceHeaderStatusUpdate(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "InvoiceHeaderStatusUpdate", "Invoice Header Status Update " + "BranchId:  " + model.BranchId + "CompId:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Assign Transport Mode
        [HttpPost]
        [Route("OrderDispatch/AssignTransportMode")]
        public string AssignTransportMode([FromBody] AssignTransportModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.AssignTransportMode(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AssignTransportMode", "Assign Transport Mode", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Get PickList Summary Data
        [HttpPost]
        [Route("OrderDispatch/GetPickListSummaryData")]
        public PickLstSummaryData GetPickListSummaryData(PickLstSummaryData model)
        {
            PickLstSummaryData PicklistData = new PickLstSummaryData();
            try
            {
                PicklistData = _unitOfWork.OrderDispatchRepository.GetPickListSummaryData(model.BranchId,model.CompId,model.PickerId,model.PicklistDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickListSummaryData", "Get Picklist Summary Data  ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return PicklistData;
        }
        #endregion

        #region Get PickList Summary Data
        [HttpPost]
        [Route("OrderDispatch/GetPickListSummaryCounts")]
        public PickLstSummaryData GetPickListSummaryCounts(PickLstSummaryData model)
        {
            PickLstSummaryData PicklistData = new PickLstSummaryData();
            try
            {
                PicklistData = _unitOfWork.OrderDispatchRepository.GetPickListSummaryCounts(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickListSummaryData", "Get Picklist Summary Data  ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return PicklistData;
        }
        #endregion

        #region Invoice Header List For Assign Trans Mode
        [HttpPost]
        [Route("OrderDispatch/InvoiceHeaderListForAssignTransMode")]
        public List<InvoiceLstModel> InvoiceListForAssignTransMode(InvoiceLstModel model)
        {
            List<InvoiceLstModel> InvoiceLst = new List<InvoiceLstModel>();
            try
            {
                InvoiceLst = _unitOfWork.OrderDispatchRepository.InvoiceListForAssignTransMode(model.BranchId, model.CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "InvoiceListForAssignTransMode", "Invoice Header List For Assign Trans Mode " + "BranchId:  " + model.BranchId + "CompId:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvoiceLst;
        }
        #endregion

        #region Import LR Data - Addedby
        [HttpPost]
        [Route("OrderDispatch/ImportLrData/{Addedby}")]
        public string ImportLrData(string Addedby)
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

                    if (file.FileName == "ImportLRDataTemplate.xls" || file.FileName == "ImportLRDataTemplate.xlsx")
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
                            List<ImportLrDataModel> modelList = new List<ImportLrDataModel>();

                            for (int i = 0; i < finalRecords.Rows.Count; i++)
                            {
                                ImportLrDataModel modelColumn = new ImportLrDataModel();
                                modelColumn.InvoiceNo = Convert.ToString(finalRecords.Rows[i][0]);
                                modelColumn.InvoiceDate = Convert.ToString(finalRecords.Rows[i][1]);
                                modelColumn.LRNo = Convert.ToString(finalRecords.Rows[i][2]);
                                string LRDate = Convert.ToString(finalRecords.Rows[i][3]);
                                string LRBox = Convert.ToString(finalRecords.Rows[i][4]);

                                if (modelColumn.InvoiceNo == "InvoiceNo" && modelColumn.InvoiceDate == "InvoiceDate" && modelColumn.LRNo == "LRNo" && LRDate == "LRDate" && LRBox == "LRBox")
                                {
                                    for (int j = 1; j < finalRecords.Rows.Count; j++)
                                    {
                                        ImportLrDataModel model = new ImportLrDataModel();
                                        model.pkId = j;
                                        model.InvoiceNo = Convert.ToString(finalRecords.Rows[j][0]);
                                        model.InvoiceDate = Convert.ToString(finalRecords.Rows[j][1]);
                                        model.LRNo = Convert.ToString(finalRecords.Rows[j][2]);
                                        model.LRDate = Convert.ToDateTime(finalRecords.Rows[j][3]).ToString("yyyy/MM/dd hh:mm:ss");
                                        model.LRBox = Convert.ToInt32(finalRecords.Rows[j][4]);
                                        modelList.Add(model);
                                    }
                                }
                            }

                            if (modelList.Count > 0)
                            {
                                // Create DataTable
                                DataTable dt = new DataTable();
                                dt.Columns.Add("pkId");
                                dt.Columns.Add("InvoiceNo");
                                dt.Columns.Add("InvoiceDate");
                                dt.Columns.Add("LRNo");
                                dt.Columns.Add("LRDate");
                                dt.Columns.Add("LRBox");

                                foreach (var itemList in modelList)
                                {
                                    // Add Rows DataTable
                                    dt.Rows.Add(itemList.pkId, itemList.InvoiceNo, itemList.InvoiceDate, itemList.LRNo, itemList.LRDate, itemList.LRBox);
                                }

                                if (dt.Rows.Count > 0)
                                {
                                    using (var db = new CFADBEntities())
                                    {
                                        SqlConnection connection = (SqlConnection)db.Database.Connection;
                                        SqlCommand cmd = new SqlCommand("CFA.usp_ImportLRData", connection);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        SqlParameter LRDataParameter = cmd.Parameters.AddWithValue("@LRData", dt);
                                        LRDataParameter.SqlDbType = SqlDbType.Structured;
                                        SqlParameter AddedbyParameter = cmd.Parameters.AddWithValue("@Addedby", Addedby);
                                        AddedbyParameter.SqlDbType = SqlDbType.NVarChar;
                                        SqlParameter RetValParameter = cmd.Parameters.AddWithValue("@RetValue", 0);
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
                                    else if (InsertedCount == -2)
                                    {
                                        message = BusinessCont.msg_InvalidInvDtsImported;
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
                BusinessCont.SaveLog(0, 0, 0, "ImportLRData", "Import LR Upload Data:  " + Addedby, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return message;
        }
        #endregion

        #region Generate PDF, QR Coode
        [HttpPost]
        [Route("OrderDispatch/GeneratePDF")]
        public string GeneratePDF(GeneratePDFModel model)
        {
            string msgPDF = string.Empty;
            PrintPDFDataModel modelPdf = new PrintPDFDataModel();  // new instance created

            try
            {
                BusinessCont.SaveLog(0, 0, 0, "GeneratePDF", "Generate PDF and QR Coode", "START", "");
                modelPdf.BranchId = model.BranchId;
                modelPdf.CompId = model.CompId;
                modelPdf.InvId = Convert.ToInt32(model.InvId);
                modelPdf.GpId = 0;
                modelPdf.Type = model.Type;
                if (!string.IsNullOrEmpty(model.BoxNo))
                {
                    modelPdf.BoxNo = model.BoxNo;
                }
                else
                {
                    modelPdf.BoxNo = model.BoxNo;
                }
                modelPdf.Action = BusinessCont.ADDRecord; // ADD
                modelPdf.Flag = BusinessCont.PendingPrinterMsg; // After PDF Saved -> Flag to set default - Pending
                modelPdf.AddedBy = model.AddedBy;
                msgPDF = _unitOfWork.OrderDispatchRepository.PrinterPDFData(modelPdf);
                BusinessCont.SaveLog(0, 0, 0, "GeneratePDF", "Generate PDF and QR Coode", "END", "");
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GeneratePDF", "Generate PDF and QR Coode", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return msgPDF;
        }
        #endregion

        #region Get PickList Generate New No
        [HttpPost]
        [Route("OrderDispatch/GetPickListGenerateNewNo")]
        public string GetPickListGenerateNewNo(PickListModel model)
        {
            string pickListNo = string.Empty;

            try
            {
                pickListNo = _unitOfWork.OrderDispatchRepository.GetPickListGenerateNewNo(model.BranchId, model.CompId,model.PicklistDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickListGenerateNewNo", "Get PickList Generate New No " + "BranchId:  " + model.BranchId + "CompId:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return pickListNo;
        }
        #endregion

        #region Get Invoice Details For Sticker
        [HttpGet]
        [Route("OrderDispatch/GetInvoiceDetailsForSticker/{BranchId}/{CompId}/{InvId}")]
        [AllowAnonymous]
        public List<GetInvoiceDetailsForStickerModel> GetInvoiceDetailsForSticker(int BranchId, int CompId, int InvId)
        {
            List<GetInvoiceDetailsForStickerModel> modelList = new List<GetInvoiceDetailsForStickerModel>();
            try
            {
                modelList = _unitOfWork.OrderDispatchRepository.GetInvoiceDetailsForSticker(BranchId, CompId, InvId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceDetailsForSticker", "Get Invoice Details For Sticker " + "BranchId:  " + BranchId + "CompId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Get LR Details List
        [HttpPost]
        [Route("OrderDispatch/GetLRDataList")]
        public List<ImportLrDataModel> GetLRDataList(ImportLrDataModel model)
        {
            List<ImportLrDataModel> LRLst = new List<ImportLrDataModel>();
            try
            {
                LRLst = _unitOfWork.OrderDispatchRepository.GetLRDataLst(model.BranchId, model.CompId, model.LRDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetLRDataList", "Get LR Data List " + "BranchId:  " + model.BranchId + "CompId:  " + model.CompId + "LRDate:  " + model.LRDate   , BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return LRLst;
        }
        #endregion

        #region Get PickList For Re-Allotment
        [HttpPost]
        [Route("OrderDispatch/GetPickForReAllotment")]
        public List<PickListModel> GetPickListForReAllotment(PickListModel model)
        {
            List<PickListModel> pickListList = new List<PickListModel>();

            try
            {
                pickListList = _unitOfWork.OrderDispatchRepository.GetPickListForReAllotment(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickListGenerateNewNo", "Get PickList Generate New No " + "BranchId:  " + model.BranchId + "CompId:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return pickListList;
        }
        #endregion

        #region Get Invoice Summary Counts
        [HttpPost]
        [Route("OrderDispatch/GetInvoiceSummaryCounts")]
        public InvCntModel InvoiceSummaryCounts(InvCntModel model)
        {
            InvCntModel InvCnts = new InvCntModel();

            try
            {
                InvCnts = _unitOfWork.OrderDispatchRepository.InvoiceSummaryCounts(model.BranchId,model.CompId,model.InvDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceSummaryCounts", "Get Invoice Summary Counts" + "BranchId:  " + model.BranchId + "CompId:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvCnts;
        }
        #endregion

        #region PickList Header Delete
        [HttpPost]
        [Route("OrderDispatch/PickListHeaderDelete")]
        public string PickListHeaderDelete([FromBody] PickListModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.PickListHeaderDelete(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PickListHeaderDelete", "PickLis tHeader Delete", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Invoice Status List
        [HttpGet]
        [Route("OrderDispatch/InvoiceStatusList")]
        public List<InvSts> InvoiceStatusListForMob()
        {
            List<InvSts> Result = new List<InvSts>();
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.InvoiceStatusForMob();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "InvoiceStatusListForMob", "Invoice Status List For Mob", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Get Printer Details
        [HttpGet]
        [Route("OrderDispatch/GetPrinterDetails/{BranchId}/{CompId}")]
        [AllowAnonymous]
        public List<PrinterDtls> GetPrinterDetails(int BranchId, int CompId)
        {
            List<PrinterDtls> Result = new List<PrinterDtls>();
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.GetPrinterDetails(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPrinterDetails", "Get Printer Details - BranchId: " + BranchId +  "  CompanyId:  " + CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Printer Log Add/Edit
        [HttpPost]
        [Route("OrderDispatch/PrinterLogAddEdit")]
        [AllowAnonymous]
        public string PrinterLogAddEdit([FromBody] PrinterLogAddEditModel model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.OrderDispatchRepository.PrinterLogAddEdit(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PrinterLogAddEdit", "Printer Log Add/Edit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        #region Printer PDF Data
        [HttpPost]
        [Route("OrderDispatch/PrinterPDFData")]
        [AllowAnonymous]
        public string PrinterPDFData([FromBody] PrintPDFDataModel model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.OrderDispatchRepository.PrinterPDFData(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PrinterPDFData", "Printer PDF Data", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        #region Get Print PDF Data
        [HttpGet]
        [Route("OrderDispatch/GetPrintPDFData/{BranchId}/{CompId}")]
        [AllowAnonymous]
        public List<PrintPDFDataModel> GetPrintPDFData(int BranchId, int CompId)
        {
            List<PrintPDFDataModel> Result = new List<PrintPDFDataModel>();
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.GetPrintPDFData(BranchId,CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPrintPDFData", "Get Print PDF Data", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Generate Gatepass Add Edit 
        [HttpPost]
        [Route("OrderDispatch/GatePassAddEdit")]
        public string GatePassAddEdit([FromBody] GatePassModel model)
        {
            string result = string.Empty,emailDtls= string.Empty;
            List<InvDtlsForEmail> invaGatepassDtls = new List<InvDtlsForEmail>();
            try
            {
                result = _unitOfWork.OrderDispatchRepository.GenerateGatepasAddEdit(model);
                if(result != "")
                {
                    invaGatepassDtls = _unitOfWork.OrderDispatchRepository.GetInvDtlsForEmail(model.BranchId, model.CompId, Convert.ToInt32(result));
                    if (invaGatepassDtls.Count > 0)
                    {
                        foreach (var i in invaGatepassDtls)
                        {
                            BusinessCont.SaveLog(0, 0, 0, "sendEmailForDispatchDone", " ", "START", "");
                            emailDtls = _unitOfWork.OrderDispatchRepository.sendEmailForDispatchDone(i.Emailid, i.StockistName, i.PONo, i.PODate, i.TransporterName,i.CompanyName,model.BranchId,model.CompId);
                            BusinessCont.SaveLog(0, 0, 0, "sendEmailForDispatchDone"," ", "END", "");
                        }
                    }
                    else
                    {
                        BusinessCont.SaveLog(0, 0, 0, "invaGatepassDtls", "No Records Found", "", "");
                    }
                }
                else
                {
                    BusinessCont.SaveLog(0, 0, 0, "invaGatepassDtls", "result;  "+ result, "", "");
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GatePassAddEdit", "Generate Gatepass Add Edit ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            if(result != "")
            {
                return "Success";
            }
            else
            {
                return "Fail";
            }
            
        }
        #endregion

        #region Gatepass List Generate New No
        [HttpPost]
        [Route("OrderDispatch/GatepassListGenerateNewNo")]
        public string GatepassListGenerateNewNo(GatePassModel model)
        {
            string GatepassNo = string.Empty;

            try
            {
                GatepassNo = _unitOfWork.OrderDispatchRepository.GatepassListGenerateNewNo(model.BranchId,model.CompId,model.GatepassDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GatepassListGenerateNewNo", "Gatepass Generate New No", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return GatepassNo;
        }
        #endregion

        #region Get Gatepass Dtls For PDF
        [HttpGet]
        [Route("OrderDispatch/GetGatepassDtlsForPDF/{BranchId}/{CompId}/{GPid}")]
        [AllowAnonymous]
        public List<InvGatpassDtls> GetGatepassDtlsForPDF(int BranchId, int CompId, int GPid)
        {
            List<InvGatpassDtls> modelList = new List<InvGatpassDtls>();
            try
            {
                modelList = _unitOfWork.OrderDispatchRepository.GetGatepassDtlsForPDF(BranchId, CompId, GPid);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetGatepassDtlsForPDF", "Get Gatepass Dtls For PDF", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Generate Gatepass PDF
        /// <summary>
        /// Generate Gatepass PDF
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("OrderDispatch/GenerateGatepassPDF")]
        public string GenerateGatepassPDF(GenerateGatepassPDFModel model)
        {
            string msgPDF = string.Empty;
            PrintPDFDataModel modelPdf = new PrintPDFDataModel();
            try
            {
                BusinessCont.SaveLog(0, 0, 0, "GenerateGatepassPDF", "Generate Gatepass PDF", "START", "");
                modelPdf.BranchId = model.BranchId;
                modelPdf.CompId = model.CompId;
                modelPdf.InvId = 0;
                modelPdf.GpId = model.GPid;
                modelPdf.Type = model.Type;
                modelPdf.Action = BusinessCont.ADDRecord; // ADD
                modelPdf.Flag = BusinessCont.PendingPrinterMsg; // After PDF Saved -> Flag to set default - Pending
                modelPdf.AddedBy = model.AddedBy;
                msgPDF = _unitOfWork.OrderDispatchRepository.PrinterPDFData(modelPdf);
                BusinessCont.SaveLog(0, 0, 0, "GenerateGatepassPDF", "Generate Gatepass PDF", "END", "");
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GenerateGatepassPDF", "Generate Gatepass PDF", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return msgPDF;
        }
        #endregion

        #region Get Gatepass Dtls For Mobile
        [HttpGet]
        [Route("OrderDispatch/GetGatepassDtlsForMob/{BranchId}/{CompId}")]
        public List<GatepassDtls> GetGatepassDtlsForMobile(int BranchId, int CompId)
        {
            List<GatepassDtls> modelList = new List<GatepassDtls>();
            try
            {
                modelList = _unitOfWork.OrderDispatchRepository.GetGatepassDtlsForMobile(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetGatepassDtlsForMobile", "Get Gatepass Dtls For Mobile", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Get Invoice Dtls For Mobile
        [HttpGet]
        [Route("OrderDispatch/GetInvoiceDtlsForMobile/{BranchId}/{CompId}/{InvStatus}")]
        public List<InvDtlsForMob> GetInvoiceDtlsForMobile(int BranchId, int CompId,int InvStatus)
        {
            List<InvDtlsForMob> modelList = new List<InvDtlsForMob>();
            try
            {
                modelList = _unitOfWork.OrderDispatchRepository.GetInvoiceDtlsForMobile(BranchId, CompId, InvStatus);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceDtlsForMobile", "Get Invoice Dtls For Mobile", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Gatepass Dtls For DeleteBy Id
        [HttpGet]
        [Route("OrderDispatch/GatepassDtlsForDeleteById/{GatepassId}")]
        public string GatepassDtlsForDeleteById(int GatepassId)
        {
            string msg = string.Empty;
            try
            {
                msg = _unitOfWork.OrderDispatchRepository.GatepassDtlsForDeleteById(GatepassId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GatepassDtlsForDeleteById", "Gatepass Dtls For DeleteBy Id", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return msg;
        }
        #endregion

        #region Print Details Add
        [HttpPost]
        [Route("OrderDispatch/PrintDetailsAdd")]
        [AllowAnonymous]
        public string PrintDetailsAdd(PrinterDtls model)
        {
            string msg = string.Empty;
            try
            {
                msg = _unitOfWork.OrderDispatchRepository.PrintDetailsAdd(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PrintDetailsAdd", "Print Details Add:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return msg;
        }
        #endregion

        #region Get Picklist By Picker Status
        [HttpPost]
        [Route("OrderDispatch/GetPicklistByPickerStatus")]
        public List<PickListModel> GetPicklistByPickerStatus(PickListModel model)
        {
            List<PickListModel> pickList = new List<PickListModel>();
            try
            {
                pickList = _unitOfWork.OrderDispatchRepository.GetPicklistByPickerStatus(model.BranchId, model.CompId, model.PicklistDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPicklistByPickerStatus", "Get Picklist By Picker Status", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return pickList;
        }
        #endregion

        #region Priority Invoice Flag Update
        [HttpPost]
        [Route("OrderDispatch/PriorityInvoiceFlagUpdate")]
        public string PriorityInvoiceFlagUpdate(PriorityFlagUpdtModel model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.OrderDispatchRepository.PriorityInvoiceFlagUpdate(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "PriorityInvoiceFlagUpdate", "Priority Invoice Flag Update: " + model.InvId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return result;
        }
        #endregion

        #region Picklist Resolve Concern Add
        [HttpPost]
        [Route("OrderDispatch/ResolveConcernAdd")]
        public string ResolveConcernAdd([FromBody] PickListModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.ResolveConcernAdd(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ResolveConcernAdd", "Resolve Concern Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Resolve Concern List 
        [HttpPost]
        [Route("OrderDispatch/ResolveConcernList")]
        public List<PickListModel> ResolveConcernList(PickListModel model)
        {
            List<PickListModel> pickList = new List<PickListModel>();
            try
            {
                pickList = _unitOfWork.OrderDispatchRepository.ResolveConcernLst(model.BranchId, model.CompId, model.PicklistDate);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPickList", "Get Pick List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return pickList;
        }
        #endregion

        #region Get Invoice Header List for Resolve Convern
        [HttpPost]
        [Route("OrderDispatch/GetInvoiceHeaderLstResolveCnrn")]
        public List<InvoiceLstModel> GetInvoiceHeaderListResolveCnrn(InvoiceLstModel model)
        {
            List<InvoiceLstModel> InvoiceLst = new List<InvoiceLstModel>();
            try
            {
                InvoiceLst = _unitOfWork.OrderDispatchRepository.GetInvoiceHeaderLstResolveCnrn(model.BranchId, model.CompId,model.BillDrawerId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetInvoiceHeaderListResolveCnrn", "Get Invoice Header List for Resolve Convern " + "BranchId:  " + model.BranchId + "CompId:  " + model.CompId, BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return InvoiceLst;
        }
        #endregion

        #region Invoice Resolve Convern Add
        [HttpPost]
        [Route("OrderDispatch/ResolveInvConcernAdd")]
        public string ResolveInvConcernAdd([FromBody] InvoiceLstModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.ResolveInvConcernAdd(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "ResolveInvConcernAdd", "Resolve Concern Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Get Assigned Transporter List
        [HttpPost]
        [Route("OrderDispatch/GetAssignedTransporterList")]
        public List<AssignedTransportModel> GetAssignedTransporterList(AssignedTransportModel model)
        {
            List<AssignedTransportModel> AssignTransList = new List<AssignedTransportModel>();
            try
            {
                AssignTransList = _unitOfWork.OrderDispatchRepository.GetAssignedTransporterList(model.BranchId, model.CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetAssignedTransporterList", "Get Assigned Transporter List ", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return AssignTransList;
        }
        #endregion

        #region Edit Assigned Transport Mode
        [HttpPost]
        [Route("OrderDispatch/EditAssignedTransportMode")]
        public string EditAssignedTransportMode([FromBody] AssignedTransportModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.OrderDispatchRepository.EditAssignedTransportMode(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "EditAssignedTransportMode", "Edit Assigned Transport Mode", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

    }
}
