using System;
using System.Collections.Generic;

namespace Newtonsoft.Json.Serialization
{
	internal delegate IEnumerable<KeyValuePair<object, object>> ExtensionDataGetter(object o);
}
