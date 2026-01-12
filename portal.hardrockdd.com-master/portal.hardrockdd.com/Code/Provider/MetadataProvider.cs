using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal
{
    public class MetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes,
                                                        Type containerType,
                                                        Func<object> modelAccessor,
                                                        Type modelType,
                                                        string propertyName)
        {
            var metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            var fieldAttribute = attributes.OfType<FieldAttribute>().FirstOrDefault();
            if (fieldAttribute != null)
            {
                metadata.AdditionalValues.Add("FieldAttribute", fieldAttribute);
            }

            var tableAttribute = attributes.OfType<TableFieldAttribute>().FirstOrDefault();
            if (tableAttribute != null)
            {
                metadata.AdditionalValues.Add("TableAttribute", tableAttribute);
            }

            var keyAttribute = attributes.OfType<KeyAttribute>().FirstOrDefault();
            if (keyAttribute != null)
            {
                metadata.AdditionalValues.Add("KeyAttribute", keyAttribute);
            }

            var hiddentAttribute = attributes.OfType<HiddenInputAttribute>().FirstOrDefault();
            if (hiddentAttribute != null)
            {
                metadata.AdditionalValues.Add("HiddenInputAttribute", hiddentAttribute);
            }

            return metadata;
        }
    }
}