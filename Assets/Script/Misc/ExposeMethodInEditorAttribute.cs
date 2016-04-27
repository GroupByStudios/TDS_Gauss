using System;

// Restrict to methods only
[AttributeUsage(AttributeTargets.Method)]
public class ExposeMethodInEditorAttribute : Attribute
{
}