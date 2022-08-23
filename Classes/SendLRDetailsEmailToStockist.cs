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

namespace CNF.API.Classes
{
    public class SendLRDetailsEmailToStockist : BaseApiController, IJob
    {
        EmailNotification emailNotification = new EmailNotification();
        string Subject = string.Empty, CCEmail = string.Empty,result = string.Empty, Date = string.Empty, EmailCC = string.Empty, MailFilePath = string.Empty, TransporterName=string.Empty, SONo=string.Empty, SODate=string.Empty, LRNo=string.Empty;
        List<CCEmailDtls> CCEmailList = new List<CCEmailDtls>();
        public void Execute(IJobExecutionContext context)
        {
            bool RetValue = false;
            try
            {
                BusinessCont.SaveLog(0, 0, 0, "SendLRDetailsEmailToStockistSchedular", "Execute", "Scheduler Execution Start", BusinessCont.SuccessStatus);
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                Subject = ConfigurationManager.AppSettings["LREmailSubject"] + Date + " ";
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\SendLRDetailsEmailToStockist.html");
                CCEmailList = _unitOfWork.chequeAccountingRepository.GetCCEmailDtlsPvt(1, 1, 6);
                if (CCEmailList.Count > 0)
                {
                    for (int i = 0; i < CCEmailList.Count; i++)
                    {
                        CCEmail += ";" + CCEmailList[i].Email;
                    }
                    EmailCC = CCEmail.TrimStart(';');
                }
                var result = _unitOfWork.chequeAccountingRepository.GetLRImportDetailsList(0, 0);
                foreach (var item in result)
                {
                    RetValue = emailNotification.SendLRImportEmailToStockist(item.Emailid,EmailCC,Subject,item.StockistName,item.PONo, item.PODate, item.TransporterName, item.LRNo, MailFilePath);
                }
                BusinessCont.SaveLog(0, 0, 0, "SendLRDetailsEmailToStockistSchedular", "Execute", "Scheduler Execution End", BusinessCont.SuccessStatus);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "Send LR Details Email To Stockist Schedular", "", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
        }

    }
}