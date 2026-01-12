using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace portal
{
    public static class EnumExtenstion
    {
        public static string GetDisplayName(this Enum enumeration)
        {
            if (enumeration == null)
            {
                throw new ArgumentNullException(nameof(enumeration));
            }
            Type enumType = enumeration.GetType();
            string enumName = Enum.GetName(enumType, enumeration);
            string displayName = enumName;
            try
            {
                MemberInfo member = enumType.GetMember(enumName)[0];

                object[] attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
                DisplayAttribute attribute = (DisplayAttribute)attributes[0];
                displayName = attribute.Name;

                if (attribute.ResourceType != null)
                {
                    displayName = attribute.GetName();
                }
            }
            catch { }
            return displayName;
        }

        public static string DisplayName(this Enum value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            FieldInfo field = value.GetType().GetField(value.ToString());


            return !(Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute) ? value.ToString() : attribute.Name;
        }
    }
}