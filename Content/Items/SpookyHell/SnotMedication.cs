using Humanizer;
using Spooky.Content.NPCs.Cemetery;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell
{
    public class SnotMedication : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            for (int i = 0; i < player.buffType.Length; i++) 
			{ 
				if (player.buffType[i] == BuffID.OgreSpit || player.buffType[i] == BuffID.Poisoned || player.buffType[i] == BuffID.Venom)
				{
					int[] Types = new int[] { BuffID.Regeneration, BuffID.Swiftness, BuffID.Ironskin };

					player.buffTime[i] = 0;
					player.AddBuff(Main.rand.Next(Types), 600);
				}
			}
        }
    }
}