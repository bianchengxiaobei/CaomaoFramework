using System;

[AttributeUsage(AttributeTargets.Class)]
public class IconAttribute : Attribute
{
    public string iconName;
    public IconAttribute(string iconName)
    {
        this.iconName = iconName;
    }
}
public class NameAttribute : Attribute
{
    public string name;
    public NameAttribute(string name)
    {
        this.name = name;
    }
}
public class DescriptionAttribute : Attribute
{
    public string description;
    public DescriptionAttribute(string description)
    {
        this.description = description;
    }
}

