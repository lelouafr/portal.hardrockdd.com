using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace portal
{

    public class FormLayoutModel
    {
        public FormLayoutModel(WebViewPage viewPage)
        {
            if (viewPage == null)
            {
                throw new System.ArgumentNullException(nameof(viewPage));
            }
            var myObject = viewPage.Model.GetType();
            Attribute[] attrs = Attribute.GetCustomAttributes(myObject);
            var classDescriptionName = "";

            foreach (Attribute attr in attrs)
            {
                if (!(attr is DisplayClassAttribute displayAttribute))
                    continue;
                classDescriptionName = displayAttribute.GetName();
            }
            var modelProps = viewPage.ViewData.ModelMetadata.Properties;

            var props = from prop in modelProps
                        where
                        prop.TemplateHint != "HiddenInput"
                        select new
                        {
                            prop.PropertyName,
                            Attributes = (FieldAttribute)modelProps.Where(w => w.PropertyName == prop.PropertyName)
                                                                   .FirstOrDefault()
                                                                   .AdditionalValues
                                                                   .Where(w => w.Key == "FieldAttribute")
                                                                   .FirstOrDefault().Value ?? new FieldAttribute(),
                            AdditionalValues = modelProps.Where(w => w.PropertyName == prop.PropertyName)
                                                                      .FirstOrDefault()
                                                                      .AdditionalValues.ToList()
                        };

            SetName = classDescriptionName;
            FormGroups = props.Where(w => w.Attributes != null)
                              .GroupBy(g => g.Attributes.FormGroup)
                              .Select(s => new FormGroupModel(s.GroupBy(a => a.Attributes.FormGroupRow)
                                                     .Select(b => new FormGroupRowModel(b.Select(c => new PropertyLayoutModel
                                                     {
                                                         Property = c.PropertyName,
                                                         IsKey = c.AdditionalValues.Where(w => w.Key == "KeyAttribute").Any(),
                                                         IsHidden = c.AdditionalValues.Where(w => w.Key == "HiddenInputAttribute").Any(),
                                                         Attributes = c.Attributes
                                                     }).ToList())
                                                     {
                                                     }).ToList())
                              {
                                  FormGroupName = s.Key
                              }).ToList();



        }

        public FormLayoutModel()
        {

        }

        public string SetName { get; set; }

        public List<FormGroupModel> FormGroups { get; }
    }

    public class FormGroupModel
    {
        public FormGroupModel()
        {

        }

        public FormGroupModel(List<FormGroupRowModel> formGroupRows)
        {
            FormGroupRows = formGroupRows;
        }

        public string FormGroupName { get; set; }

        public List<FormGroupRowModel> FormGroupRows { get; }
    }

    public class FormGroupRowModel
    {
        public FormGroupRowModel()
        {

        }
        public FormGroupRowModel(List<PropertyLayoutModel> properties)
        {
            Properties = properties;
        }
        public List<PropertyLayoutModel> Properties { get; }
    }

    public class TableLayoutModel
    {
        public TableLayoutModel(WebViewPage viewPage)
        {
            if (viewPage == null)
            {
                throw new System.ArgumentNullException(nameof(viewPage));
            }
            Properties = new List<PropertyLayoutModel>();
            if (viewPage.Model == null)
            {
                return;
            }
            var isList = viewPage.Model.GetType().IsGenericType && viewPage.Model.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
            var myObject = viewPage.Model.GetType();
            if (isList)
            {
                var modelList = (IList)viewPage.Model;

                foreach (var item in modelList)
                {
                    myObject = item.GetType();
                    continue;
                }

            }
            Attribute[] attrs = Attribute.GetCustomAttributes(myObject);
            var classDescriptionName = "";
            foreach (Attribute attr in attrs)
            {
                if (!(attr is DisplayClassAttribute displayAttribute))
                    continue;
                classDescriptionName = displayAttribute.GetName();
            }
            foreach (Attribute attr in attrs)
            {
                if (!(attr is TableControllerClassAttribute displayAttribute))
                    continue;
                ControllerInfo = displayAttribute;
            }
            var modelProps = viewPage.ViewData.ModelMetadata.Properties;

            var props = from prop in modelProps
                            //where
                            //prop.TemplateHint != "HiddenInput"
                        select new
                        {
                            prop.PropertyName,
                            propertyType = modelProps.Where(w => w.PropertyName == prop.PropertyName)
                                                                   .FirstOrDefault()
                                                                   .GetType(),
                            Attributes = (TableFieldAttribute)modelProps.Where(w => w.PropertyName == prop.PropertyName)
                                                                      .FirstOrDefault()
                                                                      .AdditionalValues
                                                                      .Where(w => w.Key == "TableAttribute")
                                                                      .FirstOrDefault().Value ?? new TableFieldAttribute(),
                            AdditionalValues = modelProps.Where(w => w.PropertyName == prop.PropertyName)
                                                                      .FirstOrDefault()
                                                                      .AdditionalValues.ToList()

                        };

            foreach (var prop in props)
            {
                Properties.Add(new PropertyLayoutModel
                {
                    IsKey = prop.AdditionalValues.Where(w => w.Key == "KeyAttribute").Any(),
                    IsHidden = prop.AdditionalValues.Where(w => w.Key == "HiddenInputAttribute").Any(),//prop.propertyType.GetCustomAttributes(typeof(HiddenInputAttribute), true).Length != 0,
                    EditorTemplate = prop.Attributes.EditorTemplate,
                    InternalTableRow = prop.Attributes.InternalTableRow,
                    Width = prop.Attributes.Width,
                    Property = prop.PropertyName
                });
            }

            foreach (var prp in Properties)
            {
                if (prp.IsKey)
                {
                    var myData = viewPage.Model.GetType().GetProperty(prp.Property);
                    var myVal = myData.GetValue(viewPage.Model, null);
                    if (myVal != null)
                    {
                        if (!string.IsNullOrEmpty(TableKey))
                        {
                            TableKey += "&";
                        }
                        TableKey += prp.Property + "=" + myVal.ToString();

                    }
                }
            }
        }

        public TableLayoutModel()
        {

        }

        public TableControllerClassAttribute ControllerInfo { get; set; }

        public List<PropertyLayoutModel> Properties { get; }

        public string TableKey { get; set; }
    }

    public class PropertyLayoutModel
    {
        public int InternalTableRow { get; set; }

        public bool IsKey { get; set; }

        public bool IsHidden { get; set; }

        public bool IsRequired { get; set; }

        public string Width { get; set; }

        public string Property { get; set; }

        public string Label { get; set; }

        public string EditorTemplate { get; set; }

        public FieldAttribute Attributes { get; set; }
    }
}