namespace Ilaro.Admin.Core
{
    public enum DeleteOption : int
    {
        Nothing = 0,
        SetNull = 1,
        CascadeDelete = 2,
        AskUser = 3
    }
}