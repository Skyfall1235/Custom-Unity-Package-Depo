using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubclassListAttribute : PropertyAttribute
{
    Type type;
    public Type Type => type;
    public SubclassListAttribute(System.Type type)
    {
        this.type = type;
    }
}
