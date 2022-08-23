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

namespace CNF.API.Classes
{
    public class SendEmailChqSummaryForMonthly: BaseApiController, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            List<ChqSummaryForMonthlyModel> ChqSummaryForMonthlyList = new List<ChqSummaryForMonthlyModel>();
            string Subject = string.Empty, MailFilePath = string.Empty, CCEmail = string.Empty, EmailCC = string.Empty, msgHtmlOutput = string.Empty;
            bool IsEmailSend = false;
            EmailNotification emailNotification = new EmailNotification();
            ChequeAccountingRepository chequeAccountingRepository = null;
            List<CCEmailDtls> CCEmailList = new List<CCEmailDtls>();
            try
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailChqSummaryForMonthly", "Execute", "Scheduler Execution START", "");
                Subject = ConfigurationManager.AppSettings["ChqSummaryForMonthlySubject"];
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\SendEmailChqSummaryForMonthly.html");
                using (CFADBEntities contextManager = new CFADBEntities())
                {
                    CCEmailList = _unitOfWork.chequeAccountingRepository.GetCCEmailDtlsPvt(1, 1, 7);
                    if (CCEmailList.Count > 0)
                    {
                        for (int i = 0; i < CCEmailList.Count; i++)
                        {
                            CCEmail += ";" + CCEmailList[i].Email;
                        }
                        EmailCC = CCEmail.TrimStart(';');
                    }
                    ChqSummaryForMonthlyList = _unitOfWork.chequeAccountingRepository.GetChqSummaryForMonthlyList(0, 0, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")));
                    if (ChqSummaryForMonthlyList.Count > 0)
                    {
                        chequeAccountingRepository = new ChequeAccountingRepository(contextManager);
                        msgHtmlOutput = chequeAccountingRepository.GetChqSummaryForMonthlyReport(ChqSummaryForMonthlyList, MailFilePath);
                        foreach (var item in ChqSummaryForMonthlyList)
                        {
                            //"anilshinde@aadyamconsultant.com"
                            IsEmailSend = emailNotification.SendEmailToChqSummaryForMonthlyOrWeekly(item.Emailid, EmailCC, Subject, msgHtmlOutput);
                        }
                    }
                }
                BusinessCont.SaveLog(0, 0, 0, "SendEmailChqSummaryForMonthly", "Execute", "Scheduler Execution END", BusinessCont.SuccessStatus);
            }
            catch(Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "SendEmailChqSummaryForMonthly", "Execute", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
        }
    }
}