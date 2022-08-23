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
    public class SendAlertToSalesTeamForChqSmmry : BaseApiController, IJob
    {
        EmailNotification emailNotification = new EmailNotification();
        string Subject = string.Empty, CCEmail = string.Empty, result = string.Empty, Date = string.Empty, EmailCC = string.Empty, MailFilePath = string.Empty;
        List<CCEmailDtls> CCEmailList = new List<CCEmailDtls>();
        public void Execute(IJobExecutionContext context)
        {
            bool RetValue = false;
            string msgHTMLOutput = string.Empty;
            List<ChqSummaryForSalesTeamModel> modelList = new List<ChqSummaryForSalesTeamModel>();
            List<ChqDepositDetails> chqDepositDetails = new List<ChqDepositDetails>();
            try
            {
                BusinessCont.SaveLog(0, 0, 0, "SendAlertToSalesTeamForChqSmmrySchedular", "Execute", "Scheduler Execution Start", BusinessCont.SuccessStatus);
                Date = DateTime.Today.Date.ToString("dd-MM-yyyy");
                Subject = ConfigurationManager.AppSettings["ChqSmmryAlertEmailSub"] + Date + " ";
                MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile\\SendAlertToSalesTeamForChqSmmry.html");
                CCEmailList = _unitOfWork.chequeAccountingRepository.GetCCEmailDtlsPvt(1, 1, 7);
                if (CCEmailList.Count > 0)
                {
                    for (int i = 0; i < CCEmailList.Count; i++)
                    {
                        CCEmail += ";" + CCEmailList[i].Email;
                    }
                    EmailCC = CCEmail.TrimStart(';');
                }

                using (CFADBEntities contextManager = new CFADBEntities())
                {
                    modelList = _unitOfWork.chequeAccountingRepository.GetRptChqSummaryForSalesTeamList(0, 0);
                    if (modelList.Count > 0)
                    {
                        msgHTMLOutput = _unitOfWork.chequeAccountingRepository.GetChequeSummaryForSalesTeamForReport(modelList, MailFilePath);
                        var result = _unitOfWork.chequeAccountingRepository.GetSalesTeamEmailList().ToList();
                        if (result.Count > 0)
                        {
                            foreach (var item in result)
                            {
                                RetValue = emailNotification.SendAlertToSalesTeamForChqSmmry(item.Email, EmailCC, Subject, msgHTMLOutput);
                            }
                        }           
                    }         
                }        
                BusinessCont.SaveLog(0, 0, 0, "SendAlertToSalesTeamForChqSmmrySchedular", "Execute", "Scheduler Execution End", BusinessCont.SuccessStatus);
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "Send Alert To Sales Team For Chq Smmry Schedular", "", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
        }
    }
}