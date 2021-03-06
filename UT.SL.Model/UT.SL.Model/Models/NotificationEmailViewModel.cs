﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UT.SL.Data.LINQ;

namespace UT.SL.Model
{
    public class NotificationEmailViewModel
    {
        public int ObjectId { get; set; }
        public int ObjectType { get; set; }
        public ObjectViewModel NotificationOnObject { get; set; }
        public App_User OwnerUser { get; set; }
        public App_User CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public NotificationEmail Notification { get; set; }

        public NotificationEmailViewModel()
        {
        }

    }
}
