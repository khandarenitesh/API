using CNF.API.Controllers;
using CNF.Business.BusinessConstant;
using CNF.Business.Model.ChequeAccounting;
using CNF.Business.Model.Context;
using CNF.Business.Repositories;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace CNF.API.Classes
{
    public class SendAlertEmailToInternalTeam : BaseApiController, IJob
    {

        EmailNotification emailNotification = new EmailNotification();
        string Subject = string.Empty, CCEmail = string.Empty, ChqNo = string.Empty, InvNo = string.Empty, result = string.Empty, Date = string.Empty, EmailCC = string.Empty, MailFilePath = string.Empty, ToEmail=string.Empty, msgHtml = string.Empty,Email = string.Empty;
        List<CCEmailDtls> CCEmailList = new List<CCEmailDtls>();
        public void Execute(IJobExecutionContext context)
        {
            bool RetValue = false;
            ChequeAccountingRepository obj = null;
            string result = string.Empty;
               List<AuditDtls> objList = new List<AuditDtls>();
            List<ChqSummaryForSalesTeamModel> modelList = new List<ChqSummaryForSalesTeamModel>();
            try
            {
                BusinessCont.SaveLog(0, 0, 0, "SendAlertEmailToInternalTeamSchedular", "Execute", "Scheduler Execution Start", BusinessCont.SuccessStatus);
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                Subject = ConfigurationManager.AppSettings["InternalTeamEmailSub"] + Date + " ";
                //ToEmail = ConfigurationManager.AppSettings["ToEmail"];
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\AlertForInternal_Audit.html");
                CCEmailList = _unitOfWork.chequeAccountingRepository.GetCCEmailDtlsPvt(1, 1, 7);
                if (CCEmailList.Count > 0)
                {
                    for (int i = 0; i < CCEmailList.Count; i++)
                    {
                        CCEmail += ";" + CCEmailList[i].Email;
                    }
                    EmailCC = CCEmail.TrimStart(';');
                }
                
                objList = _unitOfWork.chequeAccountingRepository.GetInternalTeamEmailList(0,0).ToList();
                if(objList.Count > 0)
                {
                    using (CFADBEntities _contextManager = new CFADBEntities())
                    {
                        msgHtml = "";
                        result = "";
                        msgHtml = File.OpenText(MailFilePath).ReadToEnd().ToString();
                        obj = new ChequeAccountingRepository(_contextManager);
                        result = obj.InterAuditHtml(objList, msgHtml);
                    }

                    var Email = _unitOfWork.chequeAccountingRepository.GetSalesTeamEmailList().ToList();
                    foreach (var item in Email)
                    {
                        RetValue = emailNotification.SendEmailForInternalAudit(item.Email, EmailCC, result, Subject);
                    }
                }
                BusinessCont.SaveLog(0, 0, 0, "SendAlertEmailToInternalTeamSchedular", "Execute", "Scheduler Execution End", BusinessCont.SuccessStatus);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "Send Alert Email To Internal Team Schedular", "", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
        }
    }
}