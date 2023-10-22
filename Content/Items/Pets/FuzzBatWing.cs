using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	[LegacyName("FuzzBatBlood")]
	public class FuzzBatWing : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 16;
			Item.height = 16;
			Item.shoot = ModContent.ProjectileType<FuzzBatPet>();
			Item.buffType = ModContent.BuffType<FuzzBatPetBuff>();
		}

        public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }
	}
}