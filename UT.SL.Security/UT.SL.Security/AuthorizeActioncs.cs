using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web;
using System.Web.WebPages;
using UT.SL.BLL;
using UT.SL.DAL;
using System.Reflection;
using UT.SL.Helper;

namespace UT.SL.Security
{
    public static class AuthorizeActioncs
    {
        ///Link
        public static IHtmlString AuthorizationActionLinkBootstrap(this HtmlHelper helper, string userName, int? objectId, int? objectType, string imageClass, string title
             , string actionName
             , string controllerName
             , object routeValues,
             object linkHtmlAttributes,
             string cntr)
        {

            if (Authorization.IsAuthorized(userName, objectId, objectType, actionName, controllerName, routeValues))
            {
                var builder = new TagBuilder("span");
                builder.MergeAttribute("class", imageClass + " glyColor");
                builder.MergeAttribute("title", title);

                var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, linkHtmlAttributes).ToHtmlString();
                if (string.IsNullOrEmpty(cntr))
                {
                    return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal)));
                }
                return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal) + string.Format(" {0}", cntr)));
            }
            else
                return null;
        }

        public static IHtmlString AuthorizationActionLinkBootstrap(this HtmlHelper helper, string userName, string imageClass, string title
             , string actionName
             , string controllerName
             , object routeValues,
             object linkHtmlAttributes,
             string cntr)
        {

            if (Authorization.IsAuthorized(userName, null, null, actionName, controllerName, routeValues))
            {
                var builder = new TagBuilder("span");
                builder.MergeAttribute("class", imageClass + " glyColor");
                builder.MergeAttribute("title", title);

                var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, linkHtmlAttributes).ToHtmlString();
                if (string.IsNullOrEmpty(cntr))
                {
                    return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal)));
                }
                return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal) + string.Format(" {0}", cntr)));
            }
            else
                return null;
        }

        public static IHtmlString AuthorizationActionLink(this HtmlHelper helper, string userName, int? objectId, int? objectType, string linkText
             , string actionName
             , string controllerName
             , object routeValues,
             object linkHtmlAttributes)
        {
            if (Authorization.IsAuthorized(userName, objectId, objectType, actionName, controllerName, routeValues))
            {
                return new HtmlString(helper.ActionLink(linkText, actionName, controllerName, routeValues, linkHtmlAttributes).ToHtmlString());
            }
            else
                return null;
        }

        public static IHtmlString AuthorizationActionLink(this HtmlHelper helper, string userName, string linkText
            , string actionName
            , string controllerName
            , object routeValues,
            object linkHtmlAttributes)
        {

            if (Authorization.IsAuthorized(userName, null, null, actionName, controllerName, routeValues))
            {
                return new HtmlString(helper.ActionLink(linkText, actionName, controllerName, routeValues, linkHtmlAttributes).ToHtmlString());
            }
            else
                return null;
        }

        public static IHtmlString AuthorizationImageActionLink(this HtmlHelper helper, string userName, int? objectId, int? objectType,
            string imageUrl,
            string altText,
            string actionName,
            string controllerName,
            object routeValues,
            object linkHtmlAttributes,
            object imgHtmlAttributes)
        {
            if (Authorization.IsAuthorized(userName, objectId, objectType, actionName, controllerName, routeValues))
            {
                var img = new ImageLink(helper);
                img.Src(imageUrl)
                    .Title(altText)
                    .ImageAttribute(imgHtmlAttributes)
                    .LinkAttribute(linkHtmlAttributes)
                    .Action(actionName)
                    .Controller(controllerName)
                    .RouteValue(routeValues);
                return new HtmlString(img.ToString());
            }
            else
                return null;
        }

        public static IHtmlString AuthorizationImageActionLink(this HtmlHelper helper, string userName,
           string imageUrl,
           string altText,
           string actionName,
           string controllerName,
           object routeValues,
           object linkHtmlAttributes,
           object imgHtmlAttributes)
        {
            if (Authorization.IsAuthorized(userName, null, null, actionName, controllerName, routeValues))
            {
                var img = new ImageLink(helper);
                img.Src(imageUrl)
                    .Title(altText)
                    .ImageAttribute(imgHtmlAttributes)
                    .LinkAttribute(linkHtmlAttributes)
                    .Action(actionName)
                    .Controller(controllerName)
                    .RouteValue(routeValues);
                return new HtmlString(img.ToString());
            }
            else
                return null;
        }

        ///Ajax
        public static IHtmlString AuthorizationActionLink(this AjaxHelper helper, string userName, int? objectId, int? objectType, string linkText
         , string actionName
         , string controllerName
         , object routeValues, AjaxOptions ajaxOptions,
         object linkHtmlAttributes)
        {
            if (Authorization.IsAuthorized(userName, objectId, objectType, actionName, controllerName, routeValues))
            {
                return new HtmlString(helper.ActionLink(linkText, actionName, controllerName, routeValues, ajaxOptions, linkHtmlAttributes).ToHtmlString());
            }
            else
                return null;
        }

        public static IHtmlString AuthorizationActionLink(this AjaxHelper helper, string userName, string linkText
         , string actionName
         , string controllerName
         , object routeValues, AjaxOptions ajaxOptions,
         object linkHtmlAttributes)
        {
            if (Authorization.IsAuthorized(userName, null, null, actionName, controllerName, routeValues))
            {
                return new HtmlString(helper.ActionLink(linkText, actionName, controllerName, routeValues, ajaxOptions, linkHtmlAttributes).ToHtmlString());
            }
            else
                return null;
        }

        public static IHtmlString AuthorizationImageActionLink(this AjaxHelper helper, string userName, int? objectId, int? objectType, string imageUrl, string altText
           , string actionName
           , string controllerName
           , object routeValues, AjaxOptions ajaxOptions,
           object linkHtmlAttributes,
           object imgHtmlAttributes)
        {
            if (Authorization.IsAuthorized(userName, objectId, objectType, actionName, controllerName, routeValues))
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(imgHtmlAttributes);

                var builder = new TagBuilder("img");
                builder.MergeAttribute("src", imageUrl);
                builder.MergeAttribute("alt", altText);
                builder.MergeAttribute("title", altText);

                foreach (var key in attributes.Keys)
                {
                    var value = attributes[key];
                    string valueAsString = null;
                    if (value != null)
                    {
                        valueAsString = value.ToString();
                    }
                    builder.MergeAttribute(key, valueAsString);
                }
                var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions, linkHtmlAttributes).ToHtmlString();
                return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing)));
            }
            else
                return null;
        }

        public static IHtmlString AuthorizationImageActionLink(this AjaxHelper helper, string userName, string imageUrl, string altText
          , string actionName
          , string controllerName
          , object routeValues, AjaxOptions ajaxOptions,
          object linkHtmlAttributes,
          object imgHtmlAttributes)
        {
            if (Authorization.IsAuthorized(userName, null, null, actionName, controllerName, routeValues))
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(imgHtmlAttributes);

                var builder = new TagBuilder("img");
                builder.MergeAttribute("src", imageUrl);
                builder.MergeAttribute("alt", altText);
                builder.MergeAttribute("title", altText);

                foreach (var key in attributes.Keys)
                {
                    var value = attributes[key];
                    string valueAsString = null;
                    if (value != null)
                    {
                        valueAsString = value.ToString();
                    }
                    builder.MergeAttribute(key, valueAsString);
                }
                var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions, linkHtmlAttributes).ToHtmlString();
                return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing)));
            }
            else
                return null;
        }

        public static IHtmlString AuthorizationActionLinkBootstrap(this AjaxHelper helper, string userName, int? objectId, int? objectType, string imageClass, string title
               , string actionName
               , string controllerName
               , object routeValues, AjaxOptions ajaxOptions
               , object linkHtmlAttributes
               , string cntr)
        {

            if (Authorization.IsAuthorized(userName, objectId, objectType, actionName, controllerName, routeValues))
            {
                var builder = new TagBuilder("span");
                builder.MergeAttribute("class", imageClass + " glyColor");
                if (!string.IsNullOrEmpty(title))
                    builder.MergeAttribute("title", title);
                var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions, linkHtmlAttributes).ToHtmlString();
                if (string.IsNullOrEmpty(cntr))
                {
                    return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal)));
                }
                return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal) + string.Format(" {0}", cntr)));
            }
            else
                return null;

        }

        public static IHtmlString AuthorizationActionLinkBootstrap(this AjaxHelper helper, string userName, string imageClass, string title
              , string actionName
              , string controllerName
              , object routeValues, AjaxOptions ajaxOptions
              , object linkHtmlAttributes
              , string cntr)
        {

            if (Authorization.IsAuthorized(userName, null, null, actionName, controllerName, routeValues))
            {
                var builder = new TagBuilder("span");
                builder.MergeAttribute("class", imageClass + " glyColor");
                if (!string.IsNullOrEmpty(title))
                    builder.MergeAttribute("title", title);
                var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions, linkHtmlAttributes).ToHtmlString();
                if (string.IsNullOrEmpty(cntr))
                {
                    return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal)));
                }
                return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal) + string.Format(" {0}", cntr)));
            }
            else
                return null;

        }

        public static IHtmlString AuthorizationImageActionLinkBootstrapForComment(this AjaxHelper helper, string userName, string imageClass, string title
               , string actionName
               , string controllerName
               , object routeValues, AjaxOptions ajaxOptions
               , object linkHtmlAttributes
               , string cntr
               , int objectType
               , int objectId)
        {
            if (Authorization.IsAuthorized(userName, objectId, objectType, actionName, controllerName, routeValues))
            {
                var spanId = "resourcecommentcount" + objectType + objectId;
                var builder = new TagBuilder("span");
                builder.MergeAttribute("class", imageClass + " glyColor");
                builder.MergeAttribute("title", title);
                var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions, linkHtmlAttributes).ToHtmlString();
                if (string.IsNullOrEmpty(cntr))
                {
                    return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal)));
                }
                return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal) + string.Format("<span id='{0}'> {1}<span>", spanId, cntr)));
            }
            else
                return null;

        }

    }
}
