using CNF.API.Controllers;
using CNF.Business.BusinessConstant;
using CNF.Business.Model.ChequeAccounting;
using CNF.Business.Repositories;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CNF.API.Classes
{
    public class SendEmailsToStockistNewChq : BaseApiController,IJob
    {
        EmailNotification emailNotification = new EmailNotification();
        string Subject = string.Empty, CCEmail = string.Empty, BodyText = string.Empty, result = string.Empty, Date=string.Empty, EmailCC=string.Empty;
        string[] Attachment;
        List<CCEmailDtls> CCEmailList = new List<CCEmailDtls>();
        public void Execute(IJobExecutionContext context)
        {
            bool RetValue = false;    
            try
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailsToStockistForNewChqSchedular", "Execute", "Scheduler Execution Start", BusinessCont.SuccessStatus);
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                Subject = ConfigurationManager.AppSettings["EmailSubject"] + Date + " ";
                BodyText = ConfigurationManager.AppSettings["BodyText"];
                CCEmailList = _unitOfWork.chequeAccountingRepository.GetCCEmailDtlsPvt(1, 1, 2);
                if (CCEmailList.Count > 0)
                {
                    for (int i = 0; i < CCEmailList.Count; i++)
                    {
                        CCEmail += ";" + CCEmailList[i].Email;
                    }
                    EmailCC = CCEmail.TrimStart(';');
                }
                var result = _unitOfWork.chequeAccountingRepository.GetEmailCountDetails(0, 0);
                foreach (var item in result)
                {
                    RetValue = emailNotification.SendEmailsToStockist(item.Emailid,item.StockistName, EmailCC, Subject, BodyText, Attachment);
                }
                BusinessCont.SaveLog(0, 0,0, "SendEmailsToStockistForNewChqSchedular", "Execute", "Scheduler Execution End", BusinessCont.SuccessStatus);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0,0, "Send Emails To Stockist For New Chq Schedular", "", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
        }
    }
}