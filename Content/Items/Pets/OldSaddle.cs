using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Items.Pets
{
	public class OldSaddle : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 20;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 5);
			Item.mountType = ModContent.MountType<TrapdoorSpiderMount>();
			Item.UseSound = SoundID.Item77;
		}
	}
}