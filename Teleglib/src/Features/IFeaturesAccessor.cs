using System.Collections.Generic;
using Teleglib.Utils;

namespace Teleglib.Features {
    public interface IFeaturesAccessor<TBase> {

        bool Has<T>() where T : TBase;

        Maybe<T> GetOne<T>() where T : TBase;

        T RequireOne<T>() where T : TBase;

        IEnumerable<T> GetAll<T>() where T : TBase;

        IEnumerable<T> GetAllOfBaseType<T>();

    }
}