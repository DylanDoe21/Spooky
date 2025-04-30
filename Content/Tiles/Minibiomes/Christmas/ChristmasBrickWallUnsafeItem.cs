using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Christmas
{
    public class ChristmasBrickWallUnsafeItem : ModItem
    {
		public override string Texture => "Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasBrickWallItem";

		public override void SetStaticDefaults()
        {
			ItemID.Sets.DrawUnsafeIndicator[Type] = true;
			Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<ChristmasBrickWall>());
            Item.width = 16;
			Item.height = 16;
        }

		/*
		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Vector2 vector2 = new Vector2(-4f, -4f) * scale;
			Texture2D value7 = TextureAssets.Extra[258].Value;
			Rectangle rectangle2 = value7.Frame();

			Main.spriteBatch.Draw(value7, Item.Center + vector2 + new Vector2(14f) * scale, rectangle2, itemColor, 0f, rectangle2.Size() / 2f, 1f, SpriteEffects.None, 0f);
		}
		*/
	}

    public class ChristmasBrickWallAltUnsafeItem : ModItem
    {
		public override string Texture => "Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasBrickWallAltItem";

		public override void SetStaticDefaults()
        {
			ItemID.Sets.DrawUnsafeIndicator[Type] = true;
			Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<ChristmasBrickWallAlt>());
            Item.width = 16;
			Item.height = 16;
        }
	}
}