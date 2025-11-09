using cc.dingemans.bigibas123.bulkmaterialgenerators.Editor;
using cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.MaterialVariantGen;
using cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.TextureArrayConverter;
using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(BulkMaterialGenerators))]

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Editor
{
    public class BulkMaterialGenerators : Plugin<BulkMaterialGenerators>
    {
        public static readonly string SQualifiedName = "cc.dingemans.bigibas123.bigishader.BulkMaterialGenerators";
        public static readonly string SDisplayName = "Bulk Material Generators";
        public override string QualifiedName => SQualifiedName;
        public override string DisplayName => SDisplayName;

        public static class Passes
        {
            public static readonly ConvertTextureArrayPass ConvertTextureArrayPass = new();
            public static readonly MaterialVariantGenPass MaterialVariantGentPass = new();
        }


        protected override void Configure()
        {
            InPhase(BuildPhase.Generating)
                .Run(Passes.ConvertTextureArrayPass);
            InPhase(BuildPhase.Generating)
                .Run(Passes.MaterialVariantGentPass);
        }

        public class ConvertTextureArrayPass : Pass<ConvertTextureArrayPass>
        {
            public override string QualifiedName => SQualifiedName + ".ConvertTextureArray";
            public override string DisplayName => $"[{SDisplayName}] Convert textureArray shader to normal";

            protected override void Execute(BuildContext context)
            {
                var targets = context.AvatarRootObject.GetComponentsInChildren<Runtime.TextureArrayConverter>(true);
                foreach (var converter in targets)
                {
                    converter.Process();
                }
            }
        }

        public class MaterialVariantGenPass : Pass<MaterialVariantGenPass>
        {
            public override string QualifiedName => SQualifiedName + ".MaterialVariantGen";
            public override string DisplayName => $"[{SDisplayName}] Generate Materials from list of textures";

            protected override void Execute(BuildContext context)
            {
                var targets = context.AvatarRootObject.GetComponentsInChildren<Runtime.MaterialVariantGen>(true);
                foreach (var converter in targets)
                {
                    converter.Process();
                }
            }
        }
    }
}