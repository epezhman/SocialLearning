﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web;
using System.Web.WebPages;

namespace UT.SL.Helper
{
    public class ImageLink : IHtmlString
    {
        string _src = "";
        string _alt = "";
        string _title = "";
        string _controller = "";
        string _action = "";
        object _routeValue = null;
        object _linkAttribute = null;
        object _imgAttribute = null;
        HtmlHelper _helper = null;

        public ImageLink(HtmlHelper helper)
        {
            this._helper = helper;
        }

        public ImageLink Src(string imageUrl)
        {
            UrlHelper url = new UrlHelper(_helper.ViewContext.RequestContext);
            this._src = url.Content(imageUrl);
            return this;
        }

        public ImageLink Controller(string controllerName)
        {
            this._controller = controllerName;
            return this;
        }

        public ImageLink Action(string actionName)
        {
            this._action = actionName;
            return this;
        }

        public ImageLink RouteValue(object routeValue)
        {
            this._routeValue = routeValue;
            return this;
        }

        public ImageLink ImageAttribute(object imageAttr)
        {
            this._imgAttribute = imageAttr;
            return this;
        }

        public ImageLink LinkAttribute(object linkAttr)
        {
            this._linkAttribute = linkAttr;
            return this;
        }

        public ImageLink Alt(string altText)
        {
            this._alt = altText;
            return this;
        }

        public ImageLink Title(string titleText)
        {
            this._title = titleText;
            return this;
        }

        public override string ToString()
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(this._imgAttribute);

            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", this._src);
            builder.MergeAttribute("alt", this._alt);
            builder.MergeAttribute("title", this._title);

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

            var link = _helper.ActionLink("[placeholder]", _action, _controller, _routeValue, _linkAttribute);
            var text = link.ToHtmlString();
            text = text.Replace("[placeholder]", builder.ToString(TagRenderMode.SelfClosing));
            return text;
        }
        string IHtmlString.ToHtmlString()
        {
            return this.ToString();
        }
    }

    public static class ImageActionLinks
    {
        public static ImageLink ImageActionLink(this HtmlHelper helper)
        {
            return new ImageLink(helper);
        }

        public static IHtmlString ImageActionLink(this HtmlHelper helper,
            string imageUrl,
            string altText,
            string actionName,
            string controllerName,
            object routeValues,
            object linkHtmlAttributes,
            object imgHtmlAttributes)
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

        public static IHtmlString ImageActionLinkBootstrap(this HtmlHelper helper, string imageClass, string title
            , string actionName
            , string controllerName
            , object routeValues,
            object linkHtmlAttributes,
            string cntr)
        {
            var builder = new TagBuilder("i");
            builder.MergeAttribute("class", imageClass);
            builder.MergeAttribute("title", title);

            var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, linkHtmlAttributes).ToHtmlString();
            if (string.IsNullOrEmpty(cntr))
            {
                return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal)));
            }
            return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal) + string.Format(" {0}", cntr)));

        }

        public static IHtmlString ImageActionLink(this AjaxHelper helper, string imageUrl, string altText
           , string actionName
           , string controllerName
           , object routeValues, AjaxOptions ajaxOptions,
           object linkHtmlAttributes,
           object imgHtmlAttributes)
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


        public static IHtmlString ImageActionLinkBootstrap(this AjaxHelper helper, string imageClass, string title
               , string actionName
               , string controllerName
               , object routeValues, AjaxOptions ajaxOptions
               , object linkHtmlAttributes
               , string cntr)
        {

            var builder = new TagBuilder("i");
            builder.MergeAttribute("class", imageClass);
            if (!string.IsNullOrEmpty(title))
                builder.MergeAttribute("title", title);
            var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions, linkHtmlAttributes).ToHtmlString();
            if (string.IsNullOrEmpty(cntr))
            {
                return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal)));
            }
            return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal) + string.Format(" {0}", cntr)));

        }

        public static IHtmlString ImageActionLinkBootstrapForComment(this AjaxHelper helper, string imageClass, string title
               , string actionName
               , string controllerName
               , object routeValues, AjaxOptions ajaxOptions
               , object linkHtmlAttributes
               , string cntr
               , int objectType
               , int objectId)
        {
            var spanId = "resourcecommentcount" + objectType + objectId;
            var builder = new TagBuilder("i");
            builder.MergeAttribute("class", imageClass);
            builder.MergeAttribute("title", title);
            var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions, linkHtmlAttributes).ToHtmlString();
            if (string.IsNullOrEmpty(cntr))
            {
                return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal)));
            }
            return new HtmlString(link.Replace("[replaceme]", builder.ToString(TagRenderMode.Normal) + string.Format("<span id='{0}'> {1}<span>", spanId, cntr)));

        }



    }

    public static class AuthorHelpers
    {
        public static MvcHtmlString Script(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
            return MvcHtmlString.Empty;
        }

        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    if (template != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }
            return MvcHtmlString.Empty;
        }
    }

}