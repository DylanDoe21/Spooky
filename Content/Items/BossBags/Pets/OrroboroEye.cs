using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Items.BossBags.Pets
{
	public class OrroboroEye : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eye of Orro-Boro");
			Tooltip.SetDefault("Summons a ridable eye serpent"
			+ "\n'Hope it doesn't try to eat you'");
		}

		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.master = true;
			Item.rare = ItemRarityID.Master;
			Item.value = Item.buyPrice(gold: 15);
			Item.mountType = ModContent.MountType<OrroboroMount>();
			Item.UseSound = SoundID.Item77;
		}
	}
}