using CNF.Business.BusinessConstant;
using CNF.Business.Model.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CNF.API.Controllers
{
    public class ConfigurationController : BaseApiController
    {
        #region Get App Configuration List
        /// <summary>
        /// Get App Configuration List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Configuration/GetAppConfigurationList")]
        public AppConfigurationList GetAppConfigurationList()
        {
            AppConfigurationList appconfigurationlist = new AppConfigurationList();
            try
            {
                {
                    appconfigurationlist = _unitOfWork.configurationRepository.GetAppConfiguration();
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GetAppConfigurationList", "Get App Configuration List", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return appconfigurationlist;
        }
        #endregion

        #region Add Edit App Configuration
        /// <summary>
        /// Add Edit App Configuration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Configuration/AddEditAppConfiguration")]
        public string AddEditAppConfiguration([FromBody] AppConfigurationLst model)
        {
            string result = string.Empty;
            try
            {
                result = _unitOfWork.configurationRepository.AddEditAppConfiguration(model);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "AddEditAppConfiguration", "Add Edit App configuration", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex));
            }
            return result;
        }
        #endregion

    }


}