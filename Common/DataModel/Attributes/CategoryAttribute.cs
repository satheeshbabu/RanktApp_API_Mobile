using System;

namespace DataModel.Attributes
{
        [AttributeUsage(AttributeTargets.Property)]
        public class CategoryAttribute : Attribute
        {
            //TODO Define Sub Category relationship with attribute construtor to 
            //handle parent
            public string FieldName { get; }
            public string SimpleName { get; }
            public string ParentCategory { get; }
            
            public CategoryAttribute(string fieldName, string simpleName)
            {
                FieldName = fieldName;
                SimpleName = simpleName;
            }

            public CategoryAttribute(string fieldName, string simpleName, string parentCategory)
            {
                FieldName = fieldName;
                SimpleName = simpleName;
                ParentCategory = parentCategory;
            }
    }
    
}