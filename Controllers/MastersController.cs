using CNF.Business.BusinessConstant;
using System;
using System.Web.Http;
using CNF.Business.Model.Master;
using System.Collections.Generic;

namespace CNF.API.Controllers
{
    public class MastersController : BaseApiController
    {
        # region Get General Master List
        [HttpGet]
        [Route("Masters/GetGeneralMasterList/{CategoryName}/{Status}")]
        public GeneralMasterList GetGeneralMasterList(string CategoryName, string Status)
        {
            GeneralMasterList generalmasterlist = new GeneralMasterList();
            try
            {
                {
                    generalmasterlist = _unitOfWork.MastersRepository.GetGeneralMaster(CategoryName, Status);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0,"GetGeneralMasterList", "Get General Master List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return generalmasterlist;
        }
        #endregion

        # region AddEdit Branch Master 
        [HttpPost]
        [Route("Masters/BranchMasterAddEdit")]
        public string BranchMasterAddEdit([FromBody] BranchList model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.MastersRepository.BranchMasterAddEdit(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "BranchMasterAddEdit", "AddEdit Branch Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        #region Get State List
        [HttpGet]
        [Route("Masters/GetStateList/{Flag}")]
        public GetStateList GetStateList(string Flag)
        {
            GetStateList getstatelist = new GetStateList();
            try
            {
                {
                    getstatelist = _unitOfWork.MastersRepository.GetStateList(Flag);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStateList", "Get State List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return getstatelist;
        }
        #endregion

        #region Get City List
        [HttpGet]
        [Route("Masters/GetCityList/{StateCode}/{districtCode}/{Flag}")]
        public GetCityList GetCityList(string StateCode, string districtCode, string Flag)
        {
            GetCityList getCitylist = new GetCityList();
            try
            {
                getCitylist = _unitOfWork.MastersRepository.GetCityList(StateCode,districtCode, Flag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCityList", "Get City List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return getCitylist;
        }
        #endregion

        #region Get Branch List
        [HttpGet]
        [Route("Masters/GetBranchList/{Status}")]
        public List<BranchList> GetBranchList(string Status)
        {
            List<BranchList> branchList = new List<BranchList>();
            try
            {
                branchList = _unitOfWork.MastersRepository.GetBranchList(Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetBranchList", " Get Branch List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return branchList;
        }
        #endregion

        #region Company Detalis
        [HttpGet]
        [Route("Masters/GetCompanyList/{Status}")]
        public List<CompanyDtls> GetCompanyList(string Status)
        {
            List<CompanyDtls> modelList = new List<CompanyDtls>();
            try
            {
                modelList = _unitOfWork.MastersRepository.CompanyDtls(Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCompanyList", "Get Company Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }

        [HttpPost]
        [Route("Masters/CompanyDtlsAddEdit")]
        public string CompanyDtlsAddEdit([FromBody] CompanyDtls model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.MastersRepository.CompanyDtlsAddEdit(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0,model.CompanyId, "CompanyDtlsAddEdit", "Company Details AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Update user Activation
        [HttpPost]
        [Route("Masters/EmployeeMasterActivate")]  
        public string EmployeeMasterActivate([FromBody] EmployeeActiveModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.MastersRepository.EmployeeMasterActivate(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0,(model.EmpId != 0 ? model.EmpId : 0), "EmployeeMasterActivate", "Employee Activate", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Add Employee Details
        [HttpPost]
        [Route("Masters/AddEmployeeDtls")]
        public int AddEmployeeDtls([FromBody] AddEmployeeModel model)
        {
            int RetValue = 0;
            try
            {
                RetValue = _unitOfWork.MastersRepository.AddEmployeeDtls(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, (model.EmpId != 0 ? model.EmpId : 0), "AddEmployeeDtls", "Employee Add", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Edit Employee Detalis
        [HttpPost]
        [Route("Masters/EditEmployeeDtls")]
        public int EditEmployeeDtls([FromBody] AddEmployeeModel model)
        {
            int RetValue = 0;
            try
            {
                RetValue = _unitOfWork.MastersRepository.EditEmployeeDtls(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, (model.EmpId != 0 ? model.EmpId : 0), "EditEmployeeDtls", "Add Employee", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RetValue;
        }
        #endregion

        #region Get Employee Details
        [HttpGet]
        [Route("Masters/GetEmpCmpDtls/{EmpId}")]
        public List<EmployeeDtls> GetEmpCmpDtls(int EmpId)
        {
            List<EmployeeDtls> Emplist = new List<EmployeeDtls>();
            try
            {
                Emplist = _unitOfWork.MastersRepository.GetEmployeeDtls(EmpId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, EmpId, "GetEmpCmpDtls", "Get Employee Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Emplist;
        }
        #endregion

        # region Get Category List
        [HttpGet]
        [Route("Masters/GetCategoryList")]
        public GetCategoryList GetCategoryList()
        {
            GetCategoryList CategoryList = new GetCategoryList();
            try
            {
                {
                    CategoryList = _unitOfWork.MastersRepository.GetCategoryList();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetGeneralMasterList", "Get General Master List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return CategoryList;
        }
        #endregion

        # region Add Edit Division Master
        [HttpPost]
        [Route("Masters/AddEditDivisionMaster")]
        public string AddEditDivisionMaster([FromBody] DivisionMasterLst model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.MastersRepository.AddEditDivisionMaster(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, model.DivisionId, "AddEditDivisionMaster", "Add Edit Division Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        #region Get Division Master List
        [HttpGet]
        [Route("Masters/GetDivisionMasterList/{Status}")]
        public List<DivisionMasterLst> GetDivisionMasterList(string Status)
        {
            List<DivisionMasterLst> DivisionMasterList = new List<DivisionMasterLst>();
            try
            {
                DivisionMasterList = _unitOfWork.MastersRepository.GetDivisionMasterList(Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0,"GetBranchList", "Get Division Master List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return DivisionMasterList;

        }
        #endregion

        # region Add Edit General Master
        [HttpPost]
        [Route("Masters/AddEditGeneralMaster")]
        public string AddEditGeneralMaster([FromBody] GeneralMasterLst model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.MastersRepository.AddEditGeneralMaster(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0,"AddEditGeneralMaster", "Add Edit General Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        # region Add Edit Transporter Master
        [HttpPost]
        [Route("Masters/AddEditTransporterMaster")]
        public string AddEditTransporterMaster([FromBody] TransporterMasterLst model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.MastersRepository.AddEditTransporterMaster(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0,"AddEditTransporterMaster", "Add Edit Transporter Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        #region Get Transporter Master List
        [HttpGet]
        [Route("Masters/GetTransporterMasterList/{DistrictCode}/{Status}")]
        public List<TransporterMasterLst> GetTransporterMasterList(string DistrictCode, string Status)
        {
            List<TransporterMasterLst> GeneralMasterList = new List<TransporterMasterLst>();
            try
            {
                GeneralMasterList = _unitOfWork.MastersRepository.GetTransporterMasterList(DistrictCode, Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0,"GetTransporterMasterList", "Get Transporter Master List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return GeneralMasterList;

        }
        #endregion

        #region Get Role List
        [HttpGet]
        [Route("Masters/GetRoleList")]
        public List<RoleModel> GetRoleList()
        {
            List<RoleModel> RoleLst = new List<RoleModel>();
            try
            {
                RoleLst = _unitOfWork.MastersRepository.GetRoleLst();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetRoleList", "Get Role List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RoleLst;
        }
        #endregion

        #region Get Stockist List
        [HttpGet]
        [Route("Masters/GetStockistList/{BranchId}/{CompanyId}/{Status}")]
        public List<StockistModel> GetStockistList(int BranchId,int CompanyId, string Status)
        {
            List<StockistModel> StockLst = new List<StockistModel>();
            try
            {
                StockLst = _unitOfWork.MastersRepository.GetStockistLst(BranchId,CompanyId, Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistList", "Get Stockist List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return StockLst;
        }
        #endregion

        #region Stockist Master AddEdit
        [HttpPost]
        [Route("Masters/StockistDtlsAddEdit")]
        public string StockistDtlsAddEdit([FromBody] StockistModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.MastersRepository.StockistDtlsAddEdit(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, (model.CompanyId != 0 ? model.CompanyId : 0), "StockistDtlsAddEdit", "Stockis Details AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Get Stockist Details by Id
        [HttpGet]
        [Route("Masters/GetStockistBankList/{StockistId}")]
        public List<BankModel> StockistDtlsbyId(int StockistId)
        {
            List<BankModel> BankModel = new List<BankModel>();
            try
            {
                BankModel = _unitOfWork.MastersRepository.GetStockistBankList(StockistId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, StockistId, "GetStockistBankList", "Get Stockist Bank List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return BankModel;
        }
        #endregion

        #region Get Stokist Transport Mapping List
        [HttpGet]
        [Route("Masters/GetStokistTransportMappingList/{CompanyId}")]
        public List<StokistTransportModel> GetStokistTransportMappingList(int CompanyId)
        {
            List<StokistTransportModel> StockMapLst = new List<StokistTransportModel>();
            try
            {
                StockMapLst = _unitOfWork.MastersRepository.GetStokistTransportMappingList(CompanyId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStokistTransportMappingList", "Get Stokist Transport Mapping List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return StockMapLst;
        }
        #endregion

        #region Stokist Transport Mapping AddEdit
        [HttpPost]
        [Route("Masters/StokistTransportMappingAddEdit")]
        public string StokistTransportMappingAddEdit([FromBody] StokistTransportModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.MastersRepository.StokistTransportMappingAddEdit(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, (model.CompanyId != 0 ? model.CompanyId : 0), "StokistTransportMappingAddEdit", "Stokist Transport Mapping AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Get District List
        [HttpGet]
        [Route("Masters/GetDistrictList/{StateCode}/{Flag}")]
        public GetDistrictList GetDistrictList(string StateCode, string Flag)
        {
            GetDistrictList getDistrictlist = new GetDistrictList();
            try
            {
                getDistrictlist = _unitOfWork.MastersRepository.GetDistrictList(StateCode, Flag);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCityList", "Get City List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return getDistrictlist;
        }
        #endregion

        #region Carting Agent Details
        [HttpGet]
        [Route("Masters/GetCartingAgentLst/{Status}/{BranchId}")]
        public List<cartingAgentmodel> GetCartingAgentLst(string Status, int BranchId)
        {
            List<cartingAgentmodel> CALst = new List<cartingAgentmodel>();
            try
            {
                CALst = _unitOfWork.MastersRepository.GetCartingAgentLst(Status, BranchId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, BranchId, "GetCartingAgentLst", "Get carting agent List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return CALst;
        }

        [HttpPost]
        [Route("Masters/CartingAgentMasterAddEdit")]
        public string CartingAgentMasterAddEdit([FromBody] cartingAgentmodel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.MastersRepository.CartingAgentMasterAddEdit(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "CartingAgentMasterAddEdit", "Stockis Details AddEdit", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion

        #region Get Employee Master List
        [HttpGet]
        [Route("Masters/GetEmployeeMasterList/{BranchId}/{Status}")]
        public List<EmployeeMasterList> GetEmployeeMasterList(int BranchId, string Status)
        {
            List<EmployeeMasterList> objList = new List<EmployeeMasterList>();
            try
            {
                objList = _unitOfWork.MastersRepository.GetEmployeeMasterList(BranchId, Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetEmployeeMasterList", "Get Employee Master List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return objList;

        }
        #endregion

        #region Update User Only Activation
        [HttpPost]
        [Route("Masters/UserActiveDeactive")]
        public string UserActiveDeactive([FromBody] EmployeeActiveModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.MastersRepository.UserActiveDeactive(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, model.EmpId, "UserActiveDeactive", "Update User Activate", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion
       
        #region Courier Master
        [HttpPost]
        [Route("Masters/AddEditCourierMaster")]
        public string AddEditCourierMaster([FromBody] CourierMasterLst model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.MastersRepository.AddEditCourierMaster(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditCourierMaster", "Add Edit courier Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
       
        [HttpGet]
        [Route("Masters/GetCourierMasterList/{BranchId}/{DistrictCode}/{Status}")]
        public List<CourierMasterLst> GetCourierMasterList(int BranchId, string DistrictCode, string Status)
        {
            List<CourierMasterLst> GetList = new List<CourierMasterLst>();
            try
            {
                GetList = _unitOfWork.MastersRepository.GetcourierMasterList(BranchId,DistrictCode, Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCourierMasterList", "Get courier Master List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return GetList;

        }
        #endregion

        #region Get User Details
        [HttpGet]
        [Route("Masters/GetUserDtls/{UserId}")]
        public UserDtls GetUserDtls(int UserId)
        {
            UserDtls UserLst = new UserDtls();
            try
            {
                UserLst = _unitOfWork.MastersRepository.GetUserDtls(UserId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetUserDtls", "Get Use Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return UserLst;
        }
        #endregion

        #region Get Stockist By Id
        [HttpGet]
        [Route("Masters/StockistById/{StockistId}")]
        public StockistModel GetStockistById(int StockistId)
        {
            StockistModel StockList = new StockistModel();
            try
            {
                //StockList = _unitOfWork.MastersRepository.GetStockistById(StockistId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistById", "Get Stockist By Id", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return StockList;
        }
        #endregion

        #region Get Transporter By Id
        [HttpGet]
        [Route("Masters/TransporterById/{TransporterId}")]
        public TransporterMasterLst GetTransporterById(int TransporterId)
        {
            TransporterMasterLst transporterMasterList = new TransporterMasterLst();
            try
            {
                transporterMasterList = _unitOfWork.MastersRepository.GetTransporterById(TransporterId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetTransporterById", "Get Transporter By Id", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return transporterMasterList;
        }
        #endregion

        #region Get Branch Details By BranchId
        [HttpGet]
        [Route("Masters/GetBranchByIdDtls/{BranchId}")]
        public List<BranchIdDtls> GetBranchByIdDtls(int BranchId)
        {
            List<BranchIdDtls> model = new List<BranchIdDtls>();
            try
            {
                model = _unitOfWork.MastersRepository.GetBranchByIdDtls(BranchId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, BranchId, "GetBranchByIdDtls", "Get Branch Details By BranchId", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        # region Add Edit Stockist Company Relation
        [HttpPost]
        [Route("Masters/AddEditStockistCompanyRelation")]
        public string AddEditStockistCompanyRelation([FromBody] StockistRelation model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.MastersRepository.AddEditStockistCompanyRelation(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditTransporterMaster", "Add Edit Transporter Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        # region Add Edit Stockist Branch Relation
        [HttpPost]
        [Route("Masters/AddEditStockistBranchRelation")]
        public string AddEditStockistBranchRelation([FromBody] StockistRelation model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.MastersRepository.AddEditStockistBranchRelation(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditTransporterMaster", "Add Edit Transporter Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

        #region Get Stockist Branch Relation List
        [HttpGet]
        [Route("Masters/GetStockistBranchRelationList/{BranchId}")]
        public List<StockistRelation> GetStockistBranchRelationList(int BranchId)
        {
            List<StockistRelation> StockLst = new List<StockistRelation>();
            try
            {
                StockLst = _unitOfWork.MastersRepository.GetStockistBranchRelationList(BranchId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistList", "Get Stockist List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return StockLst;
        }
        #endregion

        #region Get Stockist Company List
        [HttpGet]
        [Route("Masters/GetStockistCompanyRelationList/{CompId}")]
        public List<StockistRelation> GetStockistCompanyRelationList(int CompId)
        {
            List<StockistRelation> StockLst = new List<StockistRelation>();
            try
            {
                StockLst = _unitOfWork.MastersRepository.GetStockistCompanyRelationList(CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistList", "Get Stockist List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return StockLst;
        }
        #endregion

        #region Get Check Username Available
        [HttpGet]
        [Route("Masters/GetCheckUsernameAvailable/{Username}")]
        public CheckUsernameAvailableModel GetCheckUsernameAvailable(string Username)
        {
            CheckUsernameAvailableModel model = new CheckUsernameAvailableModel();
            try
            {
                model = _unitOfWork.MastersRepository.GetCheckUsernameAvailable(Username);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCheckUsernameAvailable", "Get Check Username Available", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Stockist List By Branch 
        [HttpGet]
        [Route("Masters/GetStockistListByBranch/{BranchId}/{Status}")]
        public List<StockistModel> GetStockistListByBranch(int BranchId, string Status)
        {
            List<StockistModel> StockLst = new List<StockistModel>();
            try
            {
                StockLst = _unitOfWork.MastersRepository.GetStockistListByBranch(BranchId, Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistBranchRelationListByBranchId", "Get Stockist Branch RelationList By BranchId", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return StockLst;
        }
        #endregion

        #region Get Stockist List By Company 
        [HttpGet]
        [Route("Masters/GetStockistListByCompany/{CompanyId}/{Status}")]
        public List<StockistModel> GetStockistListByCompany(int CompanyId, string Status)
        {
            List<StockistModel> StockLst = new List<StockistModel>();
            try
            {
                StockLst = _unitOfWork.MastersRepository.GetStockistListByCompany(CompanyId, Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistBranchRelationListByCompanyId", "Get Stockist Company RelationList By CompanyId", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return StockLst;
        }
        #endregion

        #region Company Detalis for Login
        [HttpGet]
        [Route("Master/GetCompanyListForLogIn/{Status}")]
        [AllowAnonymous]
        public List<CompanyDtls> GetCompanyListForLogin(string Status)
        {
            List<CompanyDtls> modelList = new List<CompanyDtls>();
            try
            {
                modelList = _unitOfWork.MastersRepository.CompanyDtls(Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCompanyList", "Get Company Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Get Role List for Login
        [HttpGet]
        [Route("Masters/GetRoleListForLogIn")]
        [AllowAnonymous]
        public List<RoleModel> GetRoleListForLogIn()
        {
            List<RoleModel> RoleLst = new List<RoleModel>();
            try
            {
                RoleLst = _unitOfWork.MastersRepository.GetRoleLst();
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetRoleList", "Get Role List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RoleLst;
        }
        #endregion

        #region Get Roles Details by EmpId
        [HttpGet]
        [Route("Masters/GetRolesdls/{EmpId}")]
        public List<RolesModel> GetRolesls(int EmpId)
        {
            List<RolesModel> Rolelist = new List<RolesModel>();
            try
            {
                Rolelist = _unitOfWork.MastersRepository.GetRolesls(EmpId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, EmpId, "GetRolesdls", "Get Roles Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Rolelist;
        }
        #endregion

        #region Get Guard Details
        [HttpGet]
        [Route("Masters/GetGuardDetails/{BranchId}/{CompId}")]
        public List<GuardDetails> GetGuardDetails(int BranchId, int CompId)
        {
            List<GuardDetails> RoleLst = new List<GuardDetails>();
            try
            {
                RoleLst = _unitOfWork.MastersRepository.GetGuardDetails(BranchId, CompId);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetGuardDetails", "Get Guard Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return RoleLst;
        }
        #endregion

        #region Create User
        [HttpPost]
        [Route("Masters/CreateUser")]
        public string CreateUser([FromBody] CreateUserModel model)
        {
            string Result = string.Empty;
            try
            {
                Result = _unitOfWork.MastersRepository.CreateUser(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, (model.EmpId != 0 ? model.EmpId : 0), "CreateUser", "Create User", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Result;
        }
        #endregion
  
        #region get Print List
        [HttpGet]
        [Route("Masters/GetPrintCompanyList/{Status}")]
        [AllowAnonymous]
        public List<CompanyDtls> GetPrintCompanyList(string Status)
        {
            List<CompanyDtls> modelList = new List<CompanyDtls>();
            try
            {
                modelList = _unitOfWork.MastersRepository.CompanyDtls(Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPrintCompanyList", "Get Print Company Details", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return modelList;
        }
        #endregion

        #region Get Check Employee Number Available
        [HttpPost]
        [Route("Masters/GetCheckEmployeeNumberAvilable")]
        public AddEmployeeModel GetCheckEmployeeNumberAvilable(AddEmployeeModel Model)
        {
            AddEmployeeModel model = new AddEmployeeModel();

            try
            {
                model = _unitOfWork.MastersRepository.GetCheckEmployeeNumberAvilable(Model.EmpId,Model.EmpNo, Model.EmpEmail, Model.EmpMobNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCheckEmployeeNumberAvilable", "Get Check Employee Number Available", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Check Carting Agent Available
        [HttpPost]
        [Route("Masters/GetCheckCartingAgentAvilable")]
        public cartingAgentmodel GetCheckCartingAgentAvilable(cartingAgentmodel Model)
        {
            cartingAgentmodel model = new cartingAgentmodel();
            try
            {
                model = _unitOfWork.MastersRepository.GetCheckCartingAgentAvilable(Model.CAName);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCheckCartingAgentAvilable", "Get check Carting Agent Available", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Check Courier Name Available
        [HttpPost]
        [Route("Masters/GetCheckCourierNameAvilable")]
        public CourierMasterLst GetCheckCourierNameAvilable(CourierMasterLst Model)
        {
            CourierMasterLst model = new CourierMasterLst();
            try
            {
                model = _unitOfWork.MastersRepository.GetCheckCourierNameAvilable(Model.CourierName);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetCheckCourierNameAvilable", "Get check Courier Name Available", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return model;
        }
        #endregion

        #region Get Check StocksitNo is Available
        [HttpGet]
        [Route("Masters/GetStockistNoAvailable/{StockistNo}")]
        public StockistModel GetStockistNoAvailable(string StockistNo)
        {
            StockistModel stockist = new StockistModel();
            try
            {
                stockist = _unitOfWork.MastersRepository.GetStockistNoAvailables(StockistNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetStockistNoAvailable", "Get Check StocksitNo Available", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return stockist;
        }
        #endregion

        #region Get Check TransporterNo is Available
        [HttpGet]
        [Route("Masters/GetTransporterNoAvailable/{TransporterNo}")]
        public TransporterMasterLst GetTransporterNoAvailable(string TransporterNo)
        {
            TransporterMasterLst Transporter = new TransporterMasterLst();
            try
            {
                Transporter = _unitOfWork.MastersRepository.GetTransporterNoAvailables(TransporterNo);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetTransporterNoAvailable", "Get Check TransporterNo is Available", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return Transporter;
        }
        #endregion

        #region Get Print Branch List
        [HttpGet]
        [Route("Masters/GetPrintBranchList/{Status}")]
        [AllowAnonymous]
        public List<BranchList> GetGetPrintBranchList(string Status)
        {
            List<BranchList> branchList = new List<BranchList>();
            try
            {
                branchList = _unitOfWork.MastersRepository.GetBranchList(Status);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetPrintBranchList", " Get Print Branch List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return branchList;
        }
        #endregion


        #region Add Edit City Master
        [HttpPost]
        [Route("Masters/AddEditCityMaster")]
        public string AddEditCityMaster([FromBody] CityMaster model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.MastersRepository.AddEditCityMaster(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditGeneralMaster", "Add Edit General Master", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion
    }
}