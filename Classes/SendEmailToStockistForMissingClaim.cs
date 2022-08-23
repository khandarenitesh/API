using CNF.API.Controllers;
using CNF.Business.BusinessConstant;
using CNF.Business.Model.ChequeAccounting;
using CNF.Business.Model.Login;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace CNF.API.Classes
{

    public class SendEmailToStockistForMissingClaim : BaseApiController, IJob
    {
        string msg = string.Empty, messageformat = string.Empty, Date = string.Empty, Subject = string.Empty, MailFilePath = string.Empty, ToEmail = string.Empty, CCEmail = string.Empty, ClaimBodyText = string.Empty, EmailCC = string.Empty;
        EmailSend Email = new EmailSend();
        List<CCEmailDtls> CCEmailList = new List<CCEmailDtls>();
        CCEmailDtls CCEmailModel = new CCEmailDtls();
        public void Execute(IJobExecutionContext context)
        {
            bool bResult = false;
            try
            {
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                Subject = ConfigurationManager.AppSettings["ClaimSubj"] + Date + " ";
                ClaimBodyText = ConfigurationManager.AppSettings["ClaimBodyText"];
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\SendEmailForMissingClaimForm.html");
                CCEmailList = _unitOfWork.chequeAccountingRepository.GetCCEmailDtlsPvt(1, 1, 3);
                if (CCEmailList.Count > 0)
                {
                    for (int i = 0; i < CCEmailList.Count; i++)
                    {
                        CCEmail += ";" + CCEmailList[i].Email;
                    }

                    EmailCC = CCEmail.TrimStart(';');
                }
                var result = _unitOfWork.OrderReturnRepository.GetStockistDtlsForMissingClaim(0,0,0);
                //var result = _unitOfWork.chequeAccountingRepository.GetLRImportDetailsList(0, 0);
                foreach (var item in result)
                {
                    bResult = EmailNotification.SendEmailMissingClaimForm(item.Emailid,EmailCC,Subject,item.StockistName, ClaimBodyText, MailFilePath);
                }
                BusinessCont.SaveLog(0, 0, 0, "SendEmailForMissingClaimFormSchedular", "Execute", "Scheduler Execution End", BusinessCont.SuccessStatus);
                
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailForMissingClaimForm", "Send Email For Missing Claim Form", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
        }
    }
}