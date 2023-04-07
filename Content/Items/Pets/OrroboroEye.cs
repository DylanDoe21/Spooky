using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Items.Pets
{
	public class OrroboroEye : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.master = true;
			Item.rare = ItemRarityID.Master;
			Item.value = Item.buyPrice(gold: 5);
			Item.mountType = ModContent.MountType<OrroboroMount>();
			Item.UseSound = SoundID.Item77;
		}
	}
}