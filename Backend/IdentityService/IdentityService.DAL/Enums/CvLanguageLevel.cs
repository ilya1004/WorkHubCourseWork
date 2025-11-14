using System.ComponentModel;

namespace IdentityService.DAL.Enums;

public enum CvLanguageLevel
{
    [Description("A1")] A1 = 0,

    [Description("A2")] A2 = 1,

    [Description("B1")] B1 = 2,

    [Description("B2")] B2 = 3,

    [Description("C1")] C1 = 4,

    [Description("C2")] C2 = 5,

    [Description("Native")] Native = 6
}