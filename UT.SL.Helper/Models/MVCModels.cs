using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace UT.SL.Helper
{


    public class DALReturnModel<T> where T : class 
    {
        public T ReturnObject { get; set; }
        public BatchProcessResultModel BPR { get; set; }

        public DALReturnModel()
        {
            this.BPR = new BatchProcessResultModel();
        }

        public DALReturnModel(BatchProcessResultModel BPR)
        {
            this.BPR = BPR;
        }

        public DALReturnModel(T FormObject)
        {
            this.ReturnObject = FormObject;
            this.BPR = new BatchProcessResultModel();
        }

        public DALReturnModel(T FormObject, BatchProcessResultModel BPR)
        {
            this.ReturnObject = FormObject;
            this.BPR = BPR;
        }

    }

    public class FormModel<T> where T : class
    {
        public T FormObject { get; set; }
        public IList<object> ExtraData { get; set; }
        public BatchProcessResultModel BPR { get; set; }

        public FormModel()
        {
            this.BPR = new BatchProcessResultModel();
            this.ExtraData = new List<object>();
        }

        public FormModel(BatchProcessResultModel BPR)
        {
            this.BPR = BPR;
            this.ExtraData = new List<object>();
        }

        public FormModel(T FormObject)
        {
            this.FormObject = FormObject;
            this.BPR = new BatchProcessResultModel();
            this.ExtraData = new List<object>();
        }

    }

    public class FormModel<T, U>
        where T : class
        where U : class
    {
        public T FormObject { get; set; }
        public U ExtraKnownData { get; set; }
        public IList<object> ExtraData { get; set; }
        public BatchProcessResultModel BPR { get; set; }

        public FormModel()
        {
            this.BPR = new BatchProcessResultModel();
            this.ExtraData = new List<object>();
        }

        public FormModel(BatchProcessResultModel BPR)
        {
            this.BPR = BPR;
            this.ExtraData = new List<object>();
        }

        public FormModel(T FormObject)
        {
            this.FormObject = FormObject;
            this.BPR = new BatchProcessResultModel();
            this.ExtraData = new List<object>();
        }

    }

    public class ErrorModel
    {
        public List<string> ErrorMessages { get; set; }
    }

    public class CustomDropDownModel
    {
        public string Area { get; set; }
        public object SelectedValue { get; set; }
        public string FieldName { get; set; }

        public CustomDropDownModel()
        {
            FieldName = "DowpDown";
            SelectedValue = null;
            Area = "";
        }
    }
    public class CustomDropDownModel<T> where T : class
    {
        public string Area { get; set; }
        public object SelectedValue { get; set; }
        public string FieldName { get; set; }
        public IList<T> Items { get; set; }

        public CustomDropDownModel(CustomDropDownModel obj)
        {
            FieldName = obj.FieldName;
            SelectedValue = obj.SelectedValue;
            Area = obj.Area;
        }

        public CustomDropDownModel()
        {
            FieldName = "DowpDown";
            SelectedValue = null;
            Area = "";
        }
    }

    public class DateModel
    {
        public string Area { get; set; }
        public DateTime? SelectedDate { get; set; }
        public int MaxYear { get; set; }
        public int MinYear { get; set; }

        public string FieldName { get; set; }
        public bool? Validate { get; set; }

        public DateModel()
        {
            Area = "";
            FieldName = "CreateDate";
            MaxYear = 1389;
            MinYear = 1350;
            SelectedDate = DateTime.Now;
        }
    }

    public class SelectListItems
    {
        public string[] SelectedIds { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
    }

    public class BatchProcessResultModel
    {

        public int Success
        { get; set; }

        public int Failed
        { get; set; }

        public string Message
        { get; set; }

        public string ErrorCss
        { get; set; }

        public string SuccessCss
        { get; set; }

        public Guid GuidKeyId
        { get; set; }

        public int IntKeyId
        { get; set; }

        public string StringKeyId
        { get; set; }

        public string Title
        { get; set; }

        public string ClientScript { get; set; }
        public string FailedClientScript { get; set; }
        public string SuccessClientScript { get; set; }
        public void Clear()
        {
            Message = string.Empty;
            Failed = 0;
            Success = 0;
        }

        public void AppendMessage(string newMessage, string cssClass = "", bool addLi = true)
        {
            if (!string.IsNullOrEmpty(newMessage))
            {
                if (!string.IsNullOrEmpty(cssClass))
                {
                    if (addLi)
                        Message += string.Format("<li class='{0}'>{1}</li>", cssClass, newMessage);
                    else
                        Message += string.Format("{0}", newMessage);
                }
                else
                {
                    if (addLi)
                        Message += string.Format("<li>{0}</li>", newMessage);
                    else
                        Message += string.Format("{0}", newMessage);
                }
            }
        }

        public void AddModelStateErrors(ModelStateDictionary modelState, bool addCssClass = false, bool addLi = true)
        {
            foreach (var v in modelState.Values)
                foreach (var e in v.Errors)
                    this.AddError(e.ErrorMessage, addCssClass, addLi);
        }

        public void AddError(string newMessage, bool addCssClass = false, bool addLi = true)
        {
            Failed++;
            AppendMessage(newMessage, addCssClass ? ErrorCss : "", addLi);
        }

        public void AddSuccess(string newMessage, bool addCssClass = false, bool addLi = true)
        {
            Success++;
            AppendMessage(newMessage, addCssClass ? SuccessCss : "", addLi);
        }

        public void CoverMessageWithUL()
        {
            this.Message = string.Format("<ul>{0}</ul>", this.Message);
        }

        public BatchProcessResultModel()
        {
            Success = 0;
            Failed = 0;
            ErrorCss = "red";
            SuccessCss = "green";
        }

    }

}
