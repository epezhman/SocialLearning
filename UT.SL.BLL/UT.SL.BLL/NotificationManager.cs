/*
 * ****************************************************************
 * Filename:        NotificationManager.cs 
 * version:         
 * Author's name:   Fatemeh orooji, Pezhman Nasirifard, Elearning lab 
 * Creation date:   
 * Purpose:         
 * ****************************************************************
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.DAL;
using UT.SL.Data.LINQ;
using UT.SL.Helper;
using UT.SL.Model;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;
using UT.SL.Model.Enumeration;

namespace UT.SL.BLL
{
    /// <summary>
    /// 
    /// </summary>
    public static class NotificationManager
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static NotificationViewModel GetInfo(Notification model)
        {
            var returnModel = new NotificationViewModel();
            returnModel.CreateDate = model.CreateDate;
            returnModel.OwnerUser = model.App_User;

            var dbmodel = ManageObject.GetSharedObject(model.ObjectId, model.ObjectType);
            returnModel.NotificationOnObject = dbmodel;
            returnModel.CreateUser = dbmodel.CreateUser;

            returnModel.ObjectId = model.ObjectId;
            returnModel.ObjectType = model.ObjectType;

            returnModel.Notification = model;

            if (model.ObjectType == (int)ObjectType.Vote)
            {
                           
            }
            else if (model.ObjectType == (int)ObjectType.Comment)
            { }
            else if (model.ObjectType == (int)ObjectType.TagMapper)
            { }
            else if (model.ObjectType == (int)ObjectType.Grade)
            { }
            else if (model.ObjectType == (int)ObjectType.ForumDiscussion)
            { }
            else if (model.ObjectType == (int)ObjectType.ObjectStream)
            {
            }

            return returnModel;
        }
    }
}
