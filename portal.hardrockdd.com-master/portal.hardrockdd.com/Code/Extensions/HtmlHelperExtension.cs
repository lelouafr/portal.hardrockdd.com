using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace portal
{
    public static class HtmlHelperExtension
    {

        public static MvcHtmlString CheckBoxSimple(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            string checkBoxWithHidden = htmlHelper.CheckBox(name, htmlAttributes).ToHtmlString().Trim();
            string pureCheckBox = checkBoxWithHidden.Substring(0, checkBoxWithHidden.IndexOf("<input", 1, StringComparison.Ordinal));
            return new MvcHtmlString(pureCheckBox);
        }

        public static MvcHtmlString LiActionLink(this HtmlHelper html, string text, string action, string controller)
        {
            if (html == null)
            {
                throw new System.ArgumentNullException(nameof(html));
            }
            var context = html.ViewContext;
            if (context.Controller.ControllerContext.IsChildAction)
                context = html.ViewContext.ParentActionViewContext;
            var routeValues = context.RouteData.Values;
            var currentAction = routeValues["action"].ToString();
            var currentController = routeValues["controller"].ToString();

            var isCurrent = currentAction.Equals(action, StringComparison.InvariantCulture) && currentController.Equals(controller, StringComparison.InvariantCulture);

            var str = string.Format(AppCultureInfo.CInfo(), "<li {0}>{1}</li>",
                isCurrent ? "class=\"active\"" : string.Empty, html.ActionLink(text, action, controller).ToHtmlString()
            );
            return new MvcHtmlString(str);
        }
        #region label
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString LabelForRequired<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            if (html == null)
            {
                throw new System.ArgumentNullException(nameof(html));
            }
            return LabelHelper(html,
                ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                ExpressionHelper.GetExpressionText(expression),
                "",
                null);
        }

        public static MvcHtmlString LabelForRequired<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes = null)
        {
            if (html == null)
            {
                throw new System.ArgumentNullException(nameof(html));
            }
            return LabelHelper(html,
                ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                ExpressionHelper.GetExpressionText(expression),
                labelText,
                htmlAttributes);
        }

        public static MvcHtmlString LabelForRequired<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            if (html == null)
            {
                throw new System.ArgumentNullException(nameof(html));
            }
            return LabelHelper(html,
                ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                ExpressionHelper.GetExpressionText(expression),
                "",
                htmlAttributes);
        }

        private static MvcHtmlString LabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string labelText, object htmlAttributes = null)
        {
            if (string.IsNullOrEmpty(labelText))
            {
                labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            }

            if (string.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            bool isRequired = false;
            if (metadata.ContainerType != null)
            {
                isRequired = metadata.ContainerType.GetProperty(metadata.PropertyName)
                                .GetCustomAttributes(typeof(RequiredAttribute), false)
                                .Length == 1;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.Attributes.Add(
                "for",
                TagBuilder.CreateSanitizedId(
                    html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)
                )
            );

            //if (isRequired)
            //{
            //    tag.Attributes.Add("class", "label-required");
            //}
            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tag.MergeAttributes(attributes);
            }
            tag.SetInnerText(labelText);

            if (isRequired)
            {
                var asteriskTag = new TagBuilder("span");
                asteriskTag.Attributes.Add("class", "text-danger small");
                asteriskTag.SetInnerText("*");
                tag.InnerHtml = labelText + "  " + asteriskTag.ToString(TagRenderMode.Normal);
                //labelText += asteriskTag.ToString(TagRenderMode.Normal);
            }

            var output = tag.ToString(TagRenderMode.Normal);

            //if (isRequired)
            //{//<span class="text-danger small">*</span>
            //    var asteriskTag = new TagBuilder("span");
            //    asteriskTag.Attributes.Add("class", "text-danger small");
            //    asteriskTag.SetInnerText("*");
            //    output += asteriskTag.ToString(TagRenderMode.Normal);
            //}

            return MvcHtmlString.Create(output);
        }
        #endregion

        public static MvcHtmlString EnumDropDownListForXX<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, Func<TEnum, bool> predicate, string optionLabel, object htmlAttributes) where TEnum : struct, IConvertible
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum");
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            IList<SelectListItem> selectList = Enum.GetValues(typeof(TEnum))
                    .Cast<TEnum>()
                    .Where(e => predicate(e))
                    .Select(e => new SelectListItem
                    {
                        Value = Convert.ToUInt64(value: e).ToString(AppCultureInfo.CInfo()),
                        Text = ((Enum)(object)e).GetDisplayName(),
                    }).ToList();
            if (!string.IsNullOrEmpty(optionLabel))
            {
                selectList.Insert(0, new SelectListItem
                {
                    Text = optionLabel,
                });
            }

            return htmlHelper.DropDownListFor(expression, selectList, htmlAttributes);
        }

        /// <summary>
        /// Gets the name in <see cref="DisplayAttribute"/> of the Enum.
        /// </summary>
        /// <param name="enumeration">A <see cref="Enum"/> that the method is extended to.</param>
        /// <returns>A name string in the <see cref="DisplayAttribute"/> of the Enum.</returns>
        
       
        #region EnumDisplayText
        public static MvcHtmlString EnumDisplayNameFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            if (html == null)
            {
                throw new System.ArgumentNullException(nameof(html));
            }
            var value = ModelMetadata.FromLambdaExpression(expression, html.ViewData).Model;
            if (value == null)
            {
                return new MvcHtmlString("");
            }
            var type = value.GetType();
            var member = type.GetMember(value.ToString());
            try
            {

                DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

                if (displayName != null)
                {
                    return new MvcHtmlString(displayName.Name);
                }

                return new MvcHtmlString(value.ToString());
            }
            catch (Exception)
            {

                return new MvcHtmlString("");
            }
        }
        #endregion
    }
}