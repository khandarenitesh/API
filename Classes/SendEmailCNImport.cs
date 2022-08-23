using CNF.API.Controllers;
using CNF.Business.BusinessConstant;
using CNF.Business.Model.ChequeAccounting;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using static CNF.Business.Model.OrderReturn.OrderReturn;

namespace CNF.API.Classes
{
    public class SendEmailCNImport : BaseApiController, IJob
    {
        EmailNotification emailNotification = new EmailNotification();
        string Subject = string.Empty, CCEmail = string.Empty, ChqNo = string.Empty, InvNo = string.Empty, result = string.Empty, Date = string.Empty, EmailCC = string.Empty, MailFilePath = string.Empty, ToEmail = string.Empty, msgHtml = string.Empty, Email = string.Empty;
        List<CCEmailDtls> CCEmailList = new List<CCEmailDtls>();
        

        public void Execute(IJobExecutionContext context)
        {
            bool SendMail = false;
            try
            {
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                Subject = ConfigurationManager.AppSettings["ImportCN"] + Date + " ";
                ToEmail = ConfigurationManager.AppSettings["ToEmail"];
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\SendEmail_Import CN.html");
                CCEmailList = _unitOfWork.chequeAccountingRepository.GetCCEmailDtlsPvt(1, 1, 7);
                if (CCEmailList.Count > 0)
                {
                    for (int i = 0; i < CCEmailList.Count; i++)
                    {
                        CCEmail += ";" + CCEmailList[i].Email;
                    }
                    EmailCC = CCEmail.TrimStart(';');
                }
                var Emaildata = _unitOfWork.OrderReturnRepository.GetImportCNDataForEmail(1, 1);
                foreach (var item in Emaildata)
                {
                    SendMail = emailNotification.SendEmailImportCN(item.Emailid, EmailCC,item.StockistName, Subject, MailFilePath);
                }
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "Send Email When Import CN", "", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
        }
    }
}