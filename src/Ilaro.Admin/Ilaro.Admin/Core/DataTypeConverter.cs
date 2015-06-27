namespace Ilaro.Admin.Core
{
    public static class DataTypeConverter
    {
        public static DataType Convert(System.ComponentModel.DataAnnotations.DataType sourceDataType)
        {
            switch (sourceDataType)
            {
                case System.ComponentModel.DataAnnotations.DataType.DateTime:
                case System.ComponentModel.DataAnnotations.DataType.Date:
                case System.ComponentModel.DataAnnotations.DataType.Time:
                case System.ComponentModel.DataAnnotations.DataType.Duration:
                    return DataType.DateTime;
                case System.ComponentModel.DataAnnotations.DataType.Url:
                case System.ComponentModel.DataAnnotations.DataType.ImageUrl:
                case System.ComponentModel.DataAnnotations.DataType.Upload:
                    return DataType.File;
                case System.ComponentModel.DataAnnotations.DataType.Currency:
                    return DataType.Numeric;
                case System.ComponentModel.DataAnnotations.DataType.Password:
                    return DataType.Password;
                default:
                    return DataType.Text;
            }
        }
    }
}