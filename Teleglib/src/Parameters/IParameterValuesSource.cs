using System.Collections.Generic;

namespace Teleglib.Parameters {
    public interface IParameterValuesSource {
        IEnumerable<ParameterValue> GetValues();
    }
}