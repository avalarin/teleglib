using System;
using System.Collections.Immutable;
using Teleglib.Features;
using Teleglib.Renderers;

namespace Teleglib.Middlewares {
    public class MiddlewareData {

        private readonly ImmutableFeaturesSet<IFeature> _features;
        private readonly ImmutableList<IClientRenderer> _renderers;

        public MiddlewareData() {
            _features = new ImmutableFeaturesSet<IFeature>();
            _renderers = ImmutableList<IClientRenderer>.Empty;
        }

        private MiddlewareData(ImmutableFeaturesSet<IFeature> features, ImmutableList<IClientRenderer> renderers) {
            _features = features;
            _renderers = renderers;
        }

        public IFeaturesAccessor<IFeature> Features => _features;

        public ImmutableList<IClientRenderer> Renderers => _renderers;

        public MiddlewareData UpdateFeatures(
            Func<ImmutableFeaturesSet<IFeature>, ImmutableFeaturesSet<IFeature>> updater) {

            var newFeatures = updater(_features);
            return new MiddlewareData(newFeatures, _renderers);
        }

        public MiddlewareData AddRenderer(IClientRenderer renderer) {
            var newRenderers = Renderers.Add(renderer);
            return new MiddlewareData(_features, newRenderers);
        }

    }
}