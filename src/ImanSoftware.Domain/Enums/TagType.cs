using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ImanSoftware.Domain;

/// <summary>
/// Predefined tag types with their display colors.
/// </summary>
public enum TagType
{
    [Description("#007bff")] 
    Technology,

    [Description("#28a745")]
    Science,

    [Description("#dc3545")]
    Art,

    [Description("#ffc107")]
    Business,

    [Description("#17a2b8")]
    Health,

    [Description("#6f42c1")]
    Education,

    [Description("#6c757d")]
    General
}