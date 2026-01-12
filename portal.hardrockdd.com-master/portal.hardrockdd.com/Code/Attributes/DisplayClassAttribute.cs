using System;

namespace portal
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DisplayClassAttribute : Attribute
    {
        private readonly string _name;

        public DisplayClassAttribute(string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }
    }
}