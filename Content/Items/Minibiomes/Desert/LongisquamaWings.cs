using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Minibiomes.Desert
{
	[AutoloadEquip(EquipType.Wings)]
	public class LongisquamaWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new Terraria.DataStructures.WingStats(80, 6.5f, 1f);
		}

		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 26;
			Item.value = Item.buyPrice(gold: 15);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noFallDmg = true;
        }
	}
}