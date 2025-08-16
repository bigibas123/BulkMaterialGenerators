using cc.dingemans.bigibas123.bulkmaterialgenerators.Editor;
using cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.TextureArrayConverter;
using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(BulkMaterialGenerators))]

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Editor
{
    public class BulkMaterialGenerators : Plugin<BulkMaterialGenerators>
    {
        public override string QualifiedName => "cc.dingemans.bigibas123.bigishader.BulkMaterialGenerators";
        public override string DisplayName => "LogoPlane Splitter";

        public static readonly string TAG = "[LogoPlaneSplitter]";

        protected override void Configure()
        {
            InPhase(BuildPhase.Generating)
                .Run("Convert textureArray shader to normal ", ctx =>
                {
                    var targets = ctx.AvatarRootObject.GetComponentsInChildren<Runtime.TextureArrayConverter>();
                    foreach (var converter in targets)
                    {
                        converter.Process();
                    }
                });
        }
    }
}