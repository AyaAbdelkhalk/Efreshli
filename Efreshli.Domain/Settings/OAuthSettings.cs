using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Domain.Settings
{
    public class OAuthSettings
    {
        public GoogleSettings Google { get; set; } = new GoogleSettings();
        public FacebookSettings Facebook { get; set; } = new FacebookSettings();
    }
}
