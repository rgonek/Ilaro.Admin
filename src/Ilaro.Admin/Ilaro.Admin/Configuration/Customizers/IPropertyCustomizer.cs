using Ilaro.Admin.Core;

namespace Ilaro.Admin.Configuration.Customizers
{
    public interface IPropertyCustomizer
    {
        void Column(string columnName);
        void Display(string singular, string plural);
        void Id();
        void OnDelete(DeleteOption deleteOption);
        void Template(string display = null, string editor = null);
        void Type(DataType dataType);
        void Searchable();
        void Visible();
        void Image();
        void File();
    }
}