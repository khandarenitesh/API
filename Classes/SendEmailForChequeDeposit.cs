using CNF.API.Controllers;
using CNF.Business.BusinessConstant;
using CNF.Business.Model.ChequeAccounting;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CNF.API.Classes
{
    public class SendEmailForChequeDeposit : BaseApiController, IJob
    {
        EmailNotification emailNotification = new EmailNotification();
        string Subject = string.Empty, CCEmail = string.Empty, ChqNo = string.Empty, InvNo= string.Empty, result = string.Empty, Date = string.Empty, EmailCC = string.Empty;
        List<CCEmailDtls> CCEmailList = new List<CCEmailDtls>();
        public void Execute(IJobExecutionContext context)
        {
            bool RetValue = false;
            try
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailsToStockistForChqDepositSchedular", "Execute", "Scheduler Execution Start", BusinessCont.SuccessStatus);
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                Subject = ConfigurationManager.AppSettings["EmailChqSubject"] + Date + " ";
                CCEmailList = _unitOfWork.chequeAccountingRepository.GetCCEmailDtlsPvt(1, 1, 7);
                if (CCEmailList.Count > 0)
                {
                    for (int i = 0; i < CCEmailList.Count; i++)
                    {
                        CCEmail += ";" + CCEmailList[i].Email;
                    }
                    EmailCC = CCEmail.TrimStart(';');
                }
                var result = _unitOfWork.chequeAccountingRepository.GetChequeDepositedList(0, 0);
                foreach (var item in result)
                {
                    RetValue = emailNotification.SendEmailsForChqDeposit(item.Emailid, item.StockistName, EmailCC, Subject, item.ChqNo, item.InvNo);
                }
                BusinessCont.SaveLog(0, 0, 0, "SendEmailsToStockistForChqDepositSchedular", "Execute", "Scheduler Execution End", BusinessCont.SuccessStatus);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "Send Emails To Stockist For Chq Deposit Schedular", "", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
        }
    }
}