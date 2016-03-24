using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core
{
    public class Links
    {
        public string Display { get; internal set; }
        public string Edit { get; internal set; }
        public string Delete { get; internal set; }

        public bool HasEdit
        {
            get
            {
                return Edit.IsNullOrEmpty() == false;
            }
        }
        public bool HasDelete
        {
            get
            {
                return Delete.IsNullOrEmpty() == false;
            }
        }

        public int Count
        {
            get
            {
                var count = 0;

                if (HasEdit)
                {
                    count++;
                }

                if (HasDelete)
                {
                    count++;
                }

                if (!Display.IsNullOrEmpty())
                {
                    count++;
                }

                return count;
            }
        }
    }
}