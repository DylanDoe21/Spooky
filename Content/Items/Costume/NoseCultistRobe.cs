using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Body)]
	public class NoseCultistRobe : ModItem
	{
		public override void Load() 
        {
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
		}

		public override void SetDefaults() 
        {
			Item.width = 26;
			Item.height = 28;
			Item.vanity = true;
            Item.rare = ItemRarityID.Blue;
		}

		public override void SetMatch(bool male, ref int equipSlot, ref bool robes) 
        {
			robes = true;
			equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
		}

		public override void EquipFrameEffects(Player player, EquipType type)
		{
			player.GetModPlayer<SpookyPlayer>().NoseCultistDisguise2 = true;
		}
	}
}